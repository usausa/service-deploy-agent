namespace Service.Deploy.Agent.Helpers
{
    using PInvoke;

    public static class ServiceHelper
    {
        public static bool CreateService(string serviceName, string displayName, string binPath)
        {
            using var scManager = AdvApi32.OpenSCManager(null, null, AdvApi32.ServiceManagerAccess.SC_MANAGER_CREATE_SERVICE);
            if (scManager.IsInvalid)
            {
                return false;
            }

            using var service = AdvApi32.CreateService(
                scManager,
                serviceName,
                displayName,
                0,
                AdvApi32.ServiceType.SERVICE_WIN32_OWN_PROCESS,
                AdvApi32.ServiceStartType.SERVICE_AUTO_START, // TODO demand
                AdvApi32.ServiceErrorControl.SERVICE_ERROR_IGNORE,
                binPath,
                null,
                0,
                null,
                null,
                null);
            if (service.IsInvalid)
            {
                return false;
            }

            return true;
        }

        public static bool DeleteService(string serviceName)
        {
            using var scManager = AdvApi32.OpenSCManager(null, null, AdvApi32.ServiceManagerAccess.SC_MANAGER_CREATE_SERVICE);
            if (scManager.IsInvalid)
            {
                return false;
            }

            using var service = AdvApi32.OpenService(
                scManager,
                serviceName,
                AdvApi32.ServiceAccess.SERVICE_ALL_ACCESS);
            if (service.IsInvalid)
            {
                return false;
            }

            return AdvApi32.DeleteService(service);
        }

        public static bool StartService(string serviceName)
        {
            using var scManager = AdvApi32.OpenSCManager(null, null, AdvApi32.ServiceManagerAccess.SC_MANAGER_CREATE_SERVICE);
            if (scManager.IsInvalid)
            {
                return false;
            }

            using var service = AdvApi32.OpenService(
                scManager,
                serviceName,
                AdvApi32.ServiceAccess.SERVICE_START);
            if (service.IsInvalid)
            {
                return false;
            }

            return AdvApi32.StartService(service, 0, null);
        }

        public static bool StopService(string serviceName)
        {
            using var scManager = AdvApi32.OpenSCManager(null, null, AdvApi32.ServiceManagerAccess.SC_MANAGER_CREATE_SERVICE);
            if (scManager.IsInvalid)
            {
                return false;
            }

            using var service = AdvApi32.OpenService(
                scManager,
                serviceName,
                AdvApi32.ServiceAccess.SERVICE_STOP);
            if (service.IsInvalid)
            {
                return false;
            }

            var status = default(AdvApi32.SERVICE_STATUS);
            return AdvApi32.ControlService(service, AdvApi32.ServiceControl.SERVICE_CONTROL_STOP, ref status);
        }
    }
}
