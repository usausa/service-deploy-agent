namespace Service.Deploy.Client
{
    using System.CommandLine;
    using System.Threading.Tasks;

    using Service.Deploy.Client.Commands;

    public static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var root = new RootCommandBuilder().Add(
                new DeployCommand(),
                new ConfigCommand().Add(
                    new ConfigUpdateCommand(),
                    new ConfigDeleteCommand())).Build();
            return await root.InvokeAsync(args);
        }
    }
}
