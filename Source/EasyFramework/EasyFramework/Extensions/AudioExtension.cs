using UnityEngine;

namespace EasyFramework
{
    public static class AudioExtension
    {
        public static void PlayWithChildren(this AudioSource audio)
        {
            foreach (var a in audio.GetComponentsInChildren<AudioSource>())
            {
                a.Play();
            }
        }
    }
}
