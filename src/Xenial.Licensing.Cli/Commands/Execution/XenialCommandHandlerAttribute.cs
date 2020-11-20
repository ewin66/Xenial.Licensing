using System;

namespace Xenial.Licensing.Cli.Commands
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class XenialCommandHandlerAttribute : Attribute
    {
        public XenialCommandHandlerAttribute(string commandName)
            => CommandName = commandName;

        public Type CommandType { get; set; }
        public string CommandName { get; }
        public string Description { get; set; }
        public string ShortCut { get; set; }
    }
}
