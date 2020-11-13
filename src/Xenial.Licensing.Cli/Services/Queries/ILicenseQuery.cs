using System.Threading.Tasks;

namespace Xenial.Licensing.Cli.Services.Queries
{
    public interface ILicenseQuery
    {
        Task<bool> HasActiveLicense();
    }
}
