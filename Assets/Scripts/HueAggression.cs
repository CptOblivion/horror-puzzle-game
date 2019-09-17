using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HueAggression : MonoBehaviour
{
    Material mat;
    public Color IdleColor;
    public Color SafeColor;
    public Color DangerColor;
    public float SafeDistance = 15;
    GlitchMonster glitchMonster;
    void Awake()
    {
        mat = GetComponent<SkinnedMeshRenderer>().material;
        glitchMonster = GetComponentInParent<GlitchMonster>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = (transform.position - GlobalTools.player.transform.position).magnitude;
        float threat = distance / SafeDistance;
        float ScrollThreat = 1- Mathf.Clamp01((threat * 5));
        Color FollowColor = Color.Lerp(DangerColor, SafeColor, threat);
        if (glitchMonster.aiState == GlitchMonster.AIState.Idle)
        {
            mat.SetColor("_Color", IdleColor);
            mat.SetColor("_ScrollColor", Color.white);
        }
        else if (glitchMonster.aiState == GlitchMonster.AIState.Activating)
        {
            float fade = 1 - glitchMonster.ActivateTimer / glitchMonster.WakeTime;
            mat.SetColor("_Color", Color.Lerp(IdleColor, FollowColor, fade));
            mat.SetColor("_ScrollColor", Color.Lerp(Color.white,Color.black, (ScrollThreat * fade)));
        }
        else if (glitchMonster.aiState == GlitchMonster.AIState.FacingIdle)
        {
            float fade = (glitchMonster.ActivateTimer / glitchMonster.WakeTime);
            mat.SetColor("_Color", Color.Lerp( IdleColor, FollowColor, fade));
            mat.SetColor("_ScrollColor", Color.Lerp(Color.white, Color.black, (ScrollThreat * fade)));
        }
        else
        {
            if (distance > SafeDistance)
            {
                mat.SetColor("_Color", SafeColor);
                mat.SetColor("_ScrollColor", Color.white);
            }
            else
            {
                mat.SetColor("_Color", FollowColor);
                mat.SetColor("_ScrollColor", Color.Lerp(Color.white, Color.black, (ScrollThreat)));
            }
        }
    }
}
