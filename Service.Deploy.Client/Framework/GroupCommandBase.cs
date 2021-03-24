namespace Service.Deploy.Client.Framework
{
    using System.Collections.Generic;
    using System.CommandLine;

    public abstract class GroupCommandBase : ICommandBuilder
    {
        private readonly List<ICommandBuilder> children = new();

        public GroupCommandBase Add(params ICommandBuilder[] commands)
        {
            foreach (var command in commands)
            {
                children.Add(command);
            }

            return this;
        }

        public Command Build()
        {
            var command = CreateCommand();
            foreach (var child in children)
            {
                command.Add(child.Build());
            }

            return command;
        }

        protected abstract Command CreateCommand();
    }
}
