namespace ServiceControl
{
    using System;

    using PInvoke;

    public static class Program
    {
        public static void Main()
        {
            using var scManager = AdvApi32.OpenSCManager(null, null, AdvApi32.ServiceManagerAccess.SC_MANAGER_CREATE_SERVICE);
            Console.WriteLine(scManager);
            using var service = AdvApi32.CreateService(
                scManager,
                "WebApp",
                "Web Application",
                0,
                AdvApi32.ServiceType.SERVICE_WIN32_OWN_PROCESS,
                AdvApi32.ServiceStartType.SERVICE_AUTO_START,
                AdvApi32.ServiceErrorControl.SERVICE_ERROR_IGNORE,
                @"D:\WebApplication\WebApplication.exe",
                null,
                0,
                null,
                null,
                null);
            Console.WriteLine(service);
        }
    }
}
