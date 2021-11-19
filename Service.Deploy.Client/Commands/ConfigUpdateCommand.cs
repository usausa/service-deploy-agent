namespace Service.Deploy.Client.Commands;

using System.CommandLine;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using Service.Deploy.Client.Framework;
using Service.Deploy.Client.Models;
using Service.Deploy.Client.Services;

public sealed class ConfigUpdateCommand : ExecuteCommandBase<ConfigUpdateCommand.Parameter>
{
    public class Parameter : ConfigCommand.Parameter
    {
        [AllowNull]
        public string Name { get; set; }

        public string? Url { get; set; }

        public string? Token { get; set; }
    }

    protected override Command CreateCommand() => new("update", "Update config")
    {
        new Option<string>(new[] { "--name", "-n" }, "Service name") { IsRequired = true },
        new Option<string>(new[] { "--url", "-u" }, "Agent url"),
        new Option<string>(new[] { "--token", "-t" }, "Authentication token")
    };

    protected override ValueTask<int> ExecuteAsync(Parameter parameter, IConsole console, CancellationToken cancel)
    {
        var config = new ConfigRepository(parameter.Config);
        config.Update(new DeployEntry { Name = parameter.Name, Url = parameter.Url, Token = parameter.Token });

        return ValueTask.FromResult(0);
    }
}
