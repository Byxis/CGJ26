using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class shopButtonManager : MonoBehaviour
{
    public void SwitchScene()
    {
        var deckManager = FindFirstObjectByType<DeckManager>();
        if (deckManager != null)
        {
            deckManager.SaveToInventory();
        }

        SceneManager.UnloadSceneAsync("shop");
    }
}
