#pragma warning disable CA1812
using System.CommandLine;

using Service.Deploy.Client.Commands;

var root = new RootCommandBuilder().Add(
    new DeployCommand(),
    new ConfigCommand().Add(
        new ConfigUpdateCommand(),
        new ConfigDeleteCommand())).Build();
return await root.InvokeAsync(args);
