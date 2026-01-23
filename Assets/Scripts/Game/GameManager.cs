using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Playing,
    Paused,
    Victory,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private GameObject m_endGameMenu;
    [SerializeField] private GameObject m_pauseMenu;

    private TMPro.TextMeshProUGUI m_endGameText;
    private TMPro.TextMeshProUGUI m_levelText;
    private UnityEngine.UI.Button m_actionButton;
    private TMPro.TextMeshProUGUI m_buttonText;
    private TMPro.TMP_InputField m_inputField;
    private UnityEngine.UI.Button m_quitButton;

    private SaveDataBase m_leaderboard;

    public GameState State { get; private set; } = GameState.Playing;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        m_leaderboard = FindFirstObjectByType<SaveDataBase>();

        if (m_endGameMenu != null)
        {
            UnityEngine.UI.Button[] buttons = m_endGameMenu.GetComponentsInChildren<UnityEngine.UI.Button>(true);
            foreach (var btn in buttons)
            {
                if (btn.gameObject.name.ToLower().Contains("action1"))
                    m_actionButton = btn;
                else if (btn.gameObject.name.ToLower().Contains("action2"))
                    m_quitButton = btn;
            }

            if (m_actionButton == null)
                m_actionButton = m_endGameMenu.GetComponentInChildren<UnityEngine.UI.Button>(true);

            m_inputField = m_endGameMenu.GetComponentInChildren<TMPro.TMP_InputField>(true);

            TMPro.TextMeshProUGUI[] allTexts = m_endGameMenu.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true);

            foreach (var txt in allTexts)
            {
                string n = txt.gameObject.name.ToLower();

                if (n.Contains("header"))
                {
                    m_endGameText = txt;
                }
                else if (n.Contains("action1"))
                {
                    m_buttonText = txt;
                }
                else if (n.Contains("level"))
                {
                    m_levelText = txt;
                }
            }

            if (m_buttonText == null && m_actionButton != null)
            {
                m_buttonText = m_actionButton.GetComponentInChildren<TMPro.TextMeshProUGUI>(true);
            }

            if (m_inputField != null)
            {
                m_inputField.onValueChanged.AddListener(ValidateInput);
            }

            m_endGameMenu.SetActive(false);
        }
        if (m_pauseMenu != null)
        {
            m_pauseMenu.SetActive(false);
        }
    }

    private void ValidateInput(string text)
    {
        if (m_quitButton != null)
        {
            m_quitButton.interactable = !string.IsNullOrWhiteSpace(text);
        }
    }

    public void OnBaseDestroyed(UnitController baseUnit)
    {
        if (State != GameState.Playing)
            return;

        Time.timeScale = 0f;

        bool isPlayerBase = LayerMask.LayerToName(baseUnit.gameObject.layer).Contains("Player");
        State = isPlayerBase ? GameState.GameOver : GameState.Victory;

        if (m_endGameMenu != null)
        {
            m_endGameMenu.SetActive(true);

            if (m_levelText != null && OpponentBehavior.Instance != null)
            {
                int current = OpponentBehavior.Instance.currentLevelIndex + 1;
                int total = OpponentBehavior.Instance.GetMaxLevel() + 1;
                m_levelText.text = $"Niveau {current} / {total}";
            }

            if (m_inputField != null)
            {
                ValidateInput(m_inputField.text);
            }

            if (m_quitButton != null)
            {
                m_quitButton.onClick.RemoveAllListeners();
                m_quitButton.onClick.AddListener(QuitGame);
            }

            if (State == GameState.GameOver)
            {
                m_endGameText.text = "Défaite";
                m_buttonText.text = "Rejouer";
                m_endGameText.color = Color.red;
                m_actionButton.onClick.RemoveAllListeners();
                m_actionButton.onClick.AddListener(RestartLevel);
            }
            else
            {
                m_endGameText.text = "Victoire !";
                m_buttonText.text = "Continuer";
                m_endGameText.color = Color.green;
                m_actionButton.onClick.RemoveAllListeners();
                m_actionButton.onClick.AddListener(NextLevel);
            }
        }
    }

    private void SaveAndFinish(System.Action onComplete)
    {
        if (m_leaderboard != null && m_inputField != null && !string.IsNullOrWhiteSpace(m_inputField.text))
        {
            int score = OpponentBehavior.Instance != null ? OpponentBehavior.Instance.currentLevelIndex + 1 : 0;
            m_leaderboard.EnvoyerScore(m_inputField.text, score);
        }
        onComplete?.Invoke();
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        SaveAndFinish(() =>
                      {
                          Debug.Log("Score sauvegardé. Fermeture du jeu...");
                          Application.Quit();
#if UNITY_EDITOR
                          UnityEditor.EditorApplication.isPlaying = false;
#endif
                      });
    }

    public void unPauseGame()
    {
        if (State == GameState.Paused)
        {
            State = GameState.Playing;
            Time.timeScale = 1f;
            if (m_pauseMenu != null)
            {
                m_pauseMenu.SetActive(false);
            }
        }
    }

    public void pauseGame()
    {
        if (State == GameState.Playing)
        {
            State = GameState.Paused;
            Time.timeScale = 0f;
            if (m_pauseMenu != null)
            {
                m_pauseMenu.SetActive(true);
            }
        }
    }
}
