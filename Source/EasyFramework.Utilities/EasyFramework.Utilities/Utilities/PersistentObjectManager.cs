// using System;
// using System.Collections.Generic;
//
// namespace EasyGameFramework
// {
//     public static class PersistentObjectManager
//     {
//         private static List<UnityEngine.Object> toCleanUpUnityObjects;
//
//         private static List<WeakReference> toCleanUpDisposables;
//
//         static PersistentObjectManager()
//         {
//             toCleanUpUnityObjects = new List<UnityEngine.Object>();
//             toCleanUpDisposables = new List<WeakReference>();
//             Type.GetType("UnityEditor.AssemblyReloadEvents, UnityEditor").AddEvent("beforeAssemblyReload", null, new Action(CleanUp));
//         }
//
//         public static void DestroyObjectOnAssemblyReload(UnityEngine.Object unityObj)
//         {
//             if (!(unityObj == null))
//             {
//                 toCleanUpUnityObjects.Add(unityObj);
//             }
//         }
//
//         public static void DisposeObjectOnAssemblyReload(IDisposable disposable)
//         {
//             if (disposable != null)
//             {
//                 toCleanUpDisposables.Add(new WeakReference(disposable));
//             }
//         }
//
//         private static void CleanUp()
//         {
//             foreach (UnityEngine.Object unityObj in toCleanUpUnityObjects)
//             {
//                 try
//                 {
//                     if (unityObj != null)
//                     {
//                         UnityEngine.Object.DestroyImmediate(unityObj);
//                     }
//                 }
//                 catch
//                 {
//                     // ignored
//                 }
//             }
//             foreach (WeakReference reference in toCleanUpDisposables)
//             {
//                 try
//                 {
//                     if (reference.Target is IDisposable disposable)
//                     {
//                         disposable.Dispose();
//                     }
//                 }
//                 catch
//                 {
//                     // ignored
//                 }
//             }
//             toCleanUpUnityObjects.Clear();
//             toCleanUpDisposables.Clear();
//         }
//     }
// }