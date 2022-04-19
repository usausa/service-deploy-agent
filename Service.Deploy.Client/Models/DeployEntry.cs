namespace Service.Deploy.Client.Models;

public class DeployEntry
{
    public string Name { get; set; } = default!;

    public string? Url { get; set; }

    public string? Token { get; set; }
}
