namespace Service.Deploy.Client.Commands
{
    using System.CommandLine;

    using Service.Deploy.Client.Framework;

    public sealed class RootCommandBuilder : GroupCommandBase
    {
        protected override Command CreateCommand() => new RootCommand("Service deploy tool");
    }
}
