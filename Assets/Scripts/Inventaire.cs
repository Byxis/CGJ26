using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventaire : MonoBehaviour
{

    public List<UnitStats> UnitStats;
    public UnitStats fourmi;
    void Start()
    {
        UnitStats = new List<UnitStats>
        {
            fourmi
        };
    }

    // Update is called once per frame
    public void OnShopLeave(List<UnitStats> us)
    {
        UnitStats.Clear();

        if (us.Count == 0 || us == null)
        {
            UnitStats.Add(fourmi);
        }

        foreach (UnitStats unit in us)
        {
            UnitStats.Add(unit);
            Debug.Log(unit);
        }

    }
}
