namespace EasyToolKit.Tweening
{
    public static class TweenSequenceExtension
    {
        public static TweenSequence Append(this TweenSequence sequence, AbstractTween tween)
        {
            sequence.AddTweenAsNewClip(tween);
            return sequence;
        }

        public static TweenSequence Join(this TweenSequence sequence, AbstractTween tween)
        {
            sequence.AddTweenToLastClip(tween);
            return sequence;
        }
    }
}
