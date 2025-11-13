using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class UI_GameplayScene : UI_Scene
{
    [SerializeField] private TextMeshProUGUI _itemText;
    [SerializeField] private Image _itemIcon;

    [SerializeField] private GameObject _winRoot;
    [SerializeField] private GameObject _loseRoot;

    [SerializeField] private ReviveData _reviveData;

    [SerializeField] private TextMeshProUGUI _zoneText;
    [SerializeField] private TextMeshProUGUI _levelText;

    [SerializeField] private VideoPlayer _videoPlayer;

    [SerializeField] private Animator _spinResultAnimator;
    [SerializeField] private Animator _spinResultFade;

    [SerializeField] private float _adDisapearDuration = 1.0f;

    // Each index considered as a different E_Scene element
    [SerializeField] private string[] _textForScene;
 
    private Button _leaveButton;
    private Button _continueButton;
    private Button _giveUpButton;
    private Button _goldButton;
    private Button _cashButton;
    private Button _watchAdButton;

    private TextMeshProUGUI _goldPriceText;
    private TextMeshProUGUI _cashPriceText;

    private RawImage _adImage;

    private int goldCost;
    private int cashCost;

    protected override void Start()
    {
        base.Start();

        _leaveButton.onClick.AddListener(Leave);
        _giveUpButton.onClick.AddListener(GiveUp);
        _continueButton.onClick.AddListener(Continue);
        _goldButton.onClick.AddListener(ReviveGold);
        _cashButton.onClick.AddListener(ReviveWithCash);
        _watchAdButton.onClick.AddListener(WatchAd);


        _zoneText.text = default;

        GameManager.Instance.onZoneChanged += OnZoneChanged;
        GameManager.Instance.OnLevelChanged += OnLevelChanged;
        GameManager.Instance.onSpinResult += OnSpinResult;


    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if(_leaveButton)
        { 
            _leaveButton.onClick.RemoveListener(Leave);
        }

        if (_giveUpButton)
        { 
            _giveUpButton.onClick.RemoveListener(GiveUp);
        }

        if (_continueButton)
        { 
            _continueButton.onClick.RemoveListener(Continue);
        }

        if (_goldButton)
        { 
            _goldButton.onClick.RemoveListener(ReviveGold);
        }

        if (_cashButton)
        {
            _cashButton.onClick.RemoveListener(ReviveWithCash);
        }

        if (_watchAdButton)
        {
            _watchAdButton.onClick.RemoveListener(WatchAd);
        }

        if (GameManager.Instance)
        {
            GameManager.Instance.onSpinResult -= OnSpinResult;
            GameManager.Instance.OnLevelChanged -= OnLevelChanged;
            GameManager.Instance.onZoneChanged -= OnZoneChanged;
        }
    }

    protected override void AssignButtons()
    {
        base.AssignButtons();

        if (!_leaveButton)
        {
            _leaveButton = GetComponentsInChildren<Button>(true).FirstOrDefault(b => b.name == "LeaveButton");
        }

        if (!_continueButton)
        {
            _continueButton = GetComponentsInChildren<Button>(true).FirstOrDefault(b => b.name == "ContinueButton");
        }

        if (!_giveUpButton)
        {
            _giveUpButton = GetComponentsInChildren<Button>(true).FirstOrDefault(b => b.name == "GiveUpButton");
        }

        if (!_goldButton)
        {
            _goldButton = GetComponentsInChildren<Button>(true).FirstOrDefault(b => b.name == "ReviveWithGoldButton");
            if (_goldButton)
            {
                _goldPriceText = _goldButton.GetComponentsInChildren<TextMeshProUGUI>(true).FirstOrDefault(b => b.name == "PriceText");
            }
        }

        if (!_cashButton)
        {
            _cashButton = GetComponentsInChildren<Button>(true).FirstOrDefault(b => b.name == "ReviveWithCashButton");
            if (_cashButton)
            {
                _cashPriceText = _cashButton.GetComponentsInChildren<TextMeshProUGUI>(true).FirstOrDefault(b => b.name == "PriceText");
            }
        }

        if (!_watchAdButton)
        {
            _watchAdButton = GetComponentsInChildren<Button>(true).FirstOrDefault(b => b.name == "ReviveWithAdButton");
        }

        _adImage = _videoPlayer.gameObject.GetComponent<RawImage>();
    }

    public void OnSpinResult(ItemData itemData)
    {
        if (itemData._itemName != "Bomb")
        {
            _winRoot.SetActive(true);
            _loseRoot.SetActive(false);
            _itemText.text = "+" + itemData._itemCount.ToString() + " " + itemData._itemName;
            _itemIcon.sprite = itemData._itemIcon;

            _continueButton.interactable = true;
            _leaveButton.interactable = GameManager.Instance.CurrentZone != Zone.Regular;
        }
        else
        {
            _winRoot.SetActive(false);
            _loseRoot.SetActive(true);

            goldCost = (int)(_reviveData.GoldReviveLevelMult * _reviveData.BaseGoldRevivePrice * GameManager.Instance.Level);
            cashCost = (int)(_reviveData.CashReviveLevelMult * _reviveData.BaseCashRevivePrice * GameManager.Instance.Level);

            _goldPriceText.text = goldCost.ToString();
            _cashPriceText.text = cashCost.ToString();

            _goldButton.interactable = Inventory.Instance.ItemMap["Gold"]._itemCount >= goldCost;
            _cashButton.interactable = Inventory.Instance.ItemMap["Cash"]._itemCount >= cashCost;
            _giveUpButton.interactable = true;
            _watchAdButton.interactable = true;
        }

        _buttonShowInventory.interactable = true;

        _spinResultFade.Play("FadeIn");
        _spinResultFade.Update(0f);
    }

    void DeactivateWinButtons()
    {
        _leaveButton.interactable = false;
        _continueButton.interactable = false;
        _buttonShowInventory.interactable = false;
    }

    void DeactivateLoseButtons()
    {
        _goldButton.interactable = false;
        _cashButton.interactable = false;
        _giveUpButton.interactable = false;
        _watchAdButton.interactable = false;
        _buttonShowInventory.interactable = false;
    }

    IEnumerator FadeOut(Action func)
    {
        _spinResultFade.Play("FadeOut");
        _spinResultFade.Update(0f);
        yield return new WaitForSeconds(1.15f);
        func();
    }

    void Leave()
    {
        DeactivateWinButtons();
        
        Action lambda = () =>
        {
            GameManager.Instance.ResetGameState(true);
        };

        ChangeScene(E_Scene.MainMenu, lambda);
    }

    void GiveUp()
    {
        DeactivateLoseButtons();

        Action lambda = () =>
        {
            GameManager.Instance.GiveUp();
        };
        
        ChangeScene(E_Scene.MainMenu, lambda);
    }

    void ReviveGold()
    {
        ReviveWithCurrency("Gold", goldCost);
    }

    void ReviveWithCash()
    {
        ReviveWithCurrency("Cash", cashCost);
    }

    void ReviveWithCurrency(string name, int price)
    {
        ItemData item = new ItemData();
        item._itemName = name;
        item._itemCount = -price;
        Inventory.Instance.IncrementItem(item);
        DeactivateLoseButtons();
        Continue();
    }

    void Continue()
    {
        DeactivateWinButtons();
        GameManager.Instance.Continue();

        Action lambda = () =>
        {
            _buttonSpin.interactable = true;
        };

        StartCoroutine(FadeOut(lambda));
    }

    void WatchAd()
    {
        DeactivateLoseButtons();
        Action lambda = () =>
        {
            Color color = _adImage.color;
            color.a = 1.0f;
            _adImage.color = color;

            RenderTexture.active = _videoPlayer.targetTexture;
            GL.Clear(true, true, Color.black);
            RenderTexture.active = null;

            AudioManager.Instance.AdjustMusic(false);
            _videoPlayer.Play();
            StartCoroutine(AfterVideo((float)_videoPlayer.length));
        };

        StartCoroutine(FadeOut(lambda));
    }

    IEnumerator AfterVideo(float length)
    {
        yield return new WaitForSeconds(length);

        Color startColor = new(_adImage.color.r, _adImage.color.g, _adImage.color.b, 1);
        Color endColor = new (_adImage.color.r, _adImage.color.g, _adImage.color.b, 0);
        float elapsed = 0.0f;

        while (_adImage.color.a > 0 && elapsed < _adDisapearDuration)
        {
            elapsed += Time.deltaTime;
            _adImage.color = Color.Lerp(startColor, endColor, elapsed / _adDisapearDuration);
            yield return null;
        }

        _adImage.color = endColor;
        GameManager.Instance.Continue();
        _buttonSpin.interactable = true;

        AudioManager.Instance.AdjustMusic(true);
    }

    void OnZoneChanged(Zone newZone)
    {
        _zoneText.text = _textForScene[(int)newZone];
    }

    void OnLevelChanged(int newLevel)
    {
        _levelText.text = "LEVEL: " + newLevel.ToString();
    }

    protected override void AdjustInventory(bool bShow)
    {
        base.AdjustInventory(bShow);

        string prefix = bShow ? "ShowInventory_" : "HideInventory_";

        _spinResultAnimator.Play(prefix + "SpinResult");
        _spinResultAnimator.Update(0f);
    }
}
