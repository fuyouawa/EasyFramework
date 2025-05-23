using EasyFramework.Core;
using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    public class AbstractFeedbackDrawer<T> : IFeedbackDrawer<T>
        where T : AbstractFeedback
    {
        private InspectorProperty _labelProperty;
        private InspectorProperty _enableProperty;
        private InspectorProperty _delayBeforePlayProperty;
        private InspectorProperty _blockingProperty;
        private InspectorProperty _repeatForeverProperty;
        private InspectorProperty _amountOfRepeatProperty;
        private InspectorProperty _intervalBetweenRepeatsProperty;

        private FoldoutGroupConfig _foldoutGroupConfig;

        protected override void Initialize()
        {
            base.Initialize();

            _labelProperty = Property.Children["_label"];
            _enableProperty = Property.Children["_enable"];
            _delayBeforePlayProperty = Property.Children["_delayBeforePlay"];
            _blockingProperty = Property.Children["_blocking"];
            _repeatForeverProperty = Property.Children["_repeatForever"];
            _amountOfRepeatProperty = Property.Children["_amountOfRepeat"];
            _intervalBetweenRepeatsProperty = Property.Children["_intervalBetweenRepeats"];

            _foldoutGroupConfig = new FoldoutGroupConfig(
                UniqueDrawerKey.Create(_delayBeforePlayProperty, this),
                new GUIContent("反馈设置"),
                false, DrawSettings);
        }

        protected override void OnContentGUI(Rect headerRect)
        {
            var value = ValueEntry.SmartValue;

            if (value.Tip.IsNotNullOrWhiteSpace())
            {
                EasyEditorGUI.MessageBox(value.Tip, MessageType.Info);
            }

            _labelProperty.DrawEx("标签");
            _enableProperty.DrawEx("启用");

            _foldoutGroupConfig.Expand = EasyEditorGUI.FoldoutGroup(_foldoutGroupConfig);

            foreach (var child in Property.Children)
            {
                if (child.Info.GetMemberInfo().DeclaringType != typeof(AbstractFeedback))
                {
                    child.Draw();
                }
            }
        }

        private void DrawSettings(Rect headerRect)
        {
            _delayBeforePlayProperty.DrawEx("播放前延迟", "在正式Play前经过多少时间的延迟(s)");
            _blockingProperty.DrawEx("阻塞", "是否会阻塞反馈运行");
            _repeatForeverProperty.DrawEx("无限重复", "无限重复播放");
            _amountOfRepeatProperty.DrawEx("重复次数", "重复播放的次数");
            _intervalBetweenRepeatsProperty.DrawEx("重复间隔", "每次循环播放的间隔");
        }
    }
}
