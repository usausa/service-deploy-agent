namespace Service.Deploy.Client.Framework;

using System.CommandLine;

public interface ICommandBuilder
{
    Command Build();
}
