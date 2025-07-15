namespace EasyToolKit.Core
{
    public static class UnityVersion
    {
        public static bool IsVersionOrGreater(int major, int minor)
        {
            return ThirdParty.OdinSerializer.Utilities.UnityVersion.IsVersionOrGreater(major, minor);
        }
    }
}
