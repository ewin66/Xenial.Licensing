using System;

using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace Xenial.Licensing.Model
{
    [Persistent("Company")]
    [DefaultClassOptions]
    public class Company : XenialLicenseBaseObjectId
    {
        private string name;
        private string companyGlobalId;

        /// <summary>
        /// Initializes a new instance of the <see cref="Company"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public Company(Session session) : base(session) { }

        /// <summary>
        /// Invoked when the current object is about to be initialized after its creation.
        /// </summary>
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            CompanyGlobalId = Guid.NewGuid().ToString();
        }

        [Persistent("Name")]
        public string Name { get => name; set => SetPropertyValue(ref name, value); }

        [Persistent("CompanyGlobalId")]
        public string CompanyGlobalId { get => companyGlobalId; set => SetPropertyValue(ref companyGlobalId, value); }

        [Association("Company-User"), Aggregated]
        public XPCollection<CompanyUser> Users => GetCollection<CompanyUser>();
    }

    [Persistent("CompanyUser")]
    public class CompanyUser : XenialLicenseBaseObjectId
    {
        private string name;
        private string email;
        private Company company;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyUser"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public CompanyUser(Session session) : base(session) { }

        [Persistent("Name")]
        public string Name { get => name; set => SetPropertyValue(ref name, value); }

        [Persistent("Email")]
        public string Email { get => email; set => SetPropertyValue(ref email, value); }

        [Persistent("CompanyId")]
        [Association("Company-User")]
        public Company Company { get => company; set => SetPropertyValue(ref company, value); }
    }
}
