using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Xenial.Licensing.Cli.Services.Default.Storage
{
    public class LicensePublicKeyEnvironmentStorage : ILicensePublicKeyStorage
    {
        private const string keyName = "XENIAL_LICENSE_PUBLIC_KEYS";

        public Task<string> FetchAsync(string keyName)
        {
            var keys = GetKeys();

            return Task.FromResult(keys.FirstOrDefault(r => r.Key == keyName).Value);
        }

        private static Dictionary<string, string> GetKeys()
        {
            var keys = Environment.GetEnvironmentVariable(keyName);
            if (!string.IsNullOrEmpty(keys))
            {
                var result = JsonSerializer.Deserialize<Dictionary<string, string>>(keys);
                return result;
            }
            return new Dictionary<string, string>();
        }

        public Task StoreAsync(string name, string publicKey)
        {
            var keys = GetKeys();
            keys[name] = publicKey;
            Environment.SetEnvironmentVariable(keyName, JsonSerializer.Serialize(keys), EnvironmentVariableTarget.User);
            return Task.CompletedTask;
        }
    }
}
