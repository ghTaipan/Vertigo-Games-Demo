using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UI_Zone : MonoBehaviour
{
    [SerializeField] private UI_ZoneSettings _zoneSettings;

    void Start()
    {
        SetSprite(_zoneSettings.Regular);
        GameManager.Instance.onZoneChanged += OnZoneChanged;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance)
        {
        GameManager.Instance.onZoneChanged -= OnZoneChanged;
        }
    }

    void OnZoneChanged(Zone newZone)
    {
        Sprite sprite;
        switch (newZone)
        {
            case Zone.Super:
                sprite = _zoneSettings.Super;
                break;
            case Zone.Safe:
                sprite = _zoneSettings.Safe;
                break;
            default:
                sprite = _zoneSettings.Regular;
                break;

        }

        SetSprite(sprite);
    }

    protected virtual void SetSprite(Sprite sprite){ }
}