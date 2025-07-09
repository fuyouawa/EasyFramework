using Cysharp.Threading.Tasks;

namespace EasyToolKit.Tweening
{
    public static class TweenUniTaskExtension
    {
        public static UniTask WaitForPlay(this AbstractTween tween)
        {
            var tcs = new UniTaskCompletionSource();
            tween.OnPlay(() => tcs.TrySetResult());
            return tcs.Task;
        }

        public static UniTask WaitForPause(this AbstractTween tween)
        {
            var tcs = new UniTaskCompletionSource();
            tween.OnPause(() => tcs.TrySetResult());
            return tcs.Task;
        }

        public static UniTask WaitForComplete(this AbstractTween tween)
        {
            var tcs = new UniTaskCompletionSource();
            tween.OnComplete(() => tcs.TrySetResult());
            return tcs.Task;
        }

        public static UniTask WaitForKill(this AbstractTween tween)
        {
            var tcs = new UniTaskCompletionSource();
            tween.OnKill(() => tcs.TrySetResult());
            return tcs.Task;
        }

    }
}