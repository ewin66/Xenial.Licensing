using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Xenial.Licensing.Cli.Services.Default.Storage
{
    public class TokenFileStorage : ITokenStorage
    {
        private readonly ITokenPathProvider tokenPathProvider;

        public TokenFileStorage(ITokenPathProvider tokenPathProvider)
            => this.tokenPathProvider = tokenPathProvider;

        public async Task<string> LoadAsync()
        {
            var tokenFilePath = await tokenPathProvider.GetTokenFilePath();
            if (File.Exists(tokenFilePath))
            {
                return await File.ReadAllTextAsync(tokenFilePath);
            }
            return null;
        }

        public async Task<UserToken> LoadUserTokenAsync()
        {
            var tokenFilePath = await tokenPathProvider.GetTokenFilePath();
            if (File.Exists(tokenFilePath))
            {
                using var fileStream = File.OpenRead(tokenFilePath);
                return await JsonSerializer.DeserializeAsync<UserToken>(fileStream);
            }
            return null;
        }

        public async Task StoreAsync(string token)
        {
            var tokenFilePath = await tokenPathProvider.GetTokenFilePath();
            await File.WriteAllTextAsync(tokenFilePath, token);
        }

        public Task StoreAsync(UserToken userToken) => StoreAsync(JsonSerializer.Serialize(userToken, new JsonSerializerOptions
        {
            WriteIndented = true
        }));

        public async Task DeleteAsync()
        {
            var tokenFilePath = await tokenPathProvider.GetTokenFilePath();
            if (File.Exists(tokenFilePath))
            {
                File.Delete(tokenFilePath);
            }
        }
    }
}
