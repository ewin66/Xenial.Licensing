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
    }
}
