using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScritpableObjects/Data/Revive Data")]
public class ReviveData : ScriptableObject
{
    [SerializeField] private int _baseGoldRevivePrice = 50;
    [SerializeField] private float _goldReviveLevelMult = 0.1f;
    [SerializeField] private int _baseCashRevivePrice = 100;
    [SerializeField] private float _cashReviveLevelMult = 0.1f;

    public int BaseGoldRevivePrice {
        get { return _baseGoldRevivePrice; }
    }

    public float GoldReviveLevelMult
    {
        get { return _goldReviveLevelMult; }
    }

    public int BaseCashRevivePrice
    {
        get { return _baseCashRevivePrice; }
    }

    public float CashReviveLevelMult
    {
        get { return _cashReviveLevelMult; }
    }
}
