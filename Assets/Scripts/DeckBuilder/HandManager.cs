using UnityEngine;
using System.Collections.Generic;

public class HandManager : MonoBehaviour
{
    /// <summary>
    /// Returns a list of all UnitStats currently residing in this Hand Area.
    /// This includes any runtime modifications (upgrades) applied to the specific instances.
    /// </summary>
    public void GetUnitStatsInHand()
    {
        List<UnitStats> currentStats = new List<UnitStats>();

        foreach (Transform child in transform)
        {
            // We look for BaseCardDisplay, which holds the runtime data
            BaseCardDisplay display = child.GetComponent<BaseCardDisplay>();

            // We only care about UnitStats (ignoring UpgradeCards or cosmetic items)
            if (display != null && display.CardData is UnitStats stats)
            {
                currentStats.Add(stats);
            }
        }

        FindFirstObjectByType<Inventaire>().OnShopLeave(currentStats);    
    }
}
