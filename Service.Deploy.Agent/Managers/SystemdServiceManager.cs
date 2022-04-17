namespace Service.Deploy.Agent.Managers;

using Service.Deploy.Agent.Settings;

public class SystemdServiceManager : IServiceManager
{
    public ValueTask<bool> StartAsync(ServiceEntry entry, CancellationToken cancel)
    {
        throw new NotImplementedException();
    }

    public ValueTask<bool> StopAsync(ServiceEntry entry, CancellationToken cancel)
    {
        throw new NotImplementedException();
    }
}
