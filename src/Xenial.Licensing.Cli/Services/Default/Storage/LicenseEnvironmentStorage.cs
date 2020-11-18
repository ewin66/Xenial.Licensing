using System;
using System.Threading.Tasks;

namespace Xenial.Licensing.Cli.Services.Default.Storage
{
    public class LicenseEnvironmentStorage : ILicenseStorage
    {
        public Task<string> GetLicenseAsync() => Task.FromResult(Environment.GetEnvironmentVariable("XENIAL_LICENSE"));
        public Task StoreLicenseAsync(string license)
        {
            Environment.SetEnvironmentVariable("XENIAL_LICENSE", license, EnvironmentVariableTarget.User);
            return Task.CompletedTask;
        }
    }
}
