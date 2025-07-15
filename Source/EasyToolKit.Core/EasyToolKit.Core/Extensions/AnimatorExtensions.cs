using System.Linq;
using UnityEngine;

namespace EasyToolKit.Core
{
    public static class AnimatorExtensions
    {
        public static bool HasParam(this Animator animator, string name)
        {
            return animator.parameters.FirstOrDefault(x => x.name == name) != null;
        }

        public static bool HasParam(this Animator animator, string name, AnimatorControllerParameterType typeCheck)
        {
            return animator.parameters.FirstOrDefault(x => x.name == name && x.type == typeCheck) != null;
        }
    }
}
