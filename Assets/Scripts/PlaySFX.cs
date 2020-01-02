using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlaySFX : MonoBehaviour
{
    public static AudioMixer audioMixer;
    float destroyTimer;

    void Update()
    {
        destroyTimer -= Time.unscaledDeltaTime;
        if (destroyTimer <= 0)
        {
            Destroy(gameObject);
        }
    }
    public static GameObject Play(AudioClip clip, float pitch = 1, Vector3? Position = null, bool Menu = false)
    {
        GameObject SoundPlayer = new GameObject("SoundPlayer");
        AudioSource audioSource = SoundPlayer.AddComponent<AudioSource>();
        if (Position != null)
        {
            SoundPlayer.transform.position = (Vector3)Position;
            audioSource.spatialBlend = 1;
        }
        if (Menu)
        {
            audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("MenuSounds")[0];
        }
        else
        {
            audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SoundEffects")[0];
        }
        audioSource.clip = clip;
        audioSource.pitch = pitch;
        audioSource.Play();
        PlaySFX playSFX = SoundPlayer.AddComponent<PlaySFX>();
        playSFX.destroyTimer = clip.length / pitch;
        return SoundPlayer;
    }

    public static void Initialize()
    {
        if (audioMixer == null)
        {
            audioMixer = Resources.Load("AudioMixer") as AudioMixer;
        }
    }
}
