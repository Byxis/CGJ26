using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

public class MainMenu : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    public GameObject leaderBoardPanel;
    public GameObject leaderBg;
    public GameObject startBg;
    public GameObject exitBg;

    public GameObject backBg;

    public Animator animator;
    
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
                    GameObject button = eventData.pointerPressRaycast.gameObject;
                    button.SetActive(false);
                    startBg.SetActive(false);
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
        if (animator != null)
        {
            WaitAndOpenGame(1.0f);
        }
    }

    public IEnumerator WaitAndOpenGame(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        animator.SetTrigger("StartingGame");
    }

    public void OpenGame() {
        Debug.Log("Start button pressed");
        SceneManager.LoadScene("Alexis");
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
