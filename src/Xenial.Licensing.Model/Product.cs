using DevExpress.Xpo;

namespace Xenial.Licensing.Model
{
    [Persistent("Product")]
    public class Product : XenialLicenseBaseObjectId
    {
        private string name;
        private string description;

        /// <summary>
        /// Initializes a new instance of the <see cref="Product"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public Product(Session session) : base(session) { }

        [Persistent("Name")]
        public string Name { get => name; set => SetPropertyValue(ref name, value); }

        [Persistent("Description")]
        public string Description { get => description; set => SetPropertyValue(ref description, value); }
    }
}
