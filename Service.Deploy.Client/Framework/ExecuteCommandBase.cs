namespace Service.Deploy.Client.Framework;

using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading;
using System.Threading.Tasks;

public abstract class ExecuteCommandBase<TParameter> : ICommandBuilder
{
    public Command Build()
    {
        var command = CreateCommand();
        command.Handler = CommandHandler.Create(async (TParameter parameter, IConsole console, CancellationToken cancel)
            => await ExecuteAsync(parameter, console, cancel));
        return command;
    }

    protected abstract Command CreateCommand();

    protected abstract ValueTask<int> ExecuteAsync(TParameter parameter, IConsole console, CancellationToken cancel);
}
