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
    }
}
