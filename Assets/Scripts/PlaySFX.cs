using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlaySFX : MonoBehaviour
{
    static AudioMixer audioMixer;
    float destroyTimer;

    void Update()
    {
        destroyTimer -= Time.unscaledDeltaTime;
        if (destroyTimer <= 0)
        {
            Destroy(gameObject);
        }
    }
    public static GameObject Play(AudioClip clip, float pitch = 1)
    {
        if (audioMixer == null)
        {
            audioMixer = Resources.Load("AudioMixer") as AudioMixer;
        }
        GameObject SoundPlayer = new GameObject("SoundPlayer");
        AudioSource audioSource = SoundPlayer.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SoundEffects")[0];
        audioSource.clip = clip;
        audioSource.pitch = pitch;
        audioSource.Play();
        PlaySFX playSFX = SoundPlayer.AddComponent<PlaySFX>();
        playSFX.destroyTimer = clip.length / pitch;
        return SoundPlayer;
    }
}
