using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Validation;

using IdentityModel;

using System.ComponentModel;
using System.Linq;

using Xenial.Licensing.Model;
using Xenial.Licensing.Module.Infrastructure;

namespace Xenial.Platform.Licensing.Module.BusinessObjects.Dialogs
{
    [DomainComponent]
    public class EnterPassPhraseTextDialog : NonPersistentBaseObject
    {
        private string name;
        private string text;
        private string passPhrase1;
        private string passPhrase2;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnterPassPhraseTextDialog"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        public EnterPassPhraseTextDialog()
        {
            PassPhrase1 = CryptoRandom.CreateUniqueId();
            PassPhrase2 = PassPhrase1;
            Text = text;
        }

        public string Text { get => text; set => SetPropertyValue(ref text, value); }

        [RuleRequiredField]
        public string Name { get => name; set => SetPropertyValue(ref name, value); }

        [RuleFromBoolProperty(UsedProperties = nameof(Name), CustomMessageTemplate = "There is already an licensing key with this name")]
        [Browsable(false)]
        public bool NameIsValid =>
            !string.IsNullOrEmpty(Name)
            && !this.ObjectSpaceFor<LicensingKey>().GetObjectsQuery<LicensingKey>().Any(k => k.Name == Name);

        [RuleRequiredField]
        public string PassPhrase1 { get => passPhrase1; set => SetPropertyValue(ref passPhrase1, value); }

        [RuleRequiredField]
        public string PassPhrase2 { get => passPhrase2; set => SetPropertyValue(ref passPhrase2, value); }

        [Browsable(false)]
        [RuleFromBoolProperty(UsedProperties = nameof(PassPhrase1) + ";" + nameof(PassPhrase2), CustomMessageTemplate = "Pass phrases do not match")]
        public bool DoPassPhrasesMatch =>
            !string.IsNullOrEmpty(PassPhrase1)
            && !string.IsNullOrEmpty(PassPhrase2)
            && PassPhrase1 == PassPhrase2;


    }
}
