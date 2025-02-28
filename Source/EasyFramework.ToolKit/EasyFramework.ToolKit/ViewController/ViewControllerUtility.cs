using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    public static class ViewControllerUtility
    {
        public static List<IViewBinder> GetOtherBinders(IViewController controller)
        {
            return controller.Config.EditorConfig.OtherBindersList
                .SelectMany(b => b.Targets.Collection)
                .Where(c => c != null && c.GetComponent<IViewBinder>() != null)
                .Select(c => c.GetComponent<IViewBinder>())
                .ToList();
        }

        public static List<IViewBinder> GetAllBinders(IViewController controller)
        {
            var total = new List<IViewBinder>();

            total.AddRange(GetChildrenBinders(controller));
            total.AddRange(GetOtherBinders(controller));

            return total;
        }

        public static List<IViewBinder> GetChildrenBinders(IViewController controller)
        {
            var comp = (Component)controller;
            return comp.transform.GetComponentsInChildren<IViewBinder>(true)
                .Where(b => b.Config.OwnerController == controller)
                .ToList();
        }

    }
}
