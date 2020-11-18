using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xenial.Licensing.Cli.Services.Default.Storage
{
    public class LicenseFileStorage : ILicenseStorage
    {
        private readonly IUserProfileProvider userProfileProvider;

        public LicenseFileStorage(IUserProfileProvider userProfileProvider)
            => this.userProfileProvider = userProfileProvider;

        public async Task<string> GetLicenseAsync()
        {
            var licenseFile = await GetLicenseFile();
            if (File.Exists(licenseFile))
            {
                return await File.ReadAllTextAsync(licenseFile);
            }
            return null;
        }

        private async Task<string> GetLicenseFile() => Path.Combine(await userProfileProvider.GetUserProfileDirectoryAsync(), "License.xml");

        public async Task StoreLicenseAsync(string license)
        {
            var licenseFile = await GetLicenseFile();
            await File.WriteAllTextAsync(licenseFile, license);
        }
    }
}
