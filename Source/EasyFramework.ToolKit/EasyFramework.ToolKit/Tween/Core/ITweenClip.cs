namespace EasyFramework.ToolKit
{
    public enum TweenType
    {
        Tweener,
        Sequence,
        Callback
    }

    public interface ITweenClip
    {
        internal TweenType Type { get; }
    }
}
