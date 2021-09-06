namespace Service.Deploy.Client.Commands
{
    using System.CommandLine;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;

    using Service.Deploy.Client.Framework;
    using Service.Deploy.Client.Services;

    public sealed class ConfigDeleteCommand : ExecuteCommandBase<ConfigDeleteCommand.Parameter>
    {
        public class Parameter : ConfigCommand.Parameter
        {
            [AllowNull]
            public string Name { get; set; }
        }

        protected override Command CreateCommand() => new("delete", "Delete config")
        {
            new Option<string>(new[] { "--name", "-n" }, "Service name") { IsRequired = true }
        };

        protected override ValueTask<int> ExecuteAsync(Parameter parameter, IConsole console, CancellationToken cancel)
        {
            var config = new ConfigRepository(parameter.Config);
            config.Delete(parameter.Name);

            return ValueTask.FromResult(0);
        }
    }
}
