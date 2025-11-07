using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private Button _buttonPlay;
    [SerializeField] private Button _buttonQuit;

    void Start()
    {
        _buttonPlay.onClick.AddListener(OnPlayButtonPressed);
        _buttonQuit.onClick.AddListener(OnQuitButtonPressed);
    }

    void OnDestroy()
    {
        _buttonPlay.onClick.RemoveListener(OnPlayButtonPressed);
        _buttonPlay.onClick.RemoveListener(OnQuitButtonPressed);
    }


    void OnPlayButtonPressed()
    {
        SceneLoader.LoadScene(Scene.Gameplay);
    }

    void OnQuitButtonPressed()
    {
        Application.Quit();
    }
}
