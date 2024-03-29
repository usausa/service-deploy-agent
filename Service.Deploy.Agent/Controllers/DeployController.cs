namespace Service.Deploy.Agent.Controllers;

using System.IO.Compression;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Service.Deploy.Agent.Managers;
using Service.Deploy.Agent.Settings;

[ApiController]
[Route("[controller]/[action]")]
public sealed class DeployController : ControllerBase
{
    private readonly ILogger<DeployController> log;

    private readonly IOptionsSnapshot<ServiceSetting> serviceSetting;

    private readonly IServiceManager serviceManager;

    public DeployController(
        ILogger<DeployController> log,
        IOptionsSnapshot<ServiceSetting> serviceSetting,
        IServiceManager serviceManager)
    {
        this.log = log;
        this.serviceSetting = serviceSetting;
        this.serviceManager = serviceManager;
    }

    [HttpPost("{name}")]
    public async ValueTask<IActionResult> Update(
        string name,
        IFormFile archive,
        [FromHeader(Name = "X-Deploy-Token")] string? token,
        CancellationToken cancel)
    {
        log.InfoDeployUpdateRequest(name);

        var entry = serviceSetting.Value.Entry.FirstOrDefault(x => x.Name == name);
        if (entry is null)
        {
            log.WarnDeployEntryNotFound(name);
            return NotFound();
        }

        if (!String.IsNullOrEmpty(entry.Token) && (entry.Token != token))
        {
            log.WarnDeployTokenIsInvalid(name, token);
            return BadRequest();
        }

        // Stop service
        if (!await serviceManager.StopAsync(entry, cancel))
        {
            log.WarnStopServiceFailed(name);
            return Problem("Stop service failed.");
        }

        // Extract archive
        for (var i = 0; i < 15; i++)
        {
            // ReSharper disable once EmptyGeneralCatchClause
#pragma warning disable CA1031
            try
            {
                if (Directory.Exists(entry.Directory))
                {
                    Directory.Delete(entry.Directory, true);
                }
                break;
            }
            catch
            {
            }
#pragma warning restore CA1031

            await Task.Delay(1000, cancel);
        }

        Directory.CreateDirectory(entry.Directory);

        await using var stream = archive.OpenReadStream();
        using var zip = new ZipArchive(stream, ZipArchiveMode.Read);
        zip.ExtractToDirectory(entry.Directory);

        // Start service
        if (!await serviceManager.StartAsync(entry, cancel))
        {
            log.WarnStartServiceFailed(name);
            return Problem("Start service failed.");
        }

        log.InfoDeployUpdateSuccess(name);
        return Ok("Update success.");
    }
}
