using LootLocker.Requests;
using UnityEngine;

public class SaveDataBase : MonoBehaviour
{
    // La clé que tu as créée sur le dashboard LootLocker
    [SerializeField] private string leaderboardKey = "top_scores";

    void Start()
    {
        // Initialisation obligatoire de la session au lancement du jeu
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success) Debug.Log("[LootLocker] Session Invité démarrée.");
            else Debug.LogError("[LootLocker] Erreur session: " + response.errorData);
        });
    }

    // La fonction que tu pourras appeler depuis n'importe quel autre script
    public void EnvoyerScore(string pseudo, int score)
    {
        // 1. On définit d'abord le pseudo du joueur
        LootLockerSDKManager.SetPlayerName(pseudo, (nameResponse) =>
        {
            if (nameResponse.success)
            {
                Debug.Log("[LootLocker] Pseudo défini sur : " + pseudo);

                // 2. Une fois le nom validé, on envoie le score
                LootLockerSDKManager.SubmitScore("", score, leaderboardKey, (scoreResponse) =>
                {
                    if (scoreResponse.success)
                    {
                        Debug.Log("[LootLocker] Score envoyé avec succès : " + score);
                    }
                    else
                    {
                        // Note : on utilise .error en minuscule pour éviter l'erreur de compilation précédente
                        Debug.LogError("[LootLocker] Erreur envoi score : " + scoreResponse.errorData);
                    }
                });
            }
            else
            {
                Debug.LogError("[LootLocker] Erreur lors de la définition du pseudo : " + nameResponse.errorData);
            }
        });
    }
}