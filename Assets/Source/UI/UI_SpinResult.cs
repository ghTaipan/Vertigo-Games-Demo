using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SpinResult : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _itemText;
    [SerializeField] private Image _itemIcon;
    [SerializeField] private GameObject _winRoot;
    [SerializeField] private GameObject _loseRoot;

    [SerializeField] private Button _leaveButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _giveUpButton;
    [SerializeField] private Button _goldButton;
    [SerializeField] private Button _cashButton;
    [SerializeField] private Button _watchAdButton;

    [SerializeField] private TextMeshProUGUI _goldPriceText;
    [SerializeField] private TextMeshProUGUI _cashPriceText;

    [SerializeField] private ReviveData _reviveData;

    private int goldCost;
    private int cashCost;

    void Start()
    {
        _leaveButton.onClick.AddListener(BackToMainMenu);
        _giveUpButton.onClick.AddListener(BackToMainMenu);
        _continueButton.onClick.AddListener(Continue);
        _goldButton.onClick.AddListener(ReviveGold);
        _cashButton.onClick.AddListener(ReviveWithCash);
        //_watchAdButton.onClick.AddListener(WatchAd);
    }

    void OnDestroy()
    {
        _leaveButton.onClick.RemoveListener(BackToMainMenu);
        _giveUpButton.onClick.RemoveListener(BackToMainMenu);
        _continueButton.onClick.RemoveListener(Continue);
        _goldButton.onClick.RemoveListener(ReviveGold);
        _cashButton.onClick.RemoveListener(ReviveWithCash);
        //_watchAdButton.onClick.RemoveListener(WatchAd);
    }
    public void SpinResult(ItemData itemData, Zone zone)
    {
        if (itemData._itemName != "Bomb")
        {
            _winRoot.SetActive(true);
            _loseRoot.SetActive(false);
            _itemText.text = "+" + itemData._itemCount.ToString() + " " + itemData._itemName;
            _itemIcon.sprite = itemData._itemIcon;

            _leaveButton.interactable = zone != Zone.Regular;
        }
        else
        {
            _winRoot.SetActive(false);
            _loseRoot.SetActive(true);

            goldCost = (int)(_reviveData.GoldReviveLevelMult * _reviveData.BaseGoldRevivePrice * GameManager.Instance.Level);
            cashCost = (int)(_reviveData.CashReviveLevelMult * _reviveData.BaseCashRevivePrice * GameManager.Instance.Level);

            _goldPriceText.text = goldCost.ToString();
            _cashPriceText.text = cashCost.ToString();

            _goldButton.interactable = Inventory.Instance.ItemMap["Gold"]._itemCount < goldCost;
            _cashButton.interactable = Inventory.Instance.ItemMap["Cash"]._itemCount < cashCost;
        }
    }

    void BackToMainMenu()
    {
        SceneLoader.LoadScene(E_Scene.MainMenu);
    }

    void ReviveGold()
    {
        ReviveWithCurrench("Gold", goldCost);
    }

    void ReviveWithCash()
    {
        ReviveWithCurrench("Cash", cashCost);
    }

    void ReviveWithCurrench(string name, int price)
    {
        ItemData item = new ItemData();
        item._itemName = name;
        item._itemCount = -price;
        Inventory.Instance.IncrementItem(item);
        Continue();
    }

    void Continue()
    {
        _leaveButton.interactable = false;
        GameManager.Instance.Continue();
    }
}
