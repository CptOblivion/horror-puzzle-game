using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimsWithOffset
{
    public Animation anim;
    public float Offset;
}
public class Door : MonoBehaviour
{
    public bool Locked = false;
    public bool Closed = true;
    [Tooltip("Disables further interaction after opening")]
    public bool LockOpen = true;
    public bool UpdateBackground = false;
    //TODO: make it so all other gamelogic and animations pause until door opening animation is done
    public bool HaltGame = true;
    public string LockedMessage;
    public string WrongSideMessage;
    public Collider Rightside;
    public Collider WrongSide;

    bool toggling = false;

    public AnimsWithOffset[] anims;

    public void Unlock()
    {
        Locked = false;
    }

    public void Lock()
    {
        Locked = true;
    }

    public void ToggleLock()
    {
        Locked = !Locked;
    }
    public void Interact()
    {
        if (Locked)
        {
            //Debug.Log(LockedMessage);
            ScreenText.DisplayText(LockedMessage);
        }
        else
        {
            toggling = true;
            foreach (InteractTarget interactTarget in GetComponentsInChildren<InteractTarget>())
            {
                interactTarget.enabled = false;
            }
            Closed = !Closed;
            foreach(AnimsWithOffset animWithOffset in anims)
            {
                animWithOffset.anim[animWithOffset.anim.clip.name].speed = 0;
                animWithOffset.anim[animWithOffset.anim.clip.name].time = -animWithOffset.Offset;
                animWithOffset.anim.Play();
            }
            if (HaltGame)
            {
                Time.timeScale = 0;
            }
        }
    }

    public void InteractWrongSide()
    {
        //Debug.Log(WrongSideMessage);
        ScreenText.DisplayText(WrongSideMessage);
    }

    private void Update()
    {
        if (!GlobalTools.Paused)
        {
            if (toggling)
            {
                bool doneToggling = true;
                foreach (AnimsWithOffset animWithOffset in anims)
                {
                    if (animWithOffset.anim.isPlaying)
                    {
                        animWithOffset.anim[animWithOffset.anim.clip.name].time += Time.unscaledDeltaTime;
                        doneToggling = false;
                    }
                }
                if (doneToggling)
                {
                    toggling = false;
                    if (UpdateBackground)
                    {
                        GlobalTools.currentCam.GetComponent<UpdateBG>().PrepRender();
                    }
                    if (!LockOpen)
                    {
                        foreach (InteractTarget interactTarget in GetComponentsInChildren<InteractTarget>())
                        {
                            interactTarget.enabled = true;
                        }
                    }
                    if (HaltGame)
                    {
                        Time.timeScale = 1;
                    }
                }
            }
        }
    }
}