using UnityEngine;

namespace EasyGameFramework
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
