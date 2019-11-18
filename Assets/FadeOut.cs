using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    public float FadeTime = .5f;
    public float FadeTimer;
    RawImage rawImage;
    
    // Start is called before the first frame update
    void Start()
    {
        rawImage = GetComponent<RawImage>();
        FadeTimer = FadeTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (FadeTimer > 0)
        {
            FadeTimer -= Mathf.Min(Time.unscaledDeltaTime, .01f);
            rawImage.color = new Color(1, 1, 1, FadeTimer / FadeTime);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
