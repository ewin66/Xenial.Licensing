using System.Threading.Tasks;

using DevExpress.Xpo;

namespace Xenial.Licensing.Model.Infrastructure
{
    public static class XpoExtentions
    {
        public static Task<T> GetSingletonAsync<T>(this UnitOfWork unitOfWork)
            => unitOfWork.FindObjectAsync<T>(null);
    }
}
