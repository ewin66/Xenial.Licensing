using System;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace Xenial.Licensing.Cli.Commands
{
    public static class ServiceCollectionExtentions
    {
        public static IServiceCollection AddXenialCommands(this IServiceCollection serviceCollection)
        {
            var commandHandlerTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => !type.IsAbstract && typeof(IXenialCommandHandler).IsAssignableFrom(type));

            foreach (var commandHandlerType in commandHandlerTypes.Select(type =>
            {
                var attribute = type.GetCustomAttribute<XenialCommandHandlerAttribute>();
                return (type, attribute);
            }).Where(t => t.attribute != null))
            {
                serviceCollection.AddSingleton(commandHandlerType.type);
            }
            return serviceCollection;
        }

    }
}
