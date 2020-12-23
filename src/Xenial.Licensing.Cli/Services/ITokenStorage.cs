using System.Threading.Tasks;

namespace Xenial.Licensing.Cli.Services
{
    public interface ITokenStorage
    {
        Task<string> LoadAsync();
        Task<UserToken> LoadUserTokenAsync();
        Task StoreAsync(string userToken);
        Task StoreAsync(UserToken userToken);
        Task DeleteAsync();
    }
}
