using Dispersion.Enum;
using System;
using UnityEngine;

namespace Dispersion.Sound
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioSource SoundEffect;

        [SerializeField] private SoundType[] Sounds;

        public static SoundManager Instance { get; private set; }

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void PlayEffects(Sounds sound)
        {
            AudioClip clip = GetAudioClip(sound);
            if (clip != null)
            {
                SoundEffect.PlayOneShot(clip);
            }
        }

        private AudioClip GetAudioClip(Sounds sound)
        {
            SoundType item = Array.Find(Sounds, i => i.soundType == sound);
            if (item != null)
            {
                return item.soundClip;
            }
            else
            {
                return null;
            }
        }
    }

    [Serializable]
    public class SoundType
    {
        public Sounds soundType;
        public AudioClip soundClip;
    }
}
