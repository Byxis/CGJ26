using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class shopButtonManager : MonoBehaviour
{
    public void SwitchScene()
    {
        Debug.Log("SwitchScene called - Starting Save Sequence");

        // SAVE INVENTORY (DIRECT VIA DECKMANAGER)
        var deckManager = FindFirstObjectByType<DeckManager>();
        if (deckManager != null)
        {
            Debug.Log("shopButtonManager: Found DeckManager, triggering SaveToInventory...");
            deckManager.SaveToInventory();
        }
        else
        {
            Debug.LogError("shopButtonManager: No DeckManager found to save inventory!");
        }

        Debug.Log("Exiting shop scene. Attempting to unload...");
        SceneManager.UnloadSceneAsync("shop");
        Debug.Log("UnloadSceneAsync called for 'shop'");
    }
}
