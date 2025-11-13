using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_MainMenu : UI_Scene
{
    private Button _buttonPlay;
    private Button _buttonQuit;

    [SerializeField] private SoundClip _spinReadySound;
    [SerializeField] private Animator _wheelAnimator;
    [SerializeField] private Animator _wheelUIAnimator;

    protected override void Start()
    {
        base.Start();

        _buttonPlay.onClick.AddListener(OnPlayButtonPressed);
        _buttonQuit.onClick.AddListener(OnQuitButtonPressed);

        GameManager.Instance.onSpinResult += OnSpinResult; 
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (_buttonPlay)
        {
            _buttonPlay.onClick.RemoveListener(OnPlayButtonPressed);
        }

        if (_buttonQuit)
        {
            _buttonQuit.onClick.RemoveListener(OnQuitButtonPressed);
        }

        if (GameManager.Instance)
        {
            GameManager.Instance.onSpinResult -= OnSpinResult;
        }
    }

    protected override void AssignButtons()
    {
        base.AssignButtons();
        if (!_buttonPlay)
        {
            _buttonPlay = GetComponentsInChildren<Button>(true).FirstOrDefault(b => b.name == "PlayButton");
        }

        if (!_buttonQuit)
        {
            _buttonQuit = GetComponentsInChildren<Button>(true).FirstOrDefault(b => b.name == "QuitButton");
        }
    }

    void OnPlayButtonPressed()
    {
        _buttonPlay.interactable = false;
        _buttonQuit.interactable = false;
        _buttonSpin.interactable = false;
        _buttonShowInventory.interactable = false;
        _buttonHideInventory.interactable = false;

        ChangeScene(E_Scene.Gameplay);
    }

    protected override void AdjustInventory(bool bShow)
    {
        base.AdjustInventory(bShow);

        string prefix;

        if (bShow)
        {
            prefix = "ShowInventory_";
            _buttonSpin.interactable = false;
        }
        else
        {
            prefix = "HideInventory_";
            _buttonSpin.interactable = true;
        }

        _wheelAnimator.Play(prefix + "Wheel");
        _wheelAnimator.Update(0f);

        _wheelUIAnimator.Play(prefix + "WheelUI");
        _wheelUIAnimator.Update(0f);

        
    }

    void OnQuitButtonPressed()
    {
        Application.Quit();
    }

    void OnSpinResult(ItemData itemData)
    {
        _buttonSpin.interactable = true;
        AudioManager.Instance.PlaySoundClip(_spinReadySound);
    }
}
