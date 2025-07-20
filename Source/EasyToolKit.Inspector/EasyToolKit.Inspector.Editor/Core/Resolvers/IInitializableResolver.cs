namespace EasyToolKit.Inspector.Editor
{
    public interface IInitializableResolver : IInitializable
    {
        InspectorProperty Property { get; set; }
    }
}
