using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Zone : MonoBehaviour
{
    
    [SerializeField] private ZoneSettings _zoneSettings;

    private Image zoneImage;


    void Awake()
    {

        if (!_zoneSettings)
        {
            Debug.Assert(true, gameObject.name + "'s Zone Settings is not valid!");
            return;
        }

        zoneImage = GetComponent<Image>();

        if (zoneImage)
        { 
            zoneImage.sprite = _zoneSettings.Bronze;
        }
        else
        {
            Debug.Assert(true, gameObject.name + "'s Image Component is not valid!");
        }
    }
}
