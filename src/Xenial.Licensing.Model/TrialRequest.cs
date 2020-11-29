using System;

using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace Xenial.Licensing.Model
{
    [Persistent("TrialRequest")]
    [DefaultClassOptions]
    public class TrialRequest : XenialLicenseBaseObjectId
    {
        private string machineKey;
        private string userId;
        private DateTime requestDate;
        private string userCompany;
        private string userName;
        private string userEmail;

        public TrialRequest(Session session) : base(session) { }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            RequestDate = DateTime.Now;
        }

        [Persistent("MachineKey")]
        [Size(100)]
        [Indexed]
        public string MachineKey { get => machineKey; set => SetPropertyValue(ref machineKey, value); }

        [Persistent("UserId")]
        [Size(100)]
        [Indexed]
        public string UserId { get => userId; set => SetPropertyValue(ref userId, value); }

        [ValueConverter(typeof(UtcDateTimeConverter))]
        [Persistent("RequestDate")]
        public DateTime RequestDate { get => requestDate; set => SetPropertyValue(ref requestDate, value); }

        [Persistent("UserEmail")]
        [Size(254)]
        public string UserEmail { get => userEmail; set => SetPropertyValue(ref userEmail, value); }

        [Persistent("UserName")]
        [Size(254)]
        public string UserName { get => userName; set => SetPropertyValue(ref userName, value); }

        [Persistent("UserCompany")]
        [Size(254)]
        public string UserCompany { get => userCompany; set => SetPropertyValue(ref userCompany, value); }
    }
}
