using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xenial.Licensing.Cli.Services.Default
{
    public class DefaultPathProvider : ITokenPathProvider
    {
        private readonly IUserProfileProvider userProfileProvider;

        public DefaultPathProvider(IUserProfileProvider userProfileProvider)
            => this.userProfileProvider = userProfileProvider;

        public async Task<string> GetTokenFilePath()
            => Path.Combine(await userProfileProvider.GetUserProfileDirectoryAsync(), "tokens.json");
    }
}
