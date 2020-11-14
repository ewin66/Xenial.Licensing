using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;

using Xenial.Licensing.Model;
using Xenial.Platform.Licensing.Module.BusinessObjects.Dialogs;

namespace Xenial.Licensing.Module.Controllers
{
    public class LicensingKeyNewObjectViewController : ObjectViewController<ObjectView, LicensingKey>
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            Frame.GetController<NewObjectViewController>().ObjectCreating -= LicensingKeyNewObjectViewController_ObjectCreating;
            Frame.GetController<NewObjectViewController>().ObjectCreating += LicensingKeyNewObjectViewController_ObjectCreating;
        }

        protected override void OnDeactivated()
        {
            Frame.GetController<NewObjectViewController>().ObjectCreating -= LicensingKeyNewObjectViewController_ObjectCreating;
            base.OnDeactivated();
        }

        private void LicensingKeyNewObjectViewController_ObjectCreating(object sender, ObjectCreatingEventArgs e)
        {
            if (e.ObjectType == typeof(LicensingKey))
            {
                var os = Application.CreateObjectSpace(typeof(EnterPassPhraseTextDialog));
                var obj = os.CreateObject<EnterPassPhraseTextDialog>();
                var dv = Application.CreateDetailView(os, obj);

                Application.ShowViewStrategy.ShowViewInPopupWindow(dv, () =>
                {
                    var os = Application.CreateObjectSpace(typeof(LicensingKey));
                    var key = os.CreateObject<LicensingKey>();

                    key.Name = obj.Name;
                    key.PassPhrase = obj.PassPhrase1;
                    var keyGenerator = Standard.Licensing.Security.Cryptography.KeyGenerator.Create();
                    var keyPair = keyGenerator.GenerateKeyPair();
                    key.PrivateKey = keyPair.ToEncryptedPrivateKeyString(key.PassPhrase);
                    key.PublicKey = keyPair.ToPublicKeyString();

                    var dv = Application.CreateDetailView(os, key);
                    Application.ShowViewStrategy.ShowView(
                        new ShowViewParameters(dv),
                        new ShowViewSource(Frame, Frame.GetController<NewObjectViewController>().NewObjectAction)
                    );
                });
                e.Cancel = true;
            }
        }
    }
}
