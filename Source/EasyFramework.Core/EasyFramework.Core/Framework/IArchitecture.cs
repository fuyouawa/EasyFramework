using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace EasyFramework.Core
{
    public interface IBelongToArchitecture
    {
        IArchitecture GetArchitecture();
    }

    public interface ICanInitializeAsync
    {
        bool IsInitialized { get; }
        UniTask InitializeAsync();
        UniTask DeinitializeAsync();
    }

    public interface ICanSetArchitecture
    {
        void SetArchitecture(IArchitecture architecture);
    }

    public interface IArchitecture
    {
        internal static readonly List<IArchitecture> Architectures = new List<IArchitecture>();
        static IArchitecture[] GetArchitectures() => Architectures.ToArray();

        int Index { get; }

        UniTask InitializeAsync();
        UniTask DeinitializeAsync();
        
        void RegisterSystem<TSystem>(TSystem system) where TSystem : ISystem;
        void RegisterModel<TModel>(TModel model) where TModel : IModel;
        void RegisterUtility<TUtility>(TUtility utility) where TUtility : IUtility;

        /// <summary>
        /// 增量异步初始化，用于初始化动态注册的System/Model。
        /// </summary>
        /// <returns></returns>
        UniTask IncrementalInitializeAsync();

        /// <summary>
        /// 增量异步反初始化化，用于反初始化动态取消注册的System/Model。
        /// </summary>
        /// <returns></returns>
        UniTask IncrementalDeinitializeAsync();

        /// <summary>
        /// <para>动态注册System，主要用于热更新场景下动态加载dll后注册System。</para>
        /// <para>记得调用<see cref="IncrementalInitializeAsync"/>进行初始化</para>
        /// </summary>
        /// <typeparam name="TSystem"></typeparam>
        /// <param name="system"></param>
        /// <returns></returns>
        void DynamicRegisterSystem<TSystem>(TSystem system) where TSystem : ISystem;
        
        /// <summary>
        /// <para>动态取消注册System，主要用于热更新场景下动态卸载dll前取消注册System。</para>
        /// <para>记得调用<see cref="IncrementalDeinitializeAsync"/>进行反初始化</para>
        /// </summary>
        /// <typeparam name="TSystem"></typeparam>
        /// <param name="system"></param>
        /// <returns></returns>
        void DynamicUnregisterSystem<TSystem>(TSystem system) where TSystem : ISystem;

        /// <summary>
        /// <para>动态注册Model，主要用于热更新场景下动态加载dll后注册Model。</para>
        /// <para>记得调用<see cref="IncrementalInitializeAsync"/>进行初始化</para>
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        void DynamicRegisterModel<TModel>(TModel model) where TModel : IModel;
        
        /// <summary>
        /// <para>动态取消注册Model，主要用于热更新场景下动态卸载dll前取消注册Model。</para>
        /// <para>记得调用<see cref="IncrementalDeinitializeAsync"/>进行反初始化</para>
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        void DynamicUnregisterModel<TModel>(TModel model) where TModel : IModel;

        T GetSystem<T>() where T : class, ISystem;
        T GetModel<T>() where T : class, IModel;
        T GetUtility<T>() where T : class, IUtility;

        ISystem[] GetAllSystems();
        IModel[] GetAllModels();
        IUtility[] GetAllUtilities();

        void SendCommand(ICommand command);
        TResult SendQuery<TResult>(IQuery<TResult> query);

        void SendEvent<T>(T e);

        IFromRegisterEvent RegisterEvent<T>(EasyEventHandler<T> onEvent);
        void UnregisterEvent<T>(EasyEventHandler<T> onEvent);
    }

}
