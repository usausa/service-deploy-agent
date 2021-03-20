namespace ServiceControl
{
    using System;
    using System.Diagnostics;
    using PInvoke;

    public static class Program
    {
        public static void Main()
        {
            Debug.WriteLine(StartService("WebApp"));
            Debug.WriteLine(StopService("WebApp"));
        }

        //using var scManager = AdvApi32.OpenSCManager(null, null, AdvApi32.ServiceManagerAccess.SC_MANAGER_CREATE_SERVICE);
        //Console.WriteLine(scManager);
        //using var service = AdvApi32.CreateService(
        //    scManager,
        //    "WebApp",
        //    "Web Application",
        //    0,
        //    AdvApi32.ServiceType.SERVICE_WIN32_OWN_PROCESS,
        //    AdvApi32.ServiceStartType.SERVICE_AUTO_START,
        //    AdvApi32.ServiceErrorControl.SERVICE_ERROR_IGNORE,
        //    @"D:\WebApplication\WebApplication.exe",
        //    null,
        //    0,
        //    null,
        //    null,
        //    null);
        //Console.WriteLine(service);

        private static bool StartService(string serviceName)
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

        private static bool StopService(string serviceName)
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
