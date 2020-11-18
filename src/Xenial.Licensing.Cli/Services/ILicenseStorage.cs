using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xenial.Licensing.Cli.Services
{
    public interface ILicenseStorage
    {
        Task<string> GetLicenseAsync();
        Task StoreLicenseAsync(string license);
    }
}
