using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xenial.Licensing.Cli.Services.Default
{
    public class DefaultUserProfileProvider : IUserProfileProvider
    {
        public Task<string> GetUserProfileDirectoryAsync()
        {
            var profileDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".xenial");

            if (!Directory.Exists(profileDirectory))
            {
                Directory.CreateDirectory(profileDirectory);
            }

            return Task.FromResult(profileDirectory);
        }
    }
}
