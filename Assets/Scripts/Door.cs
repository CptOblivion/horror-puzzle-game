using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class AnimsWithOffset
{
    public Animation anim;
    public float Offset;
}
public class Door : MonoBehaviour
{
    public string SaveName = "";
    public bool Locked = false;
    public bool Closed = true;
    [Tooltip("Disables further interaction after opening")]
    public bool LockOpen = true;
    public bool UpdateBackground = false;
    public bool HaltGame = true;
    public string LockedMessage;
    public string WrongSideMessage;

    string savePrefix;
    //public Collider Rightside;
    //public Collider WrongSide;

    public AnimsWithOffset[] anims;

    bool toggling = false;

    private void Start()
    {
        savePrefix = SceneManager.GetActiveScene().name + " Door ";
        if (SaveName != "")
        {
            bool? closed = SaveManager.GetBool(savePrefix + SaveName);
            if (closed != null && closed == false)
            {
                Locked = false;
                InteractSkip();
            }
        }
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
    public void UnlockAndOpen()
    {
        Unlock();
        Interact();
    }
    void InteractSkip()
    {
        Closed = !Closed;
        if (LockOpen)
        {
            foreach (InteractTarget interactTarget in GetComponentsInChildren<InteractTarget>())
            {
                interactTarget.enabled = false;
            }
        }
        foreach (AnimsWithOffset animWithOffset in anims)
        {
            animWithOffset.anim[animWithOffset.anim.clip.name].speed = 0;
            animWithOffset.anim[animWithOffset.anim.clip.name].time = animWithOffset.anim.clip.length;
            animWithOffset.anim.Play();
        }

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
            if (SaveName != "")
            {
                SaveManager.SetBool(savePrefix + SaveName, Closed);
            }
            foreach (AnimsWithOffset animWithOffset in anims)
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

}