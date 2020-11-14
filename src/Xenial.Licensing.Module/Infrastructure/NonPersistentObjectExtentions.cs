using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp;

namespace Xenial.Licensing.Module.Infrastructure
{
    public static class NonPersistentObjectExtentions
    {
        public static T GetSingleton<T>(this IObjectSpace objectSpace)
            => objectSpace.FindObject<T>(null, true);

        /// <summary>
        /// Returns an <see cref="IObjectSpace"/> for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static IObjectSpace ObjectSpaceFor(this IObjectSpaceLink baseObject, Type type)
        {
            if (baseObject.ObjectSpace is NonPersistentObjectSpace nonPersistentObjectSpace)
            {
                return nonPersistentObjectSpace.AdditionalObjectSpaces.FirstOrDefault(os => os.CanInstantiate(type));
            }
            return baseObject.ObjectSpace;
        }

        /// <summary>
        /// Returns an <see cref="IObjectSpace"/> for the specified type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns></returns>
        public static IObjectSpace ObjectSpaceFor<T>(this IObjectSpaceLink baseObject) => baseObject.ObjectSpaceFor(typeof(T));
    }
}
