using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace EasyToolKit.UIKit
{
    public static class IUIManagerExtension
    {
        private static readonly Dictionary<string, Type> _panelTypeCache = new Dictionary<string, Type>();
        
        public static UniTask CreatePanelAsync<T>(this IUIManager manager, GameObject panelPrefab, UILevel level = UILevel.Common) where T : Component, IPanel
        {
            return manager.CreatePanelAsync(typeof(T), panelPrefab, level);
        }
        
        public static UniTask CreatePanelAsync(this IUIManager manager, string panelTypeName, GameObject panelPrefab, UILevel level = UILevel.Common)
        {
            return manager.CreatePanelAsync(FindPanelType(panelTypeName), panelPrefab, level);
        }

        public static bool HasPanel<T>(this IUIManager manager) where T : Component, IPanel
        {
            return manager.HasPanel(typeof(T));
        }
        
        public static bool HasPanel(this IUIManager manager, string panelTypeName)
        {
            return manager.HasPanel(FindPanelType(panelTypeName));
        }

        public static async UniTask<T> OpenOrCreatePanelAsync<T>(this IUIManager manager, GameObject panelPrefab, IPanelData panelData = null, UILevel level = UILevel.Common) where T : Component, IPanel
        {
            return (T)await manager.OpenOrCreatePanelAsync(typeof(T), panelPrefab, panelData, level);
        }
        
        public static async UniTask<IPanel> OpenOrCreatePanelAsync(this IUIManager manager, string panelTypeName, GameObject panelPrefab, IPanelData panelData = null, UILevel level = UILevel.Common)
        {
            Type panelType = FindPanelType(panelTypeName);
            return await manager.OpenOrCreatePanelAsync(panelType, panelPrefab, panelData, level);
        }

        public static async UniTask<IPanel> OpenOrCreatePanelAsync(this IUIManager manager, Type panelType, GameObject panelPrefab, IPanelData panelData = null, UILevel level = UILevel.Common)
        {
            if (manager.HasPanel(panelType))
            {
                return manager.OpenPanel(panelType, panelData, level);
            }
            
            await manager.CreatePanelAsync(panelType, panelPrefab, level);
            return manager.OpenPanel(panelType, panelData, level);
        }

        public static T OpenPanel<T>(this IUIManager manager, IPanelData panelData = null, UILevel? level = null) where T : Component, IPanel
        {
            return (T)manager.OpenPanel(typeof(T), panelData, level);
        }

        public static IPanel OpenPanel(this IUIManager manager, string panelTypeName, IPanelData panelData = null, UILevel? level = null)
        {
            return manager.OpenPanel(FindPanelType(panelTypeName), panelData, level);
        }

        private static Type FindPanelType(string panelTypeName)
        {
            if (_panelTypeCache.TryGetValue(panelTypeName, out Type cachedType))
            {
                return cachedType;
            }
            
            // 尝试使用完全限定名称直接查找类型
            Type type = Type.GetType(panelTypeName);
            if (type != null && IsValidPanelType(type))
            {
                _panelTypeCache[panelTypeName] = type;
                return type;
            }
            
            // 回退到搜索所有程序集
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                // 尝试使用完全限定名称精确匹配
                type = assembly.GetType(panelTypeName);
                if (type != null && IsValidPanelType(type))
                {
                    _panelTypeCache[panelTypeName] = type;
                    return type;
                }
                
                // 尝试通过类名查找
                var className = panelTypeName.Split('.').Last();
                type = assembly.GetTypes()
                    .FirstOrDefault(t => 
                        (t.Name == className || t.FullName == panelTypeName) && 
                        IsValidPanelType(t));
                
                if (type != null)
                {
                    _panelTypeCache[panelTypeName] = type;
                    return type;
                }
            }
            
            throw new ArgumentException($"Cannot find panel type with name: {panelTypeName}. " +
                                      "Panel must be a non-abstract, non-interface, non-generic type that implements IPanel and inherits from Component.");
        }
        
        /// <summary>
        /// 检查类型是否为有效的面板类型
        /// </summary>
        private static bool IsValidPanelType(Type type)
        {
            return typeof(IPanel).IsAssignableFrom(type) && 
                   typeof(Component).IsAssignableFrom(type) && 
                   !type.IsAbstract && 
                   !type.IsInterface && 
                   !type.IsGenericType;
        }
    }
}
