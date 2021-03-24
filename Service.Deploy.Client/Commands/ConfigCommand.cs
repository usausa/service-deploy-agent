namespace Service.Deploy.Client.Commands
{
    using System.CommandLine;
    using System.Diagnostics.CodeAnalysis;
    using Service.Deploy.Client.Framework;

    public sealed class ConfigCommand : GroupCommandBase
    {
        public class Parameter
        {
            [AllowNull]
            public string Config { get; set; }
        }

        protected override Command CreateCommand() => new("config", "Config settings")
        {
            new Option<string>(new[] { "--config", "-c" }, "Config file")
        };
    }
}
