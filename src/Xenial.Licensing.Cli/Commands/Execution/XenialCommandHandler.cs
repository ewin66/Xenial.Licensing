using System;
using System.CommandLine.Binding;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

namespace Xenial.Licensing.Cli.Commands
{
    public abstract class XenialCommandHandler<T> : IXenialCommandHandler
        where T : IXenialCommand
    {
        public Task<int> ExecuteCommand(IXenialCommand arguments)
            => ExecuteCommand((T)arguments);
        protected abstract Task<int> ExecuteCommand(T arguments);
    }

    public class XenialCommandHandler : ICommandHandler
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IXenialCommand command;
        private readonly Type commandHandlerType;
        public XenialCommandHandler(IServiceProvider serviceProvider, IXenialCommand command, Type commandHandlerType)
        {
            this.serviceProvider = serviceProvider;
            this.command = command;
            this.commandHandlerType = commandHandlerType;
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var bindingContext = context.BindingContext;

            var binder = new ModelBinder(command.GetType());
            binder.UpdateInstance(command, bindingContext);

            var commandHandler = (IXenialCommandHandler)serviceProvider.GetRequiredService(commandHandlerType);
            var result = await commandHandler.ExecuteCommand(command);

            return result;
        }
    }
}
