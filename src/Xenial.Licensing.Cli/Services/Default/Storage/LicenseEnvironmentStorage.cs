using System;
using System.Threading.Tasks;

namespace Xenial.Licensing.Cli.Services.Default.Storage
{
    public class LicenseEnvironmentStorage : ILicenseStorage
    {
        public Task<string> FetchAsync()
            => Task.FromResult(Environment.GetEnvironmentVariable("XENIAL_LICENSE"));

        public Task StoreAsync(string license)
        {
            Environment.SetEnvironmentVariable("XENIAL_LICENSE", license, EnvironmentVariableTarget.User);
            return Task.CompletedTask;
        }
    }
}
