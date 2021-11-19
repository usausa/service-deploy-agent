namespace Service.Deploy.Client.Models;

using System;
using System.Diagnostics.CodeAnalysis;

public class DeploySetting
{
    [SuppressMessage("Performance", "CA1819", Justification = "Ignore")]
    public DeployEntry[] Entries { get; set; } = Array.Empty<DeployEntry>();
}
