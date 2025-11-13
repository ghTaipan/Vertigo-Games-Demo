using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class UI_Scene : MonoBehaviour
{
    protected Button _buttonSpin;
    protected Button _buttonShowInventory;
    protected Button _buttonHideInventory;

    [SerializeField] protected Animator _sceneTransitionAnimator;
    [SerializeField] private Animator _inventoryAnimator;

    void OnValidate()
    {
        AssignButtons();
    }

    void Awake()
    {
        AssignButtons();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        _sceneTransitionAnimator.Play("FadeIn");
        _sceneTransitionAnimator.Update(0f);

        _buttonSpin.onClick.AddListener(OnSpinButtonPressed);
        _buttonShowInventory.onClick.AddListener(OnShowInventoryButtonPressed);
        _buttonHideInventory.onClick.AddListener(OnHideInventoryButtonPressed);
    }

    protected virtual void OnDestroy()
    {
        if (_buttonSpin)
        {
            _buttonSpin.onClick.RemoveListener(OnSpinButtonPressed);
        }

        if (_buttonShowInventory)
        {
            _buttonShowInventory.onClick.RemoveListener(OnShowInventoryButtonPressed);
        }

        if (_buttonHideInventory)
        {
            _buttonHideInventory.onClick.RemoveListener(OnShowInventoryButtonPressed);
        }
    }

    protected virtual void AssignButtons()
    {
        if (!_buttonSpin)
        {
            _buttonSpin = GetComponentsInChildren<Button>(true).FirstOrDefault(b => b.name == "SpinButton");
        }

        if (!_buttonShowInventory)
        {
            _buttonShowInventory = GetComponentsInChildren<Button>(true).FirstOrDefault(b => b.name == "ShowInventoryButton");
        }

        if (!_buttonHideInventory)
        {
            _buttonHideInventory = GetComponentsInChildren<Button>(true).FirstOrDefault(b => b.name == "HideInventoryButton");
        }
    }

    void OnSpinButtonPressed()
    {
        _buttonSpin.interactable = false;
        GameManager.Instance.StartSpin();
    }

    void OnShowInventoryButtonPressed()
    {
        AdjustInventory(true);
    }

    void OnHideInventoryButtonPressed()
    {
        AdjustInventory(false);
    }

    protected virtual void AdjustInventory(bool bShow)
    {
        string prefix = bShow ? "ShowInventory_" : "HideInventory_";

        _inventoryAnimator.Play(prefix + "Inventory");
        _inventoryAnimator.Update(0f);

        _buttonShowInventory.interactable = !bShow;
        _buttonHideInventory.interactable = bShow;
    }

    protected void ChangeScene(E_Scene scene, Action action = default)
    {
        _sceneTransitionAnimator.Play("FadeOut");
        _sceneTransitionAnimator.Update(0f);
        StartCoroutine(SceneTransition(scene, _sceneTransitionAnimator.GetCurrentAnimatorStateInfo(0).length - 0.15f, action));
    }

    IEnumerator SceneTransition(E_Scene scene, float length, Action action)
    {
        yield return new WaitForSeconds(length);
        if (action != null)
        {
            action();
        }
        
        SceneLoader.LoadScene(scene);
    }
}
