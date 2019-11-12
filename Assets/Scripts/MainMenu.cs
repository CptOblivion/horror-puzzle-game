using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MainMenu : MonoBehaviour
{
    public Button ContinueButton;
    public MenuManager menuManager;
    public void Start()
    {
        Debug.Log(SaveManager.SaveName);
        if (File.Exists(SaveManager.SaveName))
        {
            ContinueButton.gameObject.SetActive(true);
            menuManager.selectOnWake = ContinueButton.gameObject;
        }
        else
        {
            ContinueButton.gameObject.SetActive(false);
        }
    }
    public void LoadScene(string sceneName)
    {
        LevelLoader.LoadLevel(sceneName);
    }
}
