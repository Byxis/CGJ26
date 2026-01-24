using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    [SerializeField] private GameObject m_pauseMenu;

    public void unPauseGame()
    {
        if (GameManager.Instance.State == GameState.Paused)
        {
            GameManager.Instance.State = GameState.Playing;
            Time.timeScale = 1f;
            if (m_pauseMenu == null)
                m_pauseMenu = GameObject.FindGameObjectWithTag("PausePanel");
            if (m_pauseMenu != null)
            {
                m_pauseMenu.SetActive(false);
            }
        }
    }

    public void pauseGame()
    {
        UnityEngine.Debug.Log("Pause game");
        if (GameManager.Instance.State == GameState.Playing)
        {
            GameManager.Instance.State = GameState.Paused;
            Time.timeScale = 0f;

            if (m_pauseMenu == null)
                m_pauseMenu = GameObject.FindGameObjectWithTag("PausePanel");

            UnityEngine.Debug.Log("Pause menu found: " + (m_pauseMenu != null));

            if (m_pauseMenu != null)
            {
                m_pauseMenu.SetActive(true);
            }
        }
    }

    public void quitGame()
    {
        GameManager.Instance.QuitGame();
    }
}
