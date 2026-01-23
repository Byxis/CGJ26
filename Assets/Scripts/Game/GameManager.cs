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

    private TMPro.TextMeshProUGUI m_endGameText;
    private UnityEngine.UI.Button m_actionButton;
    private TMPro.TextMeshProUGUI m_buttonText;

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

        if (m_endGameMenu != null)
        {
            m_actionButton = m_endGameMenu.GetComponentInChildren<UnityEngine.UI.Button>(true);
            TMPro.TextMeshProUGUI[] allTexts = m_endGameMenu.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true);

            foreach (var txt in allTexts)
            {
                string n = txt.gameObject.name.ToLower();

                // On cherche par mot-clé dans le nom de l'objet
                if (n.Contains("title") || n.Contains("titre") || n.Contains("header"))
                {
                    m_endGameText = txt;
                }
                else if (n.Contains("button") || n.Contains("btn") || n.Contains("label"))
                {
                    m_buttonText = txt;
                }
            }

            // Fallback si on n'a pas trouvé par les noms
            if (m_buttonText == null && m_actionButton != null)
            {
                m_buttonText = m_actionButton.GetComponentInChildren<TMPro.TextMeshProUGUI>(true);
            }

            if (m_endGameText == null)
            {
                foreach (var txt in allTexts)
                {
                    if (txt != m_buttonText)
                    {
                        m_endGameText = txt;
                        break;
                    }
                }
            }

            m_endGameMenu.SetActive(false);
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

            if (State == GameState.GameOver)
            {
                m_endGameText.text = "Défaite";
                m_buttonText.text = "Rejouer";
                m_actionButton.onClick.RemoveAllListeners();
                m_actionButton.onClick.AddListener(RestartLevel);
            }
            else
            {
                m_endGameText.text = "Victoire !";
                m_buttonText.text = "Continuer";
                m_actionButton.onClick.RemoveAllListeners();
                m_actionButton.onClick.AddListener(NextLevel);
            }
        }
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
}
