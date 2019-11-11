using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Color PulseColor1;
    public Color PulseColor2;
    public float PulseTime = 1;
    public bool DefaultToMainMenu = true;
    public Texture2D background1;

    float PulseTimer = 0;

    Camera camera;
    public static string Scene;
    public static string[] LevelTags = new string[0];

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }
    void Start()
    {
        GlobalTools.Unpause();
        if (Scene == null)
        {
            Debug.Log("No scene assigned!");
            if (DefaultToMainMenu)
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
        else
        {
            StartCoroutine(LoadScene());
            Scene = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        PulseTimer += Time.deltaTime;
        camera.backgroundColor = Color.Lerp(PulseColor1, PulseColor2, Mathf.Sin(PulseTimer * 2 * Mathf.PI / PulseTime) * .5f +.5f);
    }

    IEnumerator LoadScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(Scene);
        Debug.Log("Loading " + Scene);
        while (!asyncLoad.isDone)
        {
            RenderTexture.active = camera.targetTexture;
            background1.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
            background1.Apply();
            yield return null;
        }
    }
    public static void LoadLevel(string SceneName, string[] Tags = null)
    {
        Tags = Tags ?? new string[0];
        Scene = SceneName;
        SceneManager.LoadScene("LoadingScreen");
        LevelLoader.LevelTags = Tags;
    }
}
