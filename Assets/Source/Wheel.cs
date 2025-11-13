using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct SpinProps
{
    [Min(0f)] public float _spinMaxSpeed;
    [Min(0f)] public float _spinAccelerationDuration;
    [Min(0f)] public float _spinSlowdownDuration;
    [Min(0f)] public float _spinMinTimeToStartSlow;
    [Min(0f)] public float _spinMaxTimeToStartSlow;
}

public class Wheel : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _wheelSprite;

    [Header("Slots")]
    [SerializeField] private UI_ItemSlot[] _itemSlots;
    [SerializeField] private UI_ItemSlot _bombSlot;
    private int _currentSlotNo;

    [Header("SFX")]
    [SerializeField] private SoundClip _spinTickSound;
    [SerializeField] private SoundClip _revolverTriggerSound;
    [SerializeField] private SoundClip[] _revolverShotSounds;
    [SerializeField] private SoundClip _bombSound;
    [SerializeField] private SoundClip _prepareWheelSound;

    [Header ("Spinning Settings")]
    [SerializeField] private bool _bSpinRight = false;
    [SerializeField] private float _recoverSpeed = 100.0f;
    [SerializeField] private SpinProps[] _spinPropsArray;

    private float _spinDirectionMult = 1.0f;

    void Start()
    {
        _spinDirectionMult = _bSpinRight ? -1.0f : 1.0f;
    }

    public void PrepareWheel(ItemDataTable itemData, Zone zone, int level, bool bPlaySound = true)
    {     
        ItemData[] dataArray;
        int minRange = 1;
        float itemCountMult;
        float randomRange;
        List<UI_ItemSlot> tempItemSlots = new (_itemSlots);

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

        _wheelSprite.transform.rotation = Quaternion.identity;
        _currentSlotNo = 0;

        if (bPlaySound)
        { 
            AudioManager.Instance.PlaySoundClip(_prepareWheelSound);
        }
    }

    public void StartSpin()
    {
        Spin(0, _spinDirectionMult);
    }

    void Spin(int index, float spinDirectionMult)
    {
        SpinProps spinProps = _spinPropsArray[index];
        spinDirectionMult *= -1.0f;
        index++;

        float maxSpeed = spinDirectionMult * spinProps._spinMaxSpeed;
        float startRotation = transform.eulerAngles.z;

        Sequence seq = DOTween.Sequence();

        seq.Append(DOVirtual.Float(0, maxSpeed, spinProps._spinAccelerationDuration, speed =>
        {
            _wheelSprite.transform.Rotate(Vector3.forward, speed * Time.deltaTime);
            CheckSlot();
        }));

        seq.Append(DOVirtual.Float(maxSpeed, maxSpeed, 
            Random.Range(spinProps._spinMinTimeToStartSlow, spinProps._spinMaxTimeToStartSlow), speed =>
        {
            _wheelSprite.transform.Rotate(Vector3.forward, speed * Time.deltaTime);
            CheckSlot();
        }));
        seq.Append(DOVirtual.Float(maxSpeed, 0, spinProps._spinSlowdownDuration, speed =>
        {
            _wheelSprite.transform.Rotate(Vector3.forward, speed * Time.deltaTime);
            CheckSlot();
        }));

        seq.OnComplete(() =>
        {
            if (index < _spinPropsArray.Length)
            {
                Spin(index, spinDirectionMult);
            }
            else
            {

                AudioManager.Instance.PlaySoundClip(_revolverTriggerSound);

                float delta = Mathf.DeltaAngle(_wheelSprite.transform.eulerAngles.z, (float)_currentSlotNo * 360 / 8);
                float duration = Mathf.Abs(delta) / _recoverSpeed;

                Vector3 targetRotation = new Vector3(0, 0, _wheelSprite.transform.eulerAngles.z + delta);
                    _wheelSprite.transform.DORotate(targetRotation, duration, RotateMode.FastBeyond360).SetEase(Ease.Linear)
                    .OnComplete(() => {
                        SoundClip soundClip = _currentSlotNo == 0 && GameManager.Instance.CurrentZone == Zone.Regular ? _bombSound :
                            _revolverShotSounds[Random.Range(0, _revolverShotSounds.Length - 1)];
                        AudioManager.Instance.PlaySoundClip(soundClip);

                        UI_ItemSlot currentSlot = _currentSlotNo == 0 ? _bombSlot : _itemSlots[_currentSlotNo - 1];
                        GameManager.Instance.SpinResult(currentSlot.Item);
                    });

            }
        });
    }

    void CheckSlot()
    {
        int currentSlotNo;

        float angle = _wheelSprite.transform.rotation.eulerAngles.z;
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
            AudioManager.Instance.PlaySoundClip(_spinTickSound);
        }
    }
}