using System.Collections;
using EasyFramework.ToolKit;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyFramework.Modules
{
    [AddFeedbackMenu("游戏物体/实例化")]
    public class GameObjectInstantiateFeedback : AbstractFeedback
    {
        [FoldoutGroup("预制体设置")]
        [InfoBoxEx("预制体不能为空！", InfoMessageType.Error, nameof(ShowPrefabError))]
        [LabelText("预制体")]
        public GameObject Prefab;

        [FoldoutGroup("预制体设置")]
        [LabelText("拥有生命时间")]
        public bool HasLiftTime;

        [FoldoutGroup("预制体设置")]
        [ShowIf(nameof(HasLiftTime))]
        [LabelText("生命时间")]
        public float LiftTime;
        
        [FoldoutGroup("Transform设置")]
        [LabelText("父级")]
        public Transform Parent;
        
        [FoldoutGroup("Transform设置")]
        [LabelText("相对")]
        public Transform Relative;
        
        [FoldoutGroup("Transform设置")]
        [LabelText("坐标")]
        public Vector3 Position;
        
        [FoldoutGroup("Transform设置")]
        [LabelText("旋转")]
        public Vector3 Rotation;
        
        [FoldoutGroup("Transform设置")]
        [LabelText("局部缩放")]
        public Vector3 LocalScale = Vector3.one;
        
        public override string Tip => "实例化一个GameObject";

        private bool ShowPrefabError => Prefab == null;

        protected override void OnFeedbackPlay()
        {
            var inst = Object.Instantiate(Prefab, Parent);
            if (Relative != null)
            {
                inst.transform.position = Relative.transform.position + Position;
                inst.transform.rotation = Relative.transform.rotation * Quaternion.Euler(Rotation);
            }
            else
            {
                inst.transform.SetPositionAndRotation(Position, Quaternion.Euler(Rotation));
            }
            inst.transform.localScale = LocalScale;

            if (HasLiftTime && LiftTime > 0)
            {
                StartCoroutine(DestroyCo(inst));
            }
        }

        private IEnumerator DestroyCo(GameObject inst)
        {
            yield return new WaitForSeconds(LiftTime);
            Object.Destroy(inst);
        }

        protected override void OnFeedbackStop()
        {
            
        }
    }
}
