using System;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [Serializable]
    public struct SoundEffect
    {
        public string name;
        public AudioClip clip;
    }

    [SerializeField] private SoundEffect[] soundEffects;

    private AudioSource audioSource;

    public static AudioPlayer Instance;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySoundEffect(string name)
    {
        // Search for sound effect in list and play it
        for (int i = 0; i < soundEffects.Length; i++)
        {
            if (soundEffects[i].name == name)
            {
                audioSource.clip = soundEffects[i].clip;
                audioSource.Play();
            }
        }
    }
}
