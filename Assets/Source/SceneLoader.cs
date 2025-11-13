using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum E_Scene
{
    MainMenu,
    Gameplay,
}

public delegate void SceneChangedDelegate(E_Scene newScene);

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; set; }

    private static E_Scene currentScene;
    public SceneChangedDelegate onSceneChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SceneManager.sceneLoaded += SceneChanged;
        StartCoroutine(FirstScene());
    }

    IEnumerator FirstScene()
    {
        yield return new WaitForSeconds(0.25f);

        string sceneName = SceneManager.GetActiveScene().name;
        foreach (E_Scene value in Enum.GetValues(typeof(E_Scene)))
        {
            if (value.ToString().Equals(sceneName))
            {
                currentScene = value;
                SceneChanged(0, 0);
            }
        }
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= SceneChanged;
    }
    private void SceneChanged<T0, T1>(T0 arg0, T1 arg1)
    {
        onSceneChanged?.Invoke(currentScene);
    }

    public static void LoadScene (E_Scene scene)
    {
        SceneManager.LoadScene(scene.ToString());
        currentScene = scene;
    }
    public E_Scene CurrentScene {
        get { return currentScene; }
    }
}
