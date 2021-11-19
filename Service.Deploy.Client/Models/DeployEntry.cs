namespace Service.Deploy.Client.Models;

using System.Diagnostics.CodeAnalysis;

public class DeployEntry
{
    [AllowNull]
    public string Name { get; set; }

    public string? Url { get; set; }

    public string? Token { get; set; }
}
