using System.CommandLine;
using System.IO.Compression;

using Microsoft.Extensions.DependencyInjection;

using Service.Deploy.Client;
using Service.Deploy.Client.Models;
using Service.Deploy.Client.Services;

using Smart.CommandLine.Hosting;

var builder = CommandHost.CreateBuilder(args);

builder.ConfigureCommands(commands =>
{
    commands.ConfigureRootCommand(root =>
    {
        root.WithDescription("Service deploy tool");
    });

    commands.AddCommands();
});

var host = builder.Build();
return await host.RunAsync();
