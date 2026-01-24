using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventaire : MonoBehaviour
{
    public static Inventaire Instance { get; private set; }

    public List<UnitData> UnitStats;
    public UnitData fourmi;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (UnitStats == null)
            {
                UnitStats = new List<UnitData>();  // Initialize list
            }

            LoadInventory();  // Try loading

            if (UnitStats.Count == 0)
            {
                UnitStats.Add(fourmi);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start() {}

    // Update is called once per frame
    public void OnShopLeave(List<UnitData> us)
    {
        UnitStats.Clear();

        if (us == null || us.Count == 0)
        {
            UnitStats.Add(fourmi);
        }
        else
        {
            foreach (UnitData unit in us)
            {
                UnitStats.Add(unit);
            }
        }

        SaveInventory();
    }

    public int currentGold = 2;

    public void ApplyVictoryBonus()
    {
        currentGold += 1;
        SaveInventory();
    }

    private void SaveInventory()
    {
        InventorySaveData saveData = new InventorySaveData();
        saveData.gold = currentGold;  // Save Gold

        foreach (var unit in UnitStats)
        {
            if (unit == null)
                continue;

            UnitSaveData data = new UnitSaveData();
            data.resourceName = unit.name.Replace("(Clone)", "").Trim();

            if (unit.currentStats != null)
            {
                data.currentHealth = unit.currentStats.maxHealth;
                data.currentDamage = unit.currentStats.damage;
                data.currentSpeed = unit.currentStats.speed;
                data.currentRange = unit.currentStats.attackRange;
                data.currentCooldown = unit.currentStats.attackCooldown;

                // Extended Stats
                data.clickToKill = unit.currentStats.clickToKill;
                data.effectOnHit = (int)unit.currentStats.effectOnHit;
                data.effectDuration = unit.currentStats.effectDuration;
                data.effectValue = unit.currentStats.effectValue;
                data.explosionRadius = unit.currentStats.explosionRadius;
                data.numberOfInvocable = unit.currentStats.numberOfInvocable;
            }

            saveData.savedUnits.Add(data);
        }

        string json = JsonUtility.ToJson(saveData, true);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/inventory.json", json);
        Debug.Log("Inventaire: Saved to " + Application.persistentDataPath + "/inventory.json");
    }

    private void LoadInventory()
    {
        string path = Application.persistentDataPath + "/inventory.json";
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            InventorySaveData saveData = JsonUtility.FromJson<InventorySaveData>(json);

            if (saveData != null)
            {
                currentGold = saveData.gold;  // Load Gold

                if (saveData.savedUnits != null && saveData.savedUnits.Count > 0)
                {
                    UnitStats.Clear();
                    foreach (var data in saveData.savedUnits)
                    {
                        // 1. Load Base Asset
                        UnitData original = Resources.Load<UnitData>("UnitCardPool/" + data.resourceName);
                        if (original == null)
                        {
                            Debug.LogError("Inventaire: Could not find resource 'UnitCardPool/" + data.resourceName +
                                           "'");
                            continue;
                        }

                        // 2. Instantiate Runtime Copy
                        UnitData runtimeCopy = Instantiate(original);
                        runtimeCopy.name = original.name;  // Clean name

                        // 3. Instantiate Stats
                        // We need a base to clone from.
                        // If original has currentStats (unlikely for asset), use it. Else use Prefab.

                        UnitStats statsToClone = original.currentStats;
                        if (statsToClone == null && original.unitPrefab != null)
                        {
                            var ctrl = original.unitPrefab.GetComponent<UnitController>();
                            if (ctrl != null)
                                statsToClone = ctrl.Stats;
                        }

                        if (statsToClone != null)
                        {
                            runtimeCopy.currentStats = Instantiate(statsToClone);
                            // 4. Apply Saved Values
                            runtimeCopy.currentStats.maxHealth = data.currentHealth;
                            runtimeCopy.currentStats.damage = data.currentDamage;
                            runtimeCopy.currentStats.speed = data.currentSpeed;
                            runtimeCopy.currentStats.attackRange = data.currentRange;
                            runtimeCopy.currentStats.attackCooldown = data.currentCooldown;

                            // Extended Stats
                            runtimeCopy.currentStats.clickToKill = data.clickToKill;
                            runtimeCopy.currentStats.effectOnHit = (SpecialEffect)data.effectOnHit;
                            runtimeCopy.currentStats.effectDuration = data.effectDuration;
                            runtimeCopy.currentStats.effectValue = data.effectValue;
                            runtimeCopy.currentStats.explosionRadius = data.explosionRadius;
                            runtimeCopy.currentStats.numberOfInvocable = data.numberOfInvocable;
                        }
                        else
                        {
                            Debug.LogError($"Inventaire: Failed to find base stats for {data.resourceName}");
                        }

                        UnitStats.Add(runtimeCopy);
                    }
                    Debug.Log($"Inventaire: Loaded {UnitStats.Count} units from save.");
                }
            }
            else
            {
                Debug.Log("Inventaire: No save file found.");
            }
        }
    }
}
