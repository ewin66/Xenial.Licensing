using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeviceId;

namespace Xenial.Licensing.Cli.Services.Default
{
    public class DefaultDeviceIdProvider : IDeviceIdProvider
    {
        private readonly IUserProfileProvider userProfileProvider;

        public DefaultDeviceIdProvider(IUserProfileProvider userProfileProvider)
            => this.userProfileProvider = userProfileProvider;

        public async Task<string> GetDeviceIdAsync()
        {
            var tokenFile = Path.Combine(await userProfileProvider.GetUserProfileDirectoryAsync(), ".devicetoken");
            if (!File.Exists(tokenFile))
            {
                await File.WriteAllTextAsync(tokenFile, Guid.NewGuid().ToString());
            }

            var deviceIdBuilder = new DeviceIdBuilder()
                .AddMachineName()
                .AddUserName()
                .AddOSVersion()
                .AddSystemDriveSerialNumber()
                .AddOSInstallationID()
                .AddFileToken(tokenFile);

            var deviceId = deviceIdBuilder.ToString();
            return deviceId;
        }
    }
}
