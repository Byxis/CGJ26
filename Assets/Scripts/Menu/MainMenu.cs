using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour, IPointerClickHandler
{

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            string buttonName = eventData.pointerCurrentRaycast.gameObject.name;

            switch (buttonName)
            {
                case "StartButton":
                    onStartPressed();
                    break;
                case "OptionsButton":
                    onOptionsPressed();
                    break;
                case "ExitButton":
                    onExitPressed();
                    break;
                default:
                    Debug.Log("Unknown button clicked: " + buttonName);
                    break;
            }
        }
    }

    public void onStartPressed()
    {
        Debug.Log("Start button pressed");
        SceneManager.LoadScene("GameScene");
    }

    public void onOptionsPressed()
    {
        Debug.Log("Options button pressed");
    }

    public void onExitPressed()
    {
        Debug.Log("Exit button pressed");

        Application.Quit();
    }
}
