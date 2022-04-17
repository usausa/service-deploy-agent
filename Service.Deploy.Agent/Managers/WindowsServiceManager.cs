namespace Service.Deploy.Agent.Managers;

using Service.Deploy.Agent.Helpers;
using Service.Deploy.Agent.Settings;

public class WindowsServiceManager : IServiceManager
{
    public ValueTask<bool> StartAsync(ServiceEntry entry, CancellationToken cancel)
    {
        if (!ServiceHelper.CreateService(entry.Name, entry.Display ?? entry.Name, entry.BinPath))
        {
            return ValueTask.FromResult(false);
        }

        if (!ServiceHelper.StartService(entry.Name))
        {
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
                return false;
            }
        }

        ServiceHelper.DeleteService(entry.Name);

        return true;
    }
}
