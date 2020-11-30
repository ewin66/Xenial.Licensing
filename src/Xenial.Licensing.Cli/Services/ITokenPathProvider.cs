using System.Threading.Tasks;

namespace Xenial.Licensing.Cli.Services
{
    public interface ITokenPathProvider
    {
        Task<string> GetTokenFilePath();
    }
}
