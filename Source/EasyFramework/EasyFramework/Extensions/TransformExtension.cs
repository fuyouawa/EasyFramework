using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyFramework
{
    public class TransformRecorder
    {
        public Vector3 LocalPosition;
        public Quaternion LocalRotation;
        public Vector3 LocalScale;
        public Transform Parent;
        public Transform Target;

        public TransformRecorder(Transform target)
        {
            Reset(target);
        }

        public void Reset(Transform target)
        {
            LocalPosition = target.localPosition;
            LocalRotation = target.localRotation;
            LocalScale = target.localScale;
            Parent = target.parent;
            Target = target;
        }

        public void Restore()
        {
            Target.SetParent(Parent);
            Target.localPosition = LocalPosition;
            Target.localRotation = LocalRotation;
            Target.localScale = LocalScale;
        }
    }

    public static class TransformExtension
    {
        public static bool IsParentRecursive(this Transform transform, Transform parent)
        {
            if (transform == parent)
                return false;

            var p = transform.parent;
            while (p != parent)
            {
                if (p == null)
                    return false;
                p = p.parent;
            }
            return true;
        }

        public static IEnumerable<Transform> FindParents(this Transform transform, Func<Transform, bool> condition)
        {
            var ret = new List<Transform>();

            var p = transform.parent;
            while (p != null)
            {
                if (condition(p))
                {
                    ret.Add(p);
                }
                p = p.parent;
            }

            return ret;
        }

        public static void ForEachParentRecursive(this Transform transform, Func<Transform, bool> predicate)
        {
            var p = transform.parent;
            while (p != null)
            {
                if (!predicate(p))
                    return;
                p = p.parent;
            }
        }

        public static float ScaleSquare(this Transform transform)
        {
            return transform.localScale.magnitude;
        }

        public static void SetScaleSquare(this Transform transform, float size)
        {
            transform.localScale = transform.localScale.normalized * size;
        }

        public static void SetPositionXY(this Transform transform, Vector3 position)
        {
            SetPositionXY(transform, position.ToVec2());
        }

        public static void SetPositionXY(this Transform transform, Vector2 position)
        {
            transform.position = position.ToVec3(transform.position.z);
        }
        
        
        public static string GetRelativePath(this Transform transform, Transform parent, bool includeParent = true)
        {
            if (transform == null)
                return string.Empty;
            var hierarchy = new Stack<string>();

            var p = transform;
            while (p != null && p != parent)
            {
                hierarchy.Push(p.gameObject.name);
                p = p.parent;
            }
            
            if (includeParent && parent != null)
            {
                hierarchy.Push(parent.gameObject.name);
            }

            var path = string.Join("/", hierarchy);

            if (parent == null)
                path = '/' + path;

            return path;
        }

        public static string GetAbsolutePath(this Transform transform, bool includeSceneName = true)
        {
            if (transform == null)
                return string.Empty;
            var path = GetRelativePath(transform, null);

            if (includeSceneName)
                path = '/' + transform.gameObject.scene.name + path;

            return path;
        }
    }
}
