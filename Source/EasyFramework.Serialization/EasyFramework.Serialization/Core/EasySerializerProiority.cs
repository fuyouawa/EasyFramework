namespace EasyFramework.Serialization
{
    public enum EasySerializerProiority
    {
        Generic = -5000,
        SystemBasic = -4000,
        UnityObject = -3000,
        UnityBasic = -2000,
        Primitive = -1000,
        Custom = 0,
    }
}
