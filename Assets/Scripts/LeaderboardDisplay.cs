using LootLocker.Requests;
using TMPro;
using UnityEngine;

public class LeaderboardDisplay : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField]
    private string leaderboardKey = "top_scores";
    [SerializeField]
    private int maxEntries = 10;  // Nombre de scores à afficher

    [Header("UI References")]
    [SerializeField]
    private TextMeshProUGUI leaderboardText;
    [SerializeField]
    private GameObject loadingPanel;  // Optionnel : panel de chargement

    private void OnEnable()
    {
        // Récupérer et afficher le leaderboard quand le panel s'active
        LoadLeaderboard();
    }

    public void LoadLeaderboard()
    {
        if (leaderboardText == null)
        {
            Debug.LogError("[LeaderboardDisplay] TextMeshProUGUI non assigné !");
            return;
        }

        // Afficher un message de chargement
        leaderboardText.text = "Chargement du classement...";

        if (loadingPanel != null)
            loadingPanel.SetActive(true);

        // Récupérer les scores depuis LootLocker
        LootLockerSDKManager.GetScoreList(leaderboardKey,
                                          maxEntries,
                                          0,
                                          (response) =>
                                          {
                                              if (loadingPanel != null)
                                                  loadingPanel.SetActive(false);

                                              if (response.success)
                                              {
                                                  DisplayLeaderboard(response);
                                              }
                                              else
                                              {
                                                  leaderboardText.text =
                                                      "Erreur lors du chargement du classement.\n" + response.errorData;
                                                  Debug.LogError("[LeaderboardDisplay] Erreur : " + response.errorData);
                                              }
                                          });
    }

    private void DisplayLeaderboard(LootLockerGetScoreListResponse response)
    {
        if (response.items == null || response.items.Length == 0)
        {
            leaderboardText.text = "Aucun score enregistré pour le moment.";
            return;
        }

        // Construire le texte du leaderboard
        string leaderboardContent = "<b><size=120%>CLASSEMENT</size></b>\n\n";

        for (int i = 0; i < response.items.Length; i++)
        {
            var entry = response.items[i];
            int rank = entry.rank;
            string playerName = entry.player.name;
            int score = entry.score;

            // Formater chaque ligne
            leaderboardContent += $"<b>#{rank}</b> - {playerName} : <color=yellow>{score}</color> pts\n";
        }

        leaderboardText.text = leaderboardContent;
    }

    // Méthode publique pour rafraîchir manuellement
    public void RefreshLeaderboard()
    {
        LoadLeaderboard();
    }
}
