using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Xenial.Licensing.Cli.Commands
{
    public class XenialRootCommand : ICommandHandler
    {
        public RootCommand RootCommand { get; }

        public XenialRootCommand(IServiceProvider serviceProvider)
        {
            RootCommand = new RootCommand();

            var commandHandlerTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => !type.IsAbstract && typeof(IXenialCommandHandler).IsAssignableFrom(type));

            foreach (var commandHandlerType in commandHandlerTypes.Select(type => new
            {
                Type = type,
                Attribute = type.GetCustomAttribute<XenialCommandHandlerAttribute>()
            }).Where(item => item.Attribute != null))
            {
                if (!typeof(IXenialCommand).IsAssignableFrom(commandHandlerType.Attribute.CommandType))
                {
                    throw new ArgumentException();
                }

                var subCommand = new Command(commandHandlerType.Attribute.CommandName, commandHandlerType.Attribute.Description);
                if (!string.IsNullOrEmpty(commandHandlerType.Attribute.ShortCut))
                {
                    subCommand.AddAlias(commandHandlerType.Attribute.ShortCut);
                }

                var command = (IXenialCommand)Activator.CreateInstance(commandHandlerType.Attribute.CommandType);

                foreach (var option in command.CreateOptions())
                {
                    subCommand.AddOption(option);
                }

                subCommand.Handler = new XenialCommandHandler(serviceProvider, command, commandHandlerType.Type);

                RootCommand.Add(subCommand);
            }

            RootCommand.Handler = this;
        }

        public Task<int> InvokeAsync(InvocationContext context)
        {
            //TODO: invoke hello command if wizard was never ran

            new HelpResult().Apply(context);

            return Task.FromResult(-1);
        }
    }
}
