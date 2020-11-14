using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;

using Xenial.Licensing.Model.Infrastructure;

namespace Xenial.Licensing.Module.Infrastructure
{
    public class SingletonNavigationItemNodesUpdater : ModelNodesGeneratorUpdater<NavigationItemNodeGenerator>
    {
        public override void UpdateNode(ModelNode node)
        {
            if (node is IModelRootNavigationItems rootNavigationItems)
            {
                foreach (var item in rootNavigationItems.Items)
                {
                    UpdateNode(item);
                }
            }

            static void UpdateNode(IModelNavigationItem item)
            {
                if (item.View is IModelObjectView modelObjectView && item.View is IModelListView)
                {
                    if (modelObjectView.ModelClass.TypeInfo.IsAttributeDefined<SingletonAttribute>(true))
                    {
                        item.View = modelObjectView.ModelClass.DefaultDetailView;
                    }
                }
                foreach (var nestedNode in item.Items)
                {
                    UpdateNode(nestedNode);
                }
            }
        }
    }
}
