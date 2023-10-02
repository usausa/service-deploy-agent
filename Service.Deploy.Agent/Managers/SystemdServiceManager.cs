namespace Service.Deploy.Agent.Managers;

using System.Diagnostics;

using Service.Deploy.Agent.Settings;

public class SystemdServiceManager : IServiceManager
{
    public async ValueTask<bool> StartAsync(ServiceEntry entry, CancellationToken cancel)
    {
        using var chmod = new Process();
        chmod.StartInfo = new ProcessStartInfo
        {
            FileName = "/usr/bin/chmod",
            Arguments = $"+x {entry.BinPath}"
        };
        chmod.Start();
        await chmod.WaitForExitAsync(cancel);
        if (chmod.ExitCode != 0)
        {
            return false;
        }

        using var service = new Process();
        service.StartInfo = new ProcessStartInfo
        {
            FileName = "/usr/bin/systemctl",
            Arguments = $"start {entry.Name}"
        };
        service.Start();
        await service.WaitForExitAsync(cancel);
        if (service.ExitCode != 0)
        {
            return false;
        }

        return true;
    }

    public async ValueTask<bool> StopAsync(ServiceEntry entry, CancellationToken cancel)
    {
        using var service = new Process();
        service.StartInfo = new ProcessStartInfo
        {
            FileName = "/usr/bin/systemctl",
            Arguments = $"stop {entry.Name}"
        };
        service.Start();
        await service.WaitForExitAsync(cancel);
        if (service.ExitCode != 0)
        {
            return false;
        }

        return true;
    }
}
