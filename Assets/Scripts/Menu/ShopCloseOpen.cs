using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopCloseOpen : MonoBehaviour
{
    public void ToggleShop()
    {
        Scene shopScene = SceneManager.GetSceneByName("shop");
        
        if (!shopScene.isLoaded)
        {
            // La scène n'est pas chargée, on la charge
            SceneManager.LoadScene("shop", LoadSceneMode.Additive);
        }
        else
        {
            // La scène est chargée, on alterne la visibilité
            foreach (GameObject obj in shopScene.GetRootGameObjects())
            {
                obj.SetActive(!obj.activeSelf);
            }
        }
    }
}