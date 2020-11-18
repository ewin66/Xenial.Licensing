using System.Threading.Tasks;

namespace Xenial.Licensing.Cli.Services
{
    public interface ILicensePublicKeyStorage
    {
        Task<string> FetchAsync(string keyName);
        Task StoreAsync(string keyName, string publicKey);
    }
}
