using UnityEngine;

public class FinDePartie : MonoBehaviour 
{
    // Glisse l'objet qui contient le script LootLockerLeaderboard ici dans l'inspecteur
    public SaveDataBase leaderboardManager;

    public void Start()
    {
        int scoreFinal = 1500;
        string nomJoueur = "Justin";

        // Appel de ta fonction publique
        leaderboardManager.EnvoyerScore(nomJoueur, scoreFinal);
    }
}