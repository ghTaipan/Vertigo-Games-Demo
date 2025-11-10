using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct SpinProps
{
    [Min(0f)] public float _spinMaxSpeed;
    [Min(0f)] public float _spinAcceleration;
    [Min(0f)] public float _spinDecayAcceleration;
    [Min(0f)] public float _spinMinTimeToStartDecay;
    [Min(0f)] public float _spinMaxTimeToStartDecay;
}
public class Wheel : MonoBehaviour
{
    [Header ("UI")]
    [SerializeField] private Button _buttonSpin;
    [SerializeField] private Image _wheelImage;

    [Header("Slots")]
    [SerializeField] private UI_ItemSlot[] _itemSlots;
    [SerializeField] private UI_ItemSlot _bombSlot;
    private int _currentSlotNo;

    [Header("SFX")]
    [SerializeField] private AudioClip _spinTickSound;
    [SerializeField] private AudioClip _revolverTriggerSound;
    [SerializeField] private AudioClip[] _revolverShotSounds;
    [SerializeField] private AudioClip _bombSound;

    [Header("Zone Settings")]
    [SerializeField] private UI_Zone _wheelZone;
    [SerializeField] private UI_Zone _indicatorZone;
    //[Header ("Zone Specific")]

    [Header ("Spinning Settings")]
    [SerializeField] private bool _bSpinRight = false;
    [SerializeField] private SpinProps _firstSpinProps;
    [SerializeField] private SpinProps _secondSpinProps;
    [SerializeField] private float _recoverSpeed = 100.0f;

    private float _spinDecayStartTime = 0.0f;
    private float _spinSpeed = 0.0f;
    private float _spinDirectionMult = 1.0f;

    void Start()
    {
        _buttonSpin.onClick.AddListener(OnSpinButtonPressed);
        _spinDirectionMult = _bSpinRight ? -1.0f : 1.0f;
    }

    void OnDestroy()
    {
        _buttonSpin.onClick.RemoveListener(OnSpinButtonPressed);
    }
    public void PrepareWheel(ItemDataTable itemData, Zone zone, int level)
    {     
        ItemData[] dataArray;
        int minRange = 1;
        float itemCountMult;
        float randomRange;
        List<UI_ItemSlot> tempItemSlots = new List<UI_ItemSlot>(_itemSlots);

        switch (zone)
        {
            case Zone.Super:
                dataArray = itemData.SpecialItems;
                minRange = 0;
                itemCountMult = itemData.SpecialItemCountByLevelMult;
                randomRange = itemData.SpecialItemRandomRange;
                tempItemSlots.Add(_bombSlot);
                break;

            case Zone.Safe:
                dataArray = itemData.RegularItems;
                itemCountMult = itemData.RegularItemCountLevelMult;
                randomRange = itemData.RegularItemRandomRange;
                tempItemSlots.Add(_bombSlot);
                break;

            default:
                _bombSlot.Item = itemData.RegularItems[0];
                dataArray = itemData.RegularItems;
                itemCountMult = itemData.RegularItemCountLevelMult;
                randomRange = itemData.RegularItemRandomRange;
                break;    
        }

        for (int i = 0; i < tempItemSlots.Count; ++i)
        {
            ItemData item = dataArray[Random.Range(minRange, dataArray.Length)];
            item._itemCount *= Mathf.Max(1, (int) Random.Range(itemCountMult - randomRange, itemCountMult + randomRange) * level);
            tempItemSlots[i].Item = item;
        }

        _buttonSpin.interactable = true;
        _wheelImage.rectTransform.rotation = Quaternion.identity;
        _currentSlotNo = 0;
    }

    void OnSpinButtonPressed()
    {
        _buttonSpin.interactable = false;
        StartCoroutine(SpinSequence());
    }

    IEnumerator SpinSequence()
    {
        yield return StartCoroutine(Spin(_firstSpinProps, true));
        yield return StartCoroutine(Spin(_secondSpinProps, false));
        yield return StartCoroutine(RecoverWheelDirection());
        yield return new WaitForSeconds(AudioManager.Instance.PlaySound(_revolverTriggerSound));

        AudioManager.Instance.PlaySound(_currentSlotNo == 0 && GameManager.Instance.CurrentZone == Zone.Regular ? _bombSound :
            _revolverShotSounds[Random.Range(0, _revolverShotSounds.Length - 1)]);

        UI_ItemSlot currentSlot = _currentSlotNo == 0 ? _bombSlot : _itemSlots[_currentSlotNo - 1];
        GameManager.Instance.SpinResult(currentSlot.Item);
    }

    IEnumerator Spin(SpinProps spinProps, bool bFirst)
    {
        float spinnedTime = 0.0f;
        float spinDirectionMult = bFirst ? _spinDirectionMult * -1.0f : _spinDirectionMult;

        _spinDecayStartTime = Random.Range(spinProps._spinMinTimeToStartDecay, spinProps._spinMaxTimeToStartDecay);
        spinnedTime = 0.0f;

        while (_spinSpeed < spinProps._spinMaxSpeed)
        {
            _spinSpeed += Time.deltaTime * spinProps._spinAcceleration;
            _wheelImage.rectTransform.Rotate(0, 0, spinDirectionMult * _spinSpeed * Time.deltaTime);
            CheckSlot();

            yield return null;
        }

        _spinSpeed = spinProps._spinMaxSpeed;

        while (_spinSpeed > 0.0f)
        {
            if (spinnedTime < _spinDecayStartTime)
            {
                spinnedTime += Time.deltaTime;
            }
            else
            {
                _spinSpeed -= spinProps._spinDecayAcceleration * Time.deltaTime;
            }

            _wheelImage.rectTransform.Rotate(0, 0, spinDirectionMult * _spinSpeed * Time.deltaTime);
            CheckSlot();

            yield return null;
        }
    }

    IEnumerator RecoverWheelDirection()
    {
        Quaternion targetRotation = Quaternion.Euler(0.0f, 0.0f, (float)_currentSlotNo * 360 / 8);
        while (Quaternion.Angle(_wheelImage.rectTransform.rotation, targetRotation) > 0.001f)
        {
            _wheelImage.rectTransform.rotation = Quaternion.RotateTowards(_wheelImage.rectTransform.rotation, targetRotation, _recoverSpeed * Time.deltaTime);
            yield return null;
        }
    }

    void CheckSlot()
    {
        int currentSlotNo;

        float angle = _wheelImage.rectTransform.rotation.eulerAngles.z;
        if (angle <= 22.5f || angle >= 337.5f)
        {
            currentSlotNo = 0;
        }
        else
        {
            currentSlotNo = ((int)(22.5f + angle) / 45);
        }

        if (_currentSlotNo != currentSlotNo)
        {
            _currentSlotNo = currentSlotNo;
            AudioManager.Instance.PlaySound(_spinTickSound);
        }
    }
}