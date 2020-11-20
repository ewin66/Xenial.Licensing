using System;

namespace Xenial.Licensing.Cli.Commands
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class XenialCommandHandlerAttribute : Attribute
    {
        public XenialCommandHandlerAttribute(Type commandType, string commandName)
        {
            CommandType = commandType;
            CommandName = commandName;
        }

        public Type CommandType { get; }
        public string CommandName { get; }
        public string Description { get; }
        public string ShortCut { get; }
    }
}
