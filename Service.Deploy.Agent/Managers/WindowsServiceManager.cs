namespace Service.Deploy.Agent.Managers;

using Service.Deploy.Agent.Helpers;
using Service.Deploy.Agent.Settings;

public class WindowsServiceManager : IServiceManager
{
    private readonly ILogger<WindowsServiceManager> log;

    public WindowsServiceManager(ILogger<WindowsServiceManager> log)
    {
        this.log = log;
    }

    public ValueTask<bool> StartAsync(ServiceEntry entry, CancellationToken cancel)
    {
        if (!ServiceHelper.CreateService(entry.Name, entry.Display ?? entry.Name, entry.BinPath))
        {
            log.LogWarning("CreateService failed. name=[{Name}]", entry.Name);
            return ValueTask.FromResult(false);
        }

        if (!ServiceHelper.StartService(entry.Name))
        {
            log.LogWarning("StartService failed. name=[{Name}]", entry.Name);
            return ValueTask.FromResult(false);
        }

        return ValueTask.FromResult(true);
    }

    public async ValueTask<bool> StopAsync(ServiceEntry entry, CancellationToken cancel)
    {
        if (ServiceHelper.StopService(entry.Name))
        {
            if (!await ServiceHelper.WaitForStopAsync(entry.Name, 10000, cancel))
            {
                log.LogWarning("StopService wait failed. name=[{Name}]", entry.Name);
                return false;
            }
        }
        else
        {
            log.LogInformation("Service not exist. name=[{Name}]", entry.Name);
        }

        ServiceHelper.DeleteService(entry.Name);

        return true;
    }
}
