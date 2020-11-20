using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Xenial.Licensing.Cli.Services.Default.Storage
{
    public class LicensePublicKeyFileStorage : ILicensePublicKeyStorage
    {
        private readonly IUserProfileProvider userProfileProvider;

        public LicensePublicKeyFileStorage(IUserProfileProvider userProfileProvider)
            => this.userProfileProvider = userProfileProvider;

        public async Task<string> FetchAsync(string keyName)
        {
            var keys = await GetKeys();

            return keys.FirstOrDefault(r => r.Key == keyName).Value;
        }

        private async Task<Dictionary<string, string>> GetKeys()
        {
            var path = await GetLicensePublicKeyFile();
            if (File.Exists(path))
            {
                var content = await File.ReadAllTextAsync(path);
                var result = JsonSerializer.Deserialize<Dictionary<string, string>>(content);
                return result;
            }
            return new Dictionary<string, string>();
        }

        public async Task StoreAsync(string keyName, string publicKey)
        {
            var path = await GetLicensePublicKeyFile();
            var keys = await GetKeys();
            keys[keyName] = publicKey;
            await File.WriteAllTextAsync(path, JsonSerializer.Serialize(keys));
        }

        private async Task<string> GetLicensePublicKeyFile() => Path.Combine(await userProfileProvider.GetUserProfileDirectoryAsync(), "License.PublicKeys.json");
    }
}
