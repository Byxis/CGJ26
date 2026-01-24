using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class shopButtonManager : MonoBehaviour
{

    public void SwitchScene()
    {
        Debug.Log("Exiting shop scene");
        SceneManager.UnloadSceneAsync("shop");
    }
}
