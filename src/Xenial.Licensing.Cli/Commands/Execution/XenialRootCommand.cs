using System;
using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Xenial.Licensing.Cli.Services.Default;

namespace Xenial.Licensing.Cli.Commands
{
    public class XenialRootCommand : ICommandHandler
    {
        public XenialDefaultCommand Command { get; }
        public RootCommand RootCommand { get; }

        public XenialRootCommand(IServiceProvider serviceProvider)
        {
            Command = new XenialDefaultCommand();
            RootCommand = new RootCommand();

            foreach (var option in Command.CreateOptions())
            {
                RootCommand.AddOption(option);
            }

            var commandHandlerTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => !type.IsAbstract && typeof(IXenialCommandHandler).IsAssignableFrom(type));

            foreach (var commandHandlerType in commandHandlerTypes.Select(type =>
            {
                var attribute = type.GetCustomAttribute<XenialCommandHandlerAttribute>();
                return (type, attribute);
            }).Where(t => t.attribute != null))
            {
                var commandType = FindCommandType(commandHandlerType);

                var subCommand = new Command(commandHandlerType.attribute.CommandName, commandHandlerType.attribute.Description);
                if (!string.IsNullOrEmpty(commandHandlerType.attribute.ShortCut))
                {
                    subCommand.AddAlias(commandHandlerType.attribute.ShortCut);
                }

                var command = (IXenialCommand)Activator.CreateInstance(commandType);

                foreach (var option in command.CreateOptions())
                {
                    subCommand.AddOption(option);
                }

                subCommand.Handler = new XenialCommandHandler(serviceProvider, command, commandHandlerType.type);

                RootCommand.Add(subCommand);
            }

            RootCommand.Handler = this;

            static Type FindCommandType((Type type, XenialCommandHandlerAttribute attribute) commandHandlerType)
            {
                if (commandHandlerType.type.BaseType.GenericTypeArguments.Length > 0)
                {
                    return commandHandlerType.type.BaseType.GenericTypeArguments.First();
                }

                if (!typeof(IXenialCommand).IsAssignableFrom(commandHandlerType.attribute.CommandType))
                {
                    throw new ArgumentException();
                }

                return commandHandlerType.attribute.CommandType;
            }
        }

        public Task<int> InvokeAsync(InvocationContext context)
        {
            var bindingContext = context.BindingContext;

            var binder = new ModelBinder(Command.GetType());
            binder.UpdateInstance(Command, bindingContext);

            if (!Command.NoLogo)
            {
                context.Console.Out.WriteLine(Consts.Header);
                context.Console.Out.WriteLine();
            }

            //TODO: invoke hello command if wizard was never ran
            new HelpResult().Apply(context);

            return Task.FromResult(-1);
        }
    }
}
