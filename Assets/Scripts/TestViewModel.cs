using System.Reflection;
using EasyFramework;
using EasyFramework.ToolKit;
using EasyFramework.ToolKit.TemplateEngine;
using UnityEngine;

public partial class TestViewModel : MonoBehaviour
{
    void Start()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.UnityConsole()
            .MinimumLevel.Debug()
            .CreateLogger();

        var te = new TemplateEngine();
        var data = new
        {
            class_name = "Test",
            has_id = true,
        };

        var t = @"
    public class {{ class_name }} {
        {% if has_id %}
        public int Id { get; set; }
        {% endif %}
        public string Name { get; set; }
    }";

        var r = te.Render(t, data);
        Debug.Log(r);
    }

    void Update()
    {
    }
}
