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
    [SerializeField]
    private GameObject m_enemyBase;
    [SerializeField]
    private GameObject m_enemySpawnPoint;

    [SerializeField]
    private GameObject m_playerBase;
    [SerializeField]
    private GameObject m_playerSpawnPoint;

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
        
        // Réinitialiser le multiplicateur de clicks à chaque nouvelle partie
        BoosterAction.clickMultiplier = 1;

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
        ResetAndStartLevel(0);
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
                if (current > total)
                {
                    m_levelText.text = $"Mode infini, niveau {current} / ∞";
                }
                else
                {
                    m_levelText.text = $"Niveau {current} / {total}";
                }
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
        if (OpponentBehavior.Instance != null)
        {
            ResetAndStartLevel(OpponentBehavior.Instance.currentLevelIndex + 1);
        }
    }

    private void ResetAndStartLevel(int levelIndex)
    {
        UnitController[] units = Object.FindObjectsByType<UnitController>(FindObjectsSortMode.None);
        foreach (var unit in units)
        {
            Destroy(unit.gameObject);
        }

        GameObject pBase =
            Instantiate(m_playerBase, m_playerSpawnPoint.transform.position, m_playerSpawnPoint.transform.rotation);
        pBase.layer = LayerMask.NameToLayer("TeamPlayer");

        GameObject eBase =
            Instantiate(m_enemyBase, m_enemySpawnPoint.transform.position, m_enemySpawnPoint.transform.rotation);
        eBase.layer = LayerMask.NameToLayer("TeamEnemy");

        if (OpponentBehavior.Instance != null)
        {
            OpponentBehavior.Instance.opponentBase = eBase.transform;
            OpponentBehavior.Instance.StartLevel(levelIndex);
        }

        if (m_endGameMenu != null)
            m_endGameMenu.SetActive(false);

        State = GameState.Playing;
        Time.timeScale = 1f;
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
