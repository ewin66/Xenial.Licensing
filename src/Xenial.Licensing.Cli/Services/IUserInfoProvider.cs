using System.Threading.Tasks;

namespace Xenial.Licensing.Cli.Services
{
    public record UserInfo(string UserId, string UserName, string Email);

    public interface IUserInfoProvider
    {
        Task<UserInfo> GetUserInfoAsync();
    }
}
