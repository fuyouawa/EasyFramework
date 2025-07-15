using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;
using UnityEditor;

//TODO UnityEditorEventUtility待优化
namespace EasyToolKit.Core.Editor
{
    /// <summary>
    /// This class contains utility methods for subscribing to various UnityEditor events reliably and safely across all Odin-supported versions of Unity.
    /// </summary>
    [InitializeOnLoad]
    public static class UnityEditorEventUtility
    {
        private static readonly MemberInfo EditorApplication_delayCall_Member =
            ((IEnumerable<MemberInfo>)typeof(EditorApplication).GetMember("delayCall",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)).FirstOrDefault<MemberInfo>();

        private static readonly EventInfo SceneView_duringSceneGui_Event = typeof(SceneView).GetEvent("duringSceneGui",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        private static readonly FieldInfo SceneView_onSceneGUIDelegate_Field =
            typeof(SceneView).GetField("onSceneGUIDelegate",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        private static readonly System.Type SceneView_OnSceneFunc_Type = typeof(SceneView).GetNestedType("OnSceneFunc");
        private static readonly object actionsToSubscribe_LOCK = new object();
        private static readonly List<Action> actionsToSubscribe = new List<Action>();
        private static readonly EventInfo onProjectChangedEvent = typeof(EditorApplication).GetEvent("projectChanged");

        public static readonly bool HasOnProjectChanged =
            UnityEditorEventUtility.onProjectChangedEvent != (EventInfo)null;

        static UnityEditorEventUtility()
        {
            EditorApplication.update += new EditorApplication.CallbackFunction(UnityEditorEventUtility.OnUpdate);
        }

        public static event Action OnProjectChanged
        {
            add
            {
                if (!(UnityEditorEventUtility.onProjectChangedEvent != (EventInfo)null))
                    throw new NotImplementedException(
                        "EditorApplication.projectChanged is not implemented in this version of Unity.");
                UnityEditorEventUtility.onProjectChangedEvent.AddEventHandler((object)null, (Delegate)value);
            }
            remove
            {
                if (!(UnityEditorEventUtility.onProjectChangedEvent != (EventInfo)null))
                    throw new NotImplementedException(
                        "EditorApplication.projectChanged is not implemented in this version of Unity.");
                UnityEditorEventUtility.onProjectChangedEvent.RemoveEventHandler((object)null, (Delegate)value);
            }
        }

        /// <summary>
        /// Sometimes, someone accidentally overrides a delay action subscription to <see cref="F:UnityEditor.EditorApplication.delayCall" />
        /// by setting the value instead of using the += operator as should be done,
        /// which can be done because in many versions of Unity it is a field, and not an event.
        /// (In some versions of Unity it is an event, though, and in this case, this method acts as a wrapper
        /// to subscribe reliably, no matter the nature of the backing event.)
        /// This method subscribes to a lot of different callbacks, in the hopes of catching at least one.
        /// <para />
        /// As opposed to <see cref="M:Sirenix.OdinInspector.Editor.UnityEditorEventUtility.DelayAction(System.Action)" />, this method is safe to call from any thread, and will
        /// delay the actual subscription to a safe time.
        /// </summary>
        public static void DelayActionThreadSafe(Action action)
        {
            lock (UnityEditorEventUtility.actionsToSubscribe_LOCK)
                UnityEditorEventUtility.actionsToSubscribe.Add(action);
        }

        /// <summary>
        /// Sometimes, an idiot overrides a delay action subscription to <see cref="F:UnityEditor.EditorApplication.delayCall" />,
        /// which can be done because the people at Unity didn't know what events were once upon a time.
        /// This method subscribes to a lot of different callbacks, in the hopes of catching at least one.
        /// </summary>
        public static void DelayAction(Action action)
        {
            UnityEditorEventUtility.DelayAction(action, false);
        }

        /// <summary>
        /// Sometimes, an idiot overrides a delay action subscription to <see cref="F:UnityEditor.EditorApplication.delayCall" />,
        /// which can be done because the people at Unity didn't know what events were once upon a time.
        /// This method subscribes to a lot of different callbacks, in the hopes of catching at least one.
        /// </summary>
        public static void DelayAction(Action action, bool excludeGuiEventHooks)
        {
            bool executed = false;
            Action execute = (Action)null;
            EditorApplication.ProjectWindowItemCallback projectWindowItemOnGUI =
                (EditorApplication.ProjectWindowItemCallback)null;
            EditorApplication.HierarchyWindowItemCallback hierarchyWindowItemOnGUI =
                (EditorApplication.HierarchyWindowItemCallback)null;
            if (!excludeGuiEventHooks)
            {
                projectWindowItemOnGUI = (EditorApplication.ProjectWindowItemCallback)((_, __) => execute());
                hierarchyWindowItemOnGUI = (EditorApplication.HierarchyWindowItemCallback)((_, __) => execute());
            }

            EditorApplication.CallbackFunction update = (EditorApplication.CallbackFunction)(() => execute());
            Action delayCall = (Action)(() => execute());
            execute = (Action)(() =>
            {
                if (executed)
                    return;
                executed = true;
                try
                {
                    action();
                }
                finally
                {
                    if (!excludeGuiEventHooks)
                    {
                        EditorApplication.projectWindowItemOnGUI -= projectWindowItemOnGUI;
                        EditorApplication.hierarchyWindowItemOnGUI -= hierarchyWindowItemOnGUI;
                    }

                    EditorApplication.update -= update;
                    UnityEditorEventUtility.EditorApplication_delayCall -= delayCall;
                }
            });
            if (!excludeGuiEventHooks)
            {
                EditorApplication.projectWindowItemOnGUI += projectWindowItemOnGUI;
                EditorApplication.hierarchyWindowItemOnGUI += hierarchyWindowItemOnGUI;
            }

            EditorApplication.update += update;
            UnityEditorEventUtility.EditorApplication_delayCall += delayCall;
        }

        private static void OnUpdate()
        {
            lock (UnityEditorEventUtility.actionsToSubscribe_LOCK)
            {
                if (UnityEditorEventUtility.actionsToSubscribe.Count <= 0)
                    return;
                for (int index = 0; index < UnityEditorEventUtility.actionsToSubscribe.Count; ++index)
                    UnityEditorEventUtility.DelayAction(UnityEditorEventUtility.actionsToSubscribe[index]);
                UnityEditorEventUtility.actionsToSubscribe.Clear();
            }
        }

        /// <summary>
        /// In 2019.1+, this event subscribes to SceneView.duringSceneGui. In 2018.4 and lower, it subscribes to SceneView.onSceneGUIDelegate.
        /// </summary>
        public static event Action<SceneView> DuringSceneGUI
        {
            add
            {
                if (UnityEditorEventUtility.SceneView_duringSceneGui_Event == (EventInfo)null &&
                    (UnityEditorEventUtility.SceneView_onSceneGUIDelegate_Field == (FieldInfo)null ||
                     UnityEditorEventUtility.SceneView_OnSceneFunc_Type == (System.Type)null))
                    throw new InvalidOperationException(
                        "SceneView.duringSceneGui event and SceneView.onSceneGUIDelegate field could both not be found in the SceneView class, and parts of Odin will be broken. Please report this issue to the issue tracker.");
                if (UnityEditorEventUtility.SceneView_duringSceneGui_Event != (EventInfo)null)
                {
                    UnityEditorEventUtility.SceneView_duringSceneGui_Event.AddMethod.Invoke((object)null, new object[1]
                    {
                        (object)value
                    });
                }
                else
                {
                    Delegate @delegate =
                        Delegate.Combine(
                            UnityEditorEventUtility.SceneView_onSceneGUIDelegate_Field.GetValue((object)null) as
                                Delegate, value.ConvertDelegate(UnityEditorEventUtility.SceneView_OnSceneFunc_Type));
                    UnityEditorEventUtility.SceneView_onSceneGUIDelegate_Field.SetValue((object)null,
                        (object)@delegate);
                }
            }
            remove
            {
                if (UnityEditorEventUtility.SceneView_duringSceneGui_Event == (EventInfo)null &&
                    (UnityEditorEventUtility.SceneView_onSceneGUIDelegate_Field == (FieldInfo)null ||
                     UnityEditorEventUtility.SceneView_OnSceneFunc_Type == (System.Type)null))
                    throw new InvalidOperationException(
                        "SceneView.duringSceneGui event and SceneView.onSceneGUIDelegate field could both not be found in the SceneView class, and parts of Odin will be broken. Please report this issue to the issue tracker.");
                if (UnityEditorEventUtility.SceneView_duringSceneGui_Event != (EventInfo)null)
                {
                    UnityEditorEventUtility.SceneView_duringSceneGui_Event.RemoveMethod.Invoke((object)null,
                        new object[1]
                        {
                            (object)value
                        });
                }
                else
                {
                    Delegate @delegate =
                        Delegate.Remove(
                            UnityEditorEventUtility.SceneView_onSceneGUIDelegate_Field.GetValue((object)null) as
                                Delegate, value.ConvertDelegate(UnityEditorEventUtility.SceneView_OnSceneFunc_Type));
                    UnityEditorEventUtility.SceneView_onSceneGUIDelegate_Field.SetValue((object)null,
                        (object)@delegate);
                }
            }
        }

        /// <summary>
        /// In 2020.1, Unity changed EditorApplication.delayCall from a field to an event, meaning
        /// we now have to use reflection to access it consistently across all versions of Unity.
        /// </summary>
        public static event Action EditorApplication_delayCall
        {
            add
            {
                if (UnityEditorEventUtility.EditorApplication_delayCall_Member == (MemberInfo)null)
                    throw new InvalidOperationException(
                        "EditorApplication.delayCall field or event could not be found, and parts of Odin will be broken. Please report this issue to the issue tracker.");
                if ((object)(UnityEditorEventUtility.EditorApplication_delayCall_Member as FieldInfo) != null)
                {
                    EditorApplication.CallbackFunction callbackFunction =
                        (EditorApplication.CallbackFunction)Delegate.Combine(
                            (Delegate)(UnityEditorEventUtility.EditorApplication_delayCall_Member as FieldInfo)
                            .GetValue((object)null),
                            (Delegate)value.ConvertDelegate<EditorApplication.CallbackFunction>());
                    (UnityEditorEventUtility.EditorApplication_delayCall_Member as FieldInfo).SetValue((object)null,
                        (object)callbackFunction);
                }
                else if ((object)(UnityEditorEventUtility.EditorApplication_delayCall_Member as EventInfo) != null)
                    (UnityEditorEventUtility.EditorApplication_delayCall_Member as EventInfo).AddEventHandler(
                        (object)null, (Delegate)value);
                else if (UnityEditorEventUtility.EditorApplication_delayCall_Member == (MemberInfo)null)
                    throw new InvalidOperationException(
                        "EditorApplication.delayCall was not a field or an event. Odin will be broken.");
            }
            remove
            {
                if (UnityEditorEventUtility.EditorApplication_delayCall_Member == (MemberInfo)null)
                    throw new InvalidOperationException(
                        "EditorApplication.delayCall field or event could not be found. Odin will be broken.");
                if ((object)(UnityEditorEventUtility.EditorApplication_delayCall_Member as FieldInfo) != null)
                {
                    EditorApplication.CallbackFunction callbackFunction =
                        (EditorApplication.CallbackFunction)Delegate.Remove(
                            (Delegate)(UnityEditorEventUtility.EditorApplication_delayCall_Member as FieldInfo)
                            .GetValue((object)null),
                            (Delegate)value.ConvertDelegate<EditorApplication.CallbackFunction>());
                    (UnityEditorEventUtility.EditorApplication_delayCall_Member as FieldInfo).SetValue((object)null,
                        (object)callbackFunction);
                }
                else if ((object)(UnityEditorEventUtility.EditorApplication_delayCall_Member as EventInfo) != null)
                    (UnityEditorEventUtility.EditorApplication_delayCall_Member as EventInfo).RemoveEventHandler(
                        (object)null, (Delegate)value);
                else if (UnityEditorEventUtility.EditorApplication_delayCall_Member == (MemberInfo)null)
                    throw new InvalidOperationException(
                        "EditorApplication.delayCall was not a field or an event. Odin will be broken.");
            }
        }

        private static Delegate ConvertDelegate(this Delegate src, System.Type delegateType)
        {
            if ((object)src == null || src.GetType() == delegateType)
                return src;
            Delegate[] invocationList = src.GetInvocationList();
            if (invocationList.Length == 1)
                return Delegate.CreateDelegate(delegateType, src.Target, src.Method);
            for (int index = 0; index < invocationList.Length; ++index)
                invocationList[index] = invocationList[index].ConvertDelegate(delegateType);
            return Delegate.Combine(invocationList);
        }

        private static T ConvertDelegate<T>(this Delegate src)
        {
            System.Type type = typeof(T);
            if ((object)src == null || src.GetType() == type)
                return (T)(object)src;
            Delegate[] invocationList = src.GetInvocationList();
            if (invocationList.Length == 1)
                return (T)(object)Delegate.CreateDelegate(type, src.Target, src.Method);
            for (int index = 0; index < invocationList.Length; ++index)
                invocationList[index] = invocationList[index].ConvertDelegate(type);
            return (T)(object)Delegate.Combine(invocationList);
        }
    }
}
