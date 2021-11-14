namespace Service.Deploy.Agent.Controllers
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    using Service.Deploy.Agent.Helpers;
    using Service.Deploy.Agent.Settings;

    [ApiController]
    [Route("[controller]/[action]")]
    public class DeployController : ControllerBase
    {
        private readonly ILogger<DeployController> log;

        private readonly ServiceSetting serviceSetting;

        public DeployController(
            ILogger<DeployController> log,
            IOptionsSnapshot<ServiceSetting> serviceSetting)
        {
            this.log = log;
            this.serviceSetting = serviceSetting.Value;
        }

        [HttpPost("{name}")]
        public async ValueTask<IActionResult> Update(
            string name,
            IFormFile archive,
            [FromHeader(Name = "X-Deploy-Token")] string? token,
            CancellationToken cancel)
        {
            log.LogInformation("Deploy update request. name=[{Name}]", name);

            var entry = serviceSetting.Entry.FirstOrDefault(x => x.Name == name);
            if (entry is null)
            {
                log.LogWarning("Deploy entry not found. name=[{Name}]", name);
                return NotFound();
            }

            if (!String.IsNullOrEmpty(entry.Token) && (entry.Token != token))
            {
                log.LogWarning("Deploy token is invalid. name=[{Name}], token=[{Token}]", name, token);
                return Forbid();
            }

            // Stop & Delete service
            if (ServiceHelper.StopService(entry.Name))
            {
                if (!await ServiceHelper.WaitForStopAsync(entry.Name, 10000, cancel))
                {
                    log.LogWarning("Stop service failed. name=[{Name}]", name);
                    return Problem("Stop service failed.");
                }
            }

            ServiceHelper.DeleteService(entry.Name);

            // Extract archive
            if (Directory.Exists(entry.Directory))
            {
                Directory.Delete(entry.Directory, true);
            }
            Directory.CreateDirectory(entry.Directory);

            await using var stream = archive.OpenReadStream();
            using var zip = new ZipArchive(stream, ZipArchiveMode.Read);
            zip.ExtractToDirectory(entry.Directory);

            // Create & Start service
            if (!ServiceHelper.CreateService(entry.Name, entry.Display ?? entry.Name, entry.BinPath))
            {
                log.LogWarning("Create service failed. name=[{Name}]", name);
                return Problem("Create service failed.");
            }

            if (!ServiceHelper.StartService(entry.Name))
            {
                log.LogWarning("Start service failed. name=[{Name}]", name);
                return Problem("Start service failed.");
            }

            log.LogInformation("Deploy update success. name=[{Name}]", name);
            return Ok("Update success.");
        }
    }
}
