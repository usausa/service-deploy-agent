namespace Service.Deploy.Agent.Settings;

public class ServiceSetting
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819", Justification = "Ignore")]
    public ServiceEntry[] Entry { get; set; } = default!;
}
