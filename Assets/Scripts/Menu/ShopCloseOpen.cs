using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopCloseOpen : MonoBehaviour
{
    private Scene shopScene;
    private bool isLoaded = false;

    public void ToggleShop()
    {
        Debug.Log("Toggling shop scene");
        if (!isLoaded)
        {
            // Première fois : on charge la scène normalement
            SceneManager.LoadScene("shop", LoadSceneMode.Additive);
            isLoaded = true;
        }
        else
        {
            // Fois suivantes : on alterne juste la visibilité
            shopScene = SceneManager.GetSceneByName("shop");
            if (shopScene.isLoaded)
            {
                foreach (GameObject obj in shopScene.GetRootGameObjects())
                {
                    // Si c'est affiché, on cache. Si c'est caché, on affiche.
                    obj.SetActive(!obj.activeSelf);
                }
            }
        }
    }
}