using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xenial.Platform.Licensing.Module.BusinessObjects.Dialogs;

namespace Xenial.Licensing.Module.BusinessObjects
{
    public static class ModelTypeList
    {
        internal static readonly Type[] ModelTypes = new[]
        {
            typeof(EnterPassPhraseTextDialog)
        };

        public static IEnumerable<Type> UseLicensingViewModels(this IEnumerable<Type> types)
            => types.Concat(ModelTypes);
    }
}
