using System.Collections;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    [AddEasyFeedbackMenu("游戏物体/实例化")]
    public class EF_GameObjectInstantiate : AbstractEasyFeedback
    {
        public GameObject Prefab;
        public bool HasLiftTime;
        public float LiftTime;

        public Transform Parent;
        public Transform Relative;
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 LocalScale = Vector3.one;
        
        public override string Tip => "实例化一个GameObject";

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
