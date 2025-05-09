using System;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace EasyFramework.Core
{
    public static class GameObjectUtility
    {
        public static GameObject FindByRelativePath(string path, Transform parent, bool pathIncludeParent = true)
        {
            if (path.IsNullOrEmpty())
                return null;

            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            var hierarchy = path.Split('/');

            if (pathIncludeParent)
                if (hierarchy[0] != parent.gameObject.name)
                    throw new ArgumentException($"The parameter 'pathIncludeParent' is set, " +
                                                $"but the parameter 'path'({path}) is not include parent({parent.gameObject.name})!");

            if (hierarchy.Length >= (pathIncludeParent ? 2 : 1))
            {
                var p = parent;
                int seek = pathIncludeParent ? 1 : 0;

                while (seek < hierarchy.Length)
                {
                    bool found = false;
                    foreach (Transform c in p)
                    {
                        if (c.gameObject.name != hierarchy[seek])
                            continue;

                        if (seek == hierarchy.Length - 1)
                            return c.gameObject;

                        p = c;
                        seek++;
                        found = true;
                        break;
                    }

                    if (!found)
                        break;
                }
            }

            return null;
        }

        public static GameObject FindByAbsolutePath(string path, bool pathIncludeSceneName = true)
        {
            if (path.IsNullOrEmpty())
                return null;

            if (!pathIncludeSceneName)
            {
                return GameObject.Find(path);
            }

            if (path[0] == '/')
                path = path.Substring(1, path.Length - 1);
            var hierarchy = path.Split('/');

            GameObject target = null;
            if (hierarchy.Length >= 2)
            {
                var scene = SceneManager.GetSceneByName(hierarchy[0]);
                if (scene.IsValid() && scene.isLoaded)
                {
                    foreach (var o in scene.GetRootGameObjects())
                    {
                        if (o.name != hierarchy[1])
                            continue;
                        if (hierarchy.Length == 2)
                            return o;
                        
                        int len = path.IndexOf('/') + 1;
                        target = FindByRelativePath(path.Substring(len, path.Length - len), o.transform)?.gameObject;
                        if (target != null)
                            break;
                    }
                }
            }

            return target;
        }
        
    }
}
