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
        public string Directory { get; set; }

        [AllowNull]
        public string BinPath { get; set; }
    }

    public class ServiceSetting
    {
        [SuppressMessage("Performance", "CA1819", Justification = "Ignore")]
        [AllowNull]
        public ServiceEntry[] Entry { get; set; }
    }
}
