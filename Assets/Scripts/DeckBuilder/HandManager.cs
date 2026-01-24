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
        Debug.Log("HandManager: GetUnitStatsInHand called (Redirecting to DeckManager).");

        // 1. CRITICAL FIX: Prioritize the DeckManager used by ShopManager (the one that received bought cards)
        ShopManager shopManager = FindFirstObjectByType<ShopManager>(FindObjectsInactive.Include);
        DeckManager deckManager = null;

        if (shopManager != null && shopManager.deckManager != null)
        {
            Debug.Log(
                $"HandManager: Found ShopManager. Using its DeckManager [{shopManager.deckManager.GetInstanceID()}]");
            deckManager = shopManager.deckManager;
        }
        else
        {
            // Fallback: Try finding global DeckManager (even if inactive)
            Debug.LogWarning("HandManager: ShopManager not found! searching globally...");
            deckManager = FindFirstObjectByType<DeckManager>(FindObjectsInactive.Include);
        }

        if (deckManager != null)
        {
            Debug.Log("HandManager: Found DeckManager. Delegating save...");
            deckManager.SaveToInventory();
        }
        else
        {
            Debug.LogError("HandManager: CRITICAL - DeckManager NOT FOUND!");
        }
    }
}
