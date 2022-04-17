namespace Service.Deploy.Agent.Managers;

using System.Diagnostics;

using Service.Deploy.Agent.Settings;

public class SystemdServiceManager : IServiceManager
{
    public ValueTask<bool> StartAsync(ServiceEntry entry, CancellationToken cancel)
    {
        using var chmod = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/usr/bin/chmod",
                Arguments = $"+x {entry.BinPath}"
            }
        };
        chmod.Start();
        chmod.WaitForExit();
        if (chmod.ExitCode != 0)
        {
            return ValueTask.FromResult(false);
        }

        using var service = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/usr/bin/systemctl",
                Arguments = $"start {entry.Name}"
            }
        };
        service.Start();
        service.WaitForExit();
        if (service.ExitCode != 0)
        {
            return ValueTask.FromResult(false);
        }

        return ValueTask.FromResult(true);
    }

    public ValueTask<bool> StopAsync(ServiceEntry entry, CancellationToken cancel)
    {
        using var service = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/usr/bin/systemctl",
                Arguments = $"stop {entry.Name}"
            }
        };
        service.Start();
        service.WaitForExit();
        if (service.ExitCode != 0)
        {
            return ValueTask.FromResult(false);
        }

        return ValueTask.FromResult(true);
    }
}
