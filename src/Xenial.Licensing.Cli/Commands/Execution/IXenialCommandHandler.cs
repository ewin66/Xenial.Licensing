using System.Threading.Tasks;

namespace Xenial.Licensing.Cli.Commands
{
    public interface IXenialCommandHandler
    {
        Task<int> ExecuteCommand(IXenialCommand arguments);
    }
}
