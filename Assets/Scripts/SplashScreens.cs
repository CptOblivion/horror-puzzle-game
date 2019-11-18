using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

[System.Serializable]
public class SplashScreeenElement
{
    public Texture image;
    public VideoClip video;
    public float DisplayTime;
    public float HoldTime;
}

public class SplashScreens : MonoBehaviour
{
    [Tooltip("Disable splash screens (faster startup)")]
    public bool SkipSplashScreens;
    [Tooltip("Enable this object (activate the main menu) when we're done with the splash screens")]
    public GameObject enableOnEnd;
    public SplashScreeenElement[] splashScreens;
    [Tooltip("Time to fade to FadeColor after splash screens are done")]
    public float FadeOut = 1;
    [Tooltip("Time to wait before fading in on menu")]
    public float FadeHold = 0.5f;
    [Tooltip("Time to fade in on menu")]
    public float FadeIn = 1;
    float FadeOutTimer;
    float FadeHoldTimer;
    float FadeInTimer;
    int currentClip = 0;
    float DisplayTimer = 0;
    float HoldTimer = 0;

    public static bool SplashScreenDone = false;

    VideoPlayer videoPlayer;
    AudioSource audioSource;
    RawImage rawImage;
    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        audioSource = GetComponent<AudioSource>();
        rawImage = GetComponent<RawImage>();
        if (SkipSplashScreens) SplashScreenDone = true;
    }
    void Start()
    {
        //I'm too lazy to pass through fadehold so this makes it run for at least 1 frame
        if (FadeHold <= 0)
        {
            FadeHold = .001f;
        }
        if (SplashScreenDone)
        {
            if (enableOnEnd)
            {
                enableOnEnd.SetActive(true);
            }
            Destroy(gameObject);
        }
        else
        {
            videoPlayer.SetTargetAudioSource(0, audioSource);
            UpdateSplashScreen(0);
            audioSource.Play();
        }

    }
    void Update()
    {
        if (FadeOutTimer > 0)
        {
            FadeOutTimer -= Time.deltaTime;

            //squared for a smoother falloff
            float FadeProgress = 1 - Mathf.Pow(1-(FadeOutTimer / FadeOut), 2);
            if (videoPlayer.clip)
            {
                videoPlayer.targetCameraAlpha = FadeProgress;
                rawImage.color = Color.black;
            }
            else
            {
                rawImage.color = new Vector4(FadeProgress, FadeProgress, FadeProgress, 1);
            }
            if (FadeOutTimer <= 0)
            {
                FadeHoldTimer = FadeHold;
            }
        }
        else if (FadeHoldTimer > 0)
        {
            FadeHoldTimer -= Time.deltaTime;
            if (FadeHoldTimer < 0)
            {
                if (FadeIn > 0)
                {
                    FadeInTimer = FadeIn;
                }
                else
                {
                    EndSelf();
                }
            }
        }
        else if (FadeInTimer > 0)
        {
            FadeInTimer -= Time.deltaTime;
            //gamma for a smoother falloff with a more visibly defined bottom end
            float FadeProgress = Mathf.Pow(FadeInTimer / FadeIn, 1 / 2.2f);
            rawImage.color = new Vector4(0, 0, 0, FadeProgress);
            if (FadeInTimer <= 0)
            {
                EndSelf();
            }
        }
        else if (DisplayTimer > 0)
        {
            DisplayTimer -= Time.deltaTime;
        }
        else if (HoldTimer > 0)
        {
            HoldTimer -= Time.deltaTime;
        }
        else
        {
            currentClip++;
            if (currentClip < splashScreens.Length)
            {
                UpdateSplashScreen(currentClip);
            }
            else
            {
                if (FadeOut > 0)
                {
                    rawImage.texture = null;
                    FadeOutTimer = FadeOut;
                }
                else
                {
                    EndSelf();
                }
            }
        }
    }

    void EndSelf()
    {
        if (enableOnEnd)
        {
            enableOnEnd.SetActive(true);
        }
        Destroy(gameObject);
    }
    void UpdateSplashScreen(int i)
    {
        if (splashScreens[i].video)
        {
            videoPlayer.clip = splashScreens[i].video;
            DisplayTimer = (float)videoPlayer.length / videoPlayer.playbackSpeed;
            videoPlayer.Play();
        }
        else if (splashScreens[i].image)
        {
            videoPlayer.clip = null;
            videoPlayer.Stop();
            rawImage.texture = splashScreens[i].image;
            DisplayTimer = splashScreens[i].DisplayTime;
        }
        else
        {
            Debug.LogError("No Splash Screen!");
            DisplayTimer = 0;
            HoldTimer = 0;
        }
        HoldTimer = splashScreens[i].HoldTime;
    }
}
