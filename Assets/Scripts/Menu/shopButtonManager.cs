using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class shopButtonManager : MonoBehaviour
{

    public void SwitchScene()
    {
        Debug.Log("SwitchScene called");
        Debug.Log("Exiting shop scene");
        Debug.Log("Attempting to unload 'shop' scene");
        SceneManager.UnloadSceneAsync("shop");
        Debug.Log("UnloadSceneAsync called for 'shop'");
    }
}
