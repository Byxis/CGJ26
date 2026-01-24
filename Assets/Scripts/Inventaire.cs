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
                UnitStats = new List<UnitData> { fourmi };
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Initialization moved to Awake to avoid resetting on scene reload if it were not persistent
        // But since it is persistent now, Start runs once.
    }

    // Update is called once per frame
    public void OnShopLeave(List<UnitData> us)
    {
        Debug.Log($"Inventaire: OnShopLeave called with {us?.Count ?? 0} units.");
        UnitStats.Clear();

        if (us == null || us.Count == 0)
        {
            Debug.LogWarning("Inventaire: List is empty/null! Adding default 'fourmi'.");
            UnitStats.Add(fourmi);
        }
        else
        {
            foreach (UnitData unit in us)
            {
                UnitStats.Add(unit);
                Debug.Log($"Inventaire: Saved {unit.name}");
            }
        }
    }
}
