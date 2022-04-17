namespace Service.Deploy.Agent.Settings;

public class ServiceEntry
{
    public string Name { get; set; } = default!;

    public string? Display { get; set; }

    public string? Token { get; set; }

    public string Directory { get; set; } = default!;

    public string BinPath { get; set; } = default!;
}
