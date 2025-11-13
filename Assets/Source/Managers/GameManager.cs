using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public delegate void ZoneChangedDelegate(Zone newZone);
public delegate void LevelChangedDelegate(int newLevel);
public delegate void SpinResultDelegate(ItemData itemData);

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public ZoneChangedDelegate onZoneChanged;
    public LevelChangedDelegate OnLevelChanged;
    public SpinResultDelegate onSpinResult;

    [SerializeField] private ItemDataTable _itemDataTable;
    [SerializeField] private Wheel _wheel;

    [SerializeField] private ParticleSystem _bombVFX;
    [SerializeField] private GameObject bombSpawnPoint;

    [SerializeField] private int level = 1;
    [SerializeField] private Zone currentZone;
    [SerializeField] private bool bUseData = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        SaveData data = SaveSystem.LoadGame();
        if (bUseData && data != null && data.bSafe)
        {
            SetLevel(data.level);
            SetZone();
        }
    }

    void Start()
    {
        if (_wheel)
        {
            _wheel.PrepareWheel(_itemDataTable, currentZone, level, false); 
        }

        StartCoroutine(BroadcastGameState());
    }

    IEnumerator BroadcastGameState()
    {
        yield return new WaitForSeconds(0.2f);
        onZoneChanged?.Invoke(currentZone);
        OnLevelChanged?.Invoke(level);
    }

    public void StartSpin()
    {
        SaveGame();
        _wheel.StartSpin();
    }
    public void SpinResult(ItemData itemData)
    {
        if (SceneLoader.Instance.CurrentScene != E_Scene.Gameplay)
        {
            StartCoroutine(TestLuck(itemData));
            return;
        }

        if (itemData._itemName != "Bomb")
        {
            SetLevel(level + 1, false);
            SetZone();
            // this calls save game as well
            Inventory.Instance.IncrementItem(itemData);
        }
        else
        {
            ParticleSystem bombVFX = Instantiate(_bombVFX, bombSpawnPoint.transform.position, bombSpawnPoint.transform.rotation);
            bombVFX.Play();
            // Unity 2021 LTS does not have stopped and played delegates.
            Destroy(bombVFX.gameObject, 3f);
        }

        onSpinResult?.Invoke(itemData);
    }

    IEnumerator TestLuck(ItemData itemData)
    {
        yield return new WaitForSeconds(2);
        onSpinResult?.Invoke(itemData);
        _wheel.PrepareWheel(_itemDataTable, currentZone, level);
    }

    public void Continue()
    {
        OnLevelChanged?.Invoke(level);
        SetZone();
        SaveGame();
        _wheel.PrepareWheel(_itemDataTable, currentZone, level);
    }

    public void SaveGame(bool bSafe = false)
    {
        SaveSystem.SaveGame(new (Inventory.Instance.ItemMap, level, bSafe));
    }

    public void GiveUp()
    {
        Inventory.Instance.ResetInventory();
        ResetGameState();
    }

    public void ResetGameState(bool bSafe = false)
    {
        level = 1;
        SetLevel(1);
        SetZone();
        SaveGame(bSafe);
    }

    void SetZone()
    {
        Zone prevZone = currentZone;

        if (level % 30 == 0)
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

    void SetLevel(int newLevel, bool bBroadcast = true)
    {
        level = newLevel;
        if (bBroadcast)
        {
            OnLevelChanged?.Invoke(level);
        }
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
