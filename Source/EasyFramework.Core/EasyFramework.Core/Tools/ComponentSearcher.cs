// using System;
// using System.Collections.Generic;
// using UnityEngine;
//
// namespace EasyFramework.Core
// {
//     class ComponentSearcherIndex<T>
//     {
//         public static readonly ComponentSearcherIndex<T> Instance = new ComponentSearcherIndex<T>();
//
//         private readonly Dictionary<GameObject, T> ComponentsByGameObject = new Dictionary<GameObject, T>();
//         private (GameObject, T) _cache;
//
//         public bool IsRegistered = false;
//
//         public T Get(GameObject gameObject)
//         {
//             if (ReferenceEquals(gameObject, _cache.Item1))
//             {
//                 return _cache.Item2;
//             }
//
//             var o = GetImpl(gameObject);
//             _cache.Item1 = gameObject;
//             _cache.Item2 = o;
//             return o;
//         }
//
//         private T GetImpl(GameObject gameObject)
//         {
//             if (ComponentsByGameObject.TryGetValue(gameObject, out var component))
//             {
//                 return component;
//             }
//
//             component = gameObject.GetComponent<T>();
//             if (component == null)
//             {
//                 throw new ArgumentException($"GameObject '{gameObject.name}' does not contain component of type '{typeof(T)}'.");
//             }
//             ComponentsByGameObject[gameObject] = component;
//             return component;
//         }
//
//         public void ClearReleased()
//         {
//             // 移除已被销毁的 GameObject 条目（在 Unity 中，被销毁的对象会转换为 null）
//             var toRemove = new List<GameObject>();
//             foreach (var kv in ComponentsByGameObject)
//             {
//                 if (kv.Key == null)
//                 {
//                     toRemove.Add(kv.Key);
//                 }
//             }
//
//             foreach (var go in toRemove)
//             {
//                 ComponentsByGameObject.Remove(go);
//             }
//         }
//     }
//
//     public class ComponentSearcher : MonoSingleton<ComponentSearcher>
//     {
//         [SerializeField] private float _clearReleasedInterval;
//
//         public float ClearReleasedInterval
//         {
//             get => _clearReleasedInterval;
//             set => _clearReleasedInterval = value;
//         }
//
//         private Action _clearReleasedAction;
//
//         private float _elapsedTime;
//
//         public T Get<T>(GameObject gameObject)
//         {
//             var index = ComponentSearcherIndex<T>.Instance;
//             if (!index.IsRegistered)
//             {
//                 _clearReleasedAction += index.ClearReleased;
//                 index.IsRegistered = true;
//             }
//             return index.Get(gameObject);
//         }
//
//         void Update()
//         {
//             if (_clearReleasedAction == null)
//                 return;
//
//             _elapsedTime += Time.deltaTime;
//             if (_elapsedTime >= _clearReleasedInterval)
//             {
//                 _clearReleasedAction.Invoke();
//                 _elapsedTime = 0f;
//             }
//         }
//     }
// }
