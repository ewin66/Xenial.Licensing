using System;
using System.Threading.Tasks;

namespace Xenial.Licensing.Cli.Services.Default.Storage
{
    public class LicenseEnvironmentStorage : ILicenseStorage
    {
        private const string keyName = "XENIAL_LICENSE";

        public Task<string> FetchAsync()
            => Task.FromResult(Environment.GetEnvironmentVariable(keyName));

        public Task StoreAsync(string license)
        {
            Environment.SetEnvironmentVariable(keyName, license, EnvironmentVariableTarget.User);
            return Task.CompletedTask;
        }

        public Task DeleteAsync()
        {
            Environment.SetEnvironmentVariable(keyName, null, EnvironmentVariableTarget.User);
            return Task.CompletedTask;
        }
    }
}
