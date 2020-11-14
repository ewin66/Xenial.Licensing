using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;

using Xenial.Licensing.Model.Infrastructure;

namespace Xenial.Licensing.Module.Controllers
{
    public class SingletonController : ViewController
    {
        private const string key = "IsSingleton";
        protected override void OnActivated()
        {
            base.OnActivated();

            if (View.Model is IModelObjectView modelObjectView && modelObjectView.ModelClass.TypeInfo.IsAttributeDefined<SingletonAttribute>(true))
            {
                var newObjectViewController = Frame.GetController<NewObjectViewController>();
                if (newObjectViewController != null)
                {
                    newObjectViewController.NewObjectAction.Active[key] = false;
                }
                var deleteObjectViewController = Frame.GetController<DeleteObjectsViewController>();
                if (deleteObjectViewController != null)
                {
                    deleteObjectViewController.DeleteAction.Active[key] = false;
                }
                var modificationsController = Frame.GetController<ModificationsController>();
                if (modificationsController != null)
                {
                    modificationsController.SaveAndNewAction.Active[key] = false;
                }
            }
        }

        protected override void OnDeactivated()
        {
            var newObjectViewController = Frame.GetController<NewObjectViewController>();
            if (newObjectViewController != null)
            {
                newObjectViewController.NewObjectAction.Active.RemoveItem(key);
            }
            var deleteObjectViewController = Frame.GetController<DeleteObjectsViewController>();
            if (deleteObjectViewController != null)
            {
                deleteObjectViewController.DeleteAction.Active.RemoveItem(key);
            }
            var modificationsController = Frame.GetController<ModificationsController>();
            if (modificationsController != null)
            {
                modificationsController.SaveAndNewAction.Active.RemoveItem(key);
            }
            base.OnDeactivated();
        }
    }
}
