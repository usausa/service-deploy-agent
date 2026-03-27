namespace Service.Deploy.Agent.Managers;

using Service.Deploy.Agent.Settings;

public interface IServiceManager
{
    ValueTask<bool> StartAsync(ServiceEntry entry, CancellationToken cancel);

    ValueTask<bool> StopAsync(ServiceEntry entry, CancellationToken cancel);
}
