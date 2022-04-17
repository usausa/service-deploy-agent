namespace Service.Deploy.Agent.Managers;

using Service.Deploy.Agent.Settings;

public interface IServiceManager
{
    public ValueTask<bool> StartAsync(ServiceEntry entry, CancellationToken cancel);

    public ValueTask<bool> StopAsync(ServiceEntry entry, CancellationToken cancel);
}
