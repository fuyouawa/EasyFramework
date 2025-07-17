using EasyToolKit.ThirdParty.Scriban;

namespace EasyToolKit.Inspector.Editor
{
    public class InspectorText
    {
        private readonly InspectorProperty _property;
        private readonly Template _template;
        private readonly object _model;

        public InspectorText(InspectorProperty property, string text)
        {
            _property = property;
            _template = Template.Parse(text);
            _model = new
            {
                self = property.GetOwner(0)
            };
        }

        public string Render()
        {
            return _template.Render(_model, member => member.Name);
        }
    }
}
