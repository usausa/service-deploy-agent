namespace Service.Deploy.Client.Models;

#pragma warning disable CA1819
public sealed class DeploySetting
{
    public DeployEntry[] Entries { get; set; } = [];
}
#pragma warning restore CA1819
