#pragma warning disable CA1416
namespace Service.Deploy.Agent.Helpers;

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Windows.Win32;
using Windows.Win32.System.Services;

public static class ServiceHelper
{
    public static bool CreateService(string serviceName, string displayName, string binPath)
    {
        using var scManager = PInvoke.OpenSCManager(null, null, PInvoke.SC_MANAGER_CREATE_SERVICE);
        if (scManager.IsInvalid)
        {
            return false;
        }

        using var service = PInvoke.CreateService(
            scManager,
            serviceName,
            displayName,
            PInvoke.SERVICE_ALL_ACCESS,
            ENUM_SERVICE_TYPE.SERVICE_WIN32_OWN_PROCESS,
            SERVICE_START_TYPE.SERVICE_AUTO_START,
            SERVICE_ERROR.SERVICE_ERROR_IGNORE,
            binPath,
            null,
            out _);
        if (service.IsInvalid)
        {
            return false;
        }

        return true;
    }

    public static bool DeleteService(string serviceName)
    {
        using var scManager = PInvoke.OpenSCManager(null, null, PInvoke.SC_MANAGER_CREATE_SERVICE);
        if (scManager.IsInvalid)
        {
            return false;
        }

        using var service = PInvoke.OpenService(scManager, serviceName, PInvoke.SERVICE_ALL_ACCESS);
        if (service.IsInvalid)
        {
            return false;
        }

        return PInvoke.DeleteService(service);
    }

    public static unsafe bool StartService(string serviceName)
    {
        using var scManager = PInvoke.OpenSCManager(null, null, PInvoke.SC_MANAGER_CREATE_SERVICE);
        if (scManager.IsInvalid)
        {
            return false;
        }

        using var service = PInvoke.OpenService(scManager, serviceName, PInvoke.SERVICE_START);
        if (service.IsInvalid)
        {
            return false;
        }

        var hService = new SC_HANDLE(service.DangerousGetHandle());
        return PInvoke.StartService(hService, 0, null);
    }

    public static bool StopService(string serviceName)
    {
        using var scManager = PInvoke.OpenSCManager(null, null, PInvoke.SC_MANAGER_CREATE_SERVICE);
        if (scManager.IsInvalid)
        {
            return false;
        }

        using var service = PInvoke.OpenService(scManager, serviceName, PInvoke.SERVICE_STOP);
        if (service.IsInvalid)
        {
            return false;
        }

        return PInvoke.ControlService(service, PInvoke.SERVICE_CONTROL_STOP, out _);
    }

    public static async ValueTask<bool> WaitForStopAsync(string serviceName, int timeout, CancellationToken cancel = default)
    {
        using var scManager = PInvoke.OpenSCManager(null, null, PInvoke.SC_MANAGER_CREATE_SERVICE);
        if (scManager.IsInvalid)
        {
            return false;
        }

        using var service = PInvoke.OpenService(scManager, serviceName, PInvoke.SERVICE_QUERY_STATUS);
        if (service.IsInvalid)
        {
            return false;
        }

        var watch = Stopwatch.StartNew();
        do
        {
            if (!PInvoke.QueryServiceStatus(service, out var status))
            {
                return false;
            }

            if (status.dwCurrentState == SERVICE_STATUS_CURRENT_STATE.SERVICE_STOPPED)
            {
                return true;
            }

            await Task.Delay(50, cancel);
        }
        while (watch.ElapsedMilliseconds < timeout);

        return false;
    }
}
