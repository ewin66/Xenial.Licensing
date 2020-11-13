using System.Threading;
using System.Threading.Tasks;

namespace Xenial.Licensing.Cli.Services
{
    public interface IApiClientConfiguration
    {
        string BaseUrl { get; }

        Task<string> GetAccessTokenAsync(CancellationToken cancellationToken);
    }
}
