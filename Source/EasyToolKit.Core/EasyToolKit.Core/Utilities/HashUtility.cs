using EasyToolKit.ThirdParty.xxHash;

namespace EasyToolKit.Core
{
    public static class HashUtility
    {
        public static uint Compute32(string key, uint seed = 0)
        {
            return xxHash32.ComputeHash(key, seed);
        }
    }
}
