namespace EasyFramework.Serialization
{
    public enum EasySerializerProiority
    {
        Generic = -5000,
        Primitive = -4000,
        SystemBasic = -3000,
        UnityObject = -2000,
        UnityBasic = -1000,
        Custom = 0,
    }
}
