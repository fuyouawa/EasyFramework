namespace EasyToolKit.ToolKit
{
    /// <summary>
    /// 对象池事件的回调接口。
    /// 
    /// <para>若该接口由普通类实现，则在对象被取出/回收时会直接调用该实例的方法。</para>
    /// <para>若由 Unity 组件实现，则在对象池操作时，会调用该 GameObject 上所有实现该接口的组件。</para>
    /// <para>该接口为可选实现，仅在需要监听池事件时实现。</para>
    /// </summary>
    public interface IPoolCallbackReceiver
    {
        /// <summary>
        /// 对象从对象池中被取出时调用。
        /// </summary>
        /// <param name="owningPool">所属的对象池</param>
        void OnRent(IObjectPool owningPool);
        
        /// <summary>
        /// 对象被回收到对象池中时调用。
        /// </summary>
        /// <param name="owningPool">所属的对象池</param>
        void OnRelease(IObjectPool owningPool);
    }
}
