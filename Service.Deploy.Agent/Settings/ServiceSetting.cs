namespace Service.Deploy.Agent.Settings
{
    using System.Diagnostics.CodeAnalysis;

    public class ServiceEntry
    {
        [AllowNull]
        public string Name { get; set; }

        public string? Display { get; set; }

        public string? Token { get; set; }

        [AllowNull]
        public string Path { get; set; }
    }

    public class ServiceSetting
    {
        [AllowNull]
        public ServiceEntry[] Entry { get; set; }
    }
}
