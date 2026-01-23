using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    public GameObject leaderBoardPanel;
    public GameObject leaderBg;
    public GameObject startBg;
    public GameObject exitBg;

    public GameObject backBg;
    
    private string currentButtonName = "";

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
                    bvrir le sujet en direct sur Twitch et commencer votre Brainstorming d'équipe sur Discord.reak;
                default:
                    Debug.Log("Unknown button clicked: " + buttonName);
                    break;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            string buttonName = eventData.pointerCurrentRaycast.gameObject.name;
            currentButtonName = buttonName;

            switch (buttonName)
            {
                case "StartButton":
                    startBg.SetActive(true);
                    break;
                case "LeaderBoardButton":
                    leaderBg.SetActive(true);
                    break;
                case "ExitButton":
                    exitBg.SetActive(true);
                    break;
                case "BackButton":
                    backBg.SetActive(true);
                    break;
                default:
                    Debug.Log("Unknown button clicked: " + buttonName);
                    break;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("leaving");
        string buttonName = currentButtonName;

        switch (buttonName)
        {
            case "StartButton":
                startBg.SetActive(false);
                break;
            case "LeaderBoardButton":
                leaderBg.SetActive(false);
                break;
            case "ExitButton":
                exitBg.SetActive(false);
                break;
            case "BackButton":
                backBg.SetActive(false);
                break;
            default:
                Debug.Log("Unknown button clicked: " + buttonName);
                break;
        }
        currentButtonName = "";
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
