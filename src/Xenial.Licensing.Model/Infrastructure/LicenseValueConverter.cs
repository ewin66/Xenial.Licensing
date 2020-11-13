using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.Xpo.Metadata;

namespace Xenial.Licensing.Model.Infrastructure
{
    public class LicenseValueConverter : ValueConverter
    {
        public override Type StorageType => typeof(string);

        public override object ConvertFromStorageType(object value)
        {
            if (value is string str && str.Length > 0)
            {
                return Standard.Licensing.License.Load(str);
            }
            return null;
        }
        public override object ConvertToStorageType(object value)
        {
            if (value is Standard.Licensing.License lic)
            {
                return lic.ToString();
            }
            return null;
        }
    }
}
