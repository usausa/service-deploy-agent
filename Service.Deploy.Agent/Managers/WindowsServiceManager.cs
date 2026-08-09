namespace Service.Deploy.Agent.Managers;

using Service.Deploy.Agent.Helpers;
using Service.Deploy.Agent.Settings;

public sealed class WindowsServiceManager : IServiceManager
{
    private const int StopTimeout = 30000;

    public ValueTask<bool> StartAsync(ServiceEntry entry, CancellationToken cancel) =>
        ValueTask.FromResult(ServiceHelper.StartService(entry.Name));

    public async ValueTask<bool> StopAsync(ServiceEntry entry, CancellationToken cancel)
    {
        if (ServiceHelper.IsStopped(entry.Name))
        {
            return true;
        }

        if (!ServiceHelper.StopService(entry.Name))
        {
            return false;
        }

        return await ServiceHelper.WaitForStopAsync(entry.Name, StopTimeout, cancel);
    }
}
