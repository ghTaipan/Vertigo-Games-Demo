using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Zone : MonoBehaviour
{
    
    [SerializeField] private UI_ZoneSettings _zoneSettings;
    [SerializeField]  private Image zoneImage;

    void Start()
    {
        zoneImage.sprite = _zoneSettings.Regular;
        GameManager.Instance.onZoneChanged += OnZoneChanged;
    }

    private void OnDestroy()
    {
        GameManager.Instance.onZoneChanged -= OnZoneChanged;
    }

    void OnZoneChanged(Zone newZone)
    {
        switch (newZone)
        {
            case Zone.Super:
                zoneImage.sprite = _zoneSettings.Super;
                break;
            case Zone.Safe:
                zoneImage.sprite = _zoneSettings.Safe;
                break;
            default:
                zoneImage.sprite = _zoneSettings.Regular;
                break;

        }
    }
}
