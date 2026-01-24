using UnityEngine;
using System.Collections.Generic;

public class HandManager : MonoBehaviour
{
    /// <summary>
    /// Returns a list of all UnitData currently residing in this Hand Area.
    /// This includes any runtime modifications (upgrades) applied to the specific instances.
    /// </summary>
    public void GetUnitStatsInHand()
    {
        // 1. CRITICAL FIX: Prioritize the DeckManager used by ShopManager (the one that received bought cards)
        ShopManager shopManager = FindFirstObjectByType<ShopManager>(FindObjectsInactive.Include);
        DeckManager deckManager = null;

        if (shopManager != null && shopManager.deckManager != null)
        {
            deckManager = shopManager.deckManager;
        }
        else
        {
            // Fallback: Try finding global DeckManager (even if inactive)
            deckManager = FindFirstObjectByType<DeckManager>(FindObjectsInactive.Include);
        }

        if (deckManager != null)
        {
            deckManager.SaveToInventory();
        }
    }
}
