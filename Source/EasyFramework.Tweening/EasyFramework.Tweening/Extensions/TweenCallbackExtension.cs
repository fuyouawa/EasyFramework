using System;

namespace EasyFramework.Tweening
{
    public static class TweenCallbackExtension
    {
        public static TweenCallback AddCallback(this TweenCallback tweenCallback, Action callback)
        {
            tweenCallback.Callback += callback;
            return tweenCallback;
        }

        public static TweenCallback RemoveCallback(this TweenCallback tweenCallback, Action callback)
        {
            tweenCallback.Callback -= callback;
            return tweenCallback;
        }
    }
}
