namespace Service.Deploy.Agent.Controllers;

using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

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
    private const int DeleteRetryCount = 15;

    private static readonly TimeSpan DeleteRetryInterval = TimeSpan.FromSeconds(1);

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

        if (!IsValidToken(entry.Token, token))
        {
            log.WarnDeployTokenIsInvalid(name);
            return BadRequest();
        }

        // Checked before stopping
        if (String.IsNullOrWhiteSpace(entry.BinPath) || !IsDeployableDirectory(entry.Directory))
        {
            log.WarnDeployEntryIsInvalid(name);
            return Problem("Service entry is not configured correctly.");
        }

        // Stop service
        if (!await serviceManager.StopAsync(entry, cancel))
        {
            log.WarnStopServiceFailed(name);
            return Problem("Stop service failed.");
        }

        // Extract archive
        if (!await DeleteDirectoryAsync(entry.Directory, cancel))
        {
            log.WarnDeleteDirectoryFailed(name, entry.Directory);
            _ = await serviceManager.StartAsync(entry, cancel);
            return Problem("Delete directory failed.");
        }

        Directory.CreateDirectory(entry.Directory);

        await using var stream = archive.OpenReadStream();
        await using var zip = new ZipArchive(stream, ZipArchiveMode.Read);
        await zip.ExtractToDirectoryAsync(entry.Directory, cancel);

        // Start service
        if (!await serviceManager.StartAsync(entry, cancel))
        {
            log.WarnStartServiceFailed(name);
            return Problem("Start service failed.");
        }

        log.InfoDeployUpdateSuccess(name);
        return Ok("Update success.");
    }

    private static bool IsDeployableDirectory(string? path)
    {
        if (String.IsNullOrWhiteSpace(path) || !Path.IsPathFullyQualified(path))
        {
            return false;
        }

        try
        {
            var full = Path.TrimEndingDirectorySeparator(Path.GetFullPath(path));
            var pathRoot = Path.TrimEndingDirectorySeparator(Path.GetPathRoot(full) ?? String.Empty);
            return !String.Equals(full, pathRoot, StringComparison.OrdinalIgnoreCase);
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    private static bool IsValidToken(string? expected, string? actual)
    {
        if (String.IsNullOrEmpty(expected) || String.IsNullOrEmpty(actual))
        {
            return false;
        }

        return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(expected), Encoding.UTF8.GetBytes(actual));
    }

    private static async ValueTask<bool> DeleteDirectoryAsync(string path, CancellationToken cancel)
    {
        for (var i = 0; i < DeleteRetryCount; i++)
        {
            if (i > 0)
            {
                await Task.Delay(DeleteRetryInterval, cancel);
            }

            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }

                return true;
            }
            catch (IOException)
            {
                // Retry
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        return false;
    }
}
