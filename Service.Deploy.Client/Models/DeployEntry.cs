namespace Service.Deploy.Client.Models;

#pragma warning disable CA1056
public sealed class DeployEntry
{
    public string Name { get; set; } = default!;

    public string? Url { get; set; }

    public string? Token { get; set; }
}
#pragma warning restore CA1056
