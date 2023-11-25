namespace Service.Deploy.Client.Models;

using System;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable CA1819
public class DeploySetting
{
    public DeployEntry[] Entries { get; set; } = Array.Empty<DeployEntry>();
}
#pragma warning restore CA1819
