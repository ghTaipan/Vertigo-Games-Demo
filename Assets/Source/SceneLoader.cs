using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scene
{
    MainMenu,
    Gameplay,
    Bomb
}


public class SceneLoader : MonoBehaviour
{
    public static void LoadScene (Scene scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }
}
