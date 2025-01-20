using EasyFramework.Inspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Tools.Editor
{
    [DrawerPriority(0.0, 0.0, 1.1)]
    public class EF_GameObjectInstantiateDrawer : AbstractEasyFeedbackDrawer<EF_GameObjectInstantiate>
    {
        private InspectorProperty _prefab;
        private InspectorProperty _hasLiftTime;
        private InspectorProperty _liftTime;
        private InspectorProperty _parent;
        private InspectorProperty _relative;
        private InspectorProperty _position;
        private InspectorProperty _rotation;
        private InspectorProperty _localScale;

        protected override void Initialize()
        {
            base.Initialize();
            _prefab = Property.Children["Prefab"];
            _hasLiftTime = Property.Children["HasLiftTime"];
            _liftTime = Property.Children["LiftTime"];
            _parent = Property.Children["Parent"];
            _relative = Property.Children["Relative"];
            _position = Property.Children["Position"];
            _rotation = Property.Children["Rotation"];
            _localScale = Property.Children["LocalScale"];
        }

        protected override void DrawOtherPropertyLayout()
        {
            base.DrawOtherPropertyLayout();
            var value = ValueEntry.SmartValue;

            _prefab.State.Expanded = EasyEditorGUI.FoldoutGroup(new FoldoutGroupConfig(
                UniqueDrawerKey.Create(_prefab, this),
                "实例化", _prefab.State.Expanded)
            {
                OnContentGUI = rect =>
                {
                    if (value.Prefab == null)
                    {
                        EasyEditorGUI.MessageBox("预制体不能为空！", MessageType.Error);
                    }

                    _prefab.Draw(new GUIContent("预制体"));
                    _hasLiftTime.Draw(new GUIContent("拥有生命时间"));

                    if (value.HasLiftTime)
                    {
                        _liftTime.Draw(new GUIContent("生命时间"));
                    }

                    _parent.Draw(new GUIContent("父级"));
                    _relative.Draw(new GUIContent("相对"));
                    _position.Draw(new GUIContent("坐标"));
                    _rotation.Draw(new GUIContent("旋转"));
                    _localScale.Draw(new GUIContent("局部缩放"));
                }
            });
        }
    }
}
