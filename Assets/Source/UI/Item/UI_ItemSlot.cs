using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class UI_ItemSlot : MonoBehaviour
{
    protected ItemData item;

    public virtual ItemData Item {
        get { return item; }
        set 
        { 
            item = value;
        }
    }
}