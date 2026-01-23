using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour, IPointerClickHandler
{

    public GameObject leaderBoardPanel;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            string buttonName = eventData.pointerCurrentRaycast.gameObject.name;

            switch (buttonName)
            {
                case "StartButton":
                if (leaderBoardPanel.activeInHierarchy)
                    {
                        break;
                    }
                    onStartPressed();
                    break;
                case "LeaderBoardButton":
                    if (leaderBoardPanel.activeInHierarchy)
                    {
                        break;
                    }
                    onLeaderBoardPressed();
                    break;
                case "ExitButton":
                    if (leaderBoardPanel.activeInHierarchy)
                    {
                        break;
                    }
                    onExitPressed();
                    break;
                case "BackButton":
                    leaderBoardPanel.SetActive(false);
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

    public void onLeaderBoardPressed()
    {
        Debug.Log("LeaderBoard button pressed");
        leaderBoardPanel.SetActive(true);
    }

    public void onExitPressed()
    {
        Debug.Log("Exit button pressed");

        Application.Quit();
    }
}
