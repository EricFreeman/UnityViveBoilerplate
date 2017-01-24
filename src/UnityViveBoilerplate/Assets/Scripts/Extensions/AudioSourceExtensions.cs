using UnityEngine;

namespace Assets.Scripts.Extensions
{
    public static class AudioSourceExtensions
    {
        public static AudioSource Randomize(this AudioSource audioSource, float amount = .05f)
        {
            audioSource.pitch = Random.Range(1 - amount, 1 + amount);
            return audioSource;
        }
    }
}