using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public delegate void ZoneChangedDelegate(Zone newZone);

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public ZoneChangedDelegate onZoneChanged;

    [SerializeField] private ItemDataTable _itemDataTable;
    [SerializeField] private Wheel _wheel;
    [SerializeField] private UI_SpinResult _spinResult;
    [SerializeField] private ParticleSystem _bombVFX;
    [SerializeField] private GameObject bombSpawnPoint;

    [SerializeField] private int level = 1;
    [SerializeField] private Zone currentZone;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    void Start()
    {
        if (_wheel)
        {
            _wheel.PrepareWheel(_itemDataTable, currentZone, level); 
        }

        onZoneChanged?.Invoke(currentZone);
    }

    public void SpinResult(ItemData itemData)
    {
        if (SceneLoader.Instance.CurrentScene != E_Scene.Gameplay)
        {
            StartCoroutine(TestLuck());
            return;
        }

        if (itemData._itemName != "Bomb")
        {
            Inventory.Instance.IncrementItem(itemData);
            level++;
            Zone prevZone = currentZone;
            if (level % 50 == 0)
            {
                currentZone = Zone.Super;
            }
            else if (level % 5 == 0)
            {
                currentZone = Zone.Safe;
            }
            else
            {
                currentZone = Zone.Regular;
            }       
            
            if (currentZone != prevZone)
            {
                onZoneChanged?.Invoke(currentZone);
            }
        }
        else
        {
            ParticleSystem bombVFX = Instantiate(_bombVFX, bombSpawnPoint.transform.position, bombSpawnPoint.transform.rotation);
            // Unity 2021 LTS does not have stopped and played delegates.
            bombVFX.Play();
            Destroy(bombVFX.gameObject, 3f);
        }

        _spinResult.SpinResult(itemData, currentZone);
    }

    IEnumerator TestLuck()
    {
        yield return new WaitForSeconds(2);
        _wheel.PrepareWheel(_itemDataTable, currentZone, level);
    }

    public void Continue()
    {
        _wheel.PrepareWheel(_itemDataTable, currentZone, level);
    }

    public ItemDataTable ItemDataTable
    {
        get { return _itemDataTable; }
    }

    public int Level
    {
        get { return level; }
    }

    public Zone CurrentZone
    {
        get { return currentZone; }
    }
}
