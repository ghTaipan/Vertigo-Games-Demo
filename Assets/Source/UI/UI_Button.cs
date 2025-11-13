using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Button : MonoBehaviour, IPointerEnterHandler
{
    private Button _button;
    // Set it if you don't wanna use the default one.
    [SerializeField] private SoundClip _howerSound;
    [SerializeField] private bool bPlayHowerSound = true;
    // Set it if you don't wanna use the default one.
    [SerializeField] private SoundClip _clickSound;
    [SerializeField] private bool bPlayClickSound = true;

    private void OnValidate()
    {
        _button = GetComponent<Button>();
    }

    void Awake()
    {
        _button = GetComponent<Button>();
    }

    void Start()
    {
        if (bPlayClickSound)
        {
            _button.onClick.AddListener(OnButtonClick);
        }
    }

    private void OnDestroy()
    {
        if (_button)
        {
            _button.onClick.RemoveListener(OnButtonClick);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (bPlayHowerSound && _button.IsInteractable())
        { 
            AudioManager.Instance.PlayUISound(E_UISound.ButtonHover, _howerSound);
        }
    }

    public void OnButtonClick()
    {
        AudioManager.Instance.PlayUISound(E_UISound.ButtonClick, _clickSound);
    }
}
