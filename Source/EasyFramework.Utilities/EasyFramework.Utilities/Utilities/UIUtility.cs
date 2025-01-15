using UnityEngine;

namespace EasyGameFramework
{
    public static class UIUtility
    {
        public static T FindInParents<T>(GameObject go) where T : Component
        {
            if (go == null)
                return null;
			
            var comp = go.GetComponent<T>();
			
            if (comp != null)
                return comp;
			
            var t = go.transform.parent;
			
            while (t != null && comp == null)
            {
                comp = t.gameObject.GetComponent<T>();
                t = t.parent;
            }
			
            return comp;
        }
    }
}
