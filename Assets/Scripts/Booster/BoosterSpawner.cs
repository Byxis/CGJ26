using UnityEngine;

public class BoosterSpawner : MonoBehaviour
{
   public GameObject boosterPrefab;
    public float checkInterval = 25f;
    [Range(0, 100)]
    public int spawnChance = 10;
    
    [Header("Marges depuis les bords (optionnel)")]
    public float marginX = 0.5f;  // Marge à gauche/droite
    public float marginY = 0.5f;  // Marge en haut/bas
    
    private Camera mainCamera;
    
    void Start()
    {   
        InvokeRepeating("TrySpawnBooster", checkInterval, checkInterval);
    }

    void TrySpawnBooster()
    {
        //int roll = Random.Range(0, 100);
        //if (roll < spawnChance)
        //{
            Spawn();
        //}
    }

    void Spawn()
    {
        if (mainCamera == null) return;
        
        // Calculer les limites de la caméra en coordonnées monde
        float camHeight = mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;
        
        // Générer une position dans les limites de la caméra avec marges
        float x = Random.Range(-camWidth + marginX, camWidth - marginX);
        float y = Random.Range(-camHeight + marginY, camHeight - marginY);
        Vector3 spawnPos = new Vector3(x, y, 0);

        Instantiate(boosterPrefab, spawnPos, Quaternion.identity);
        Debug.Log("Un Booster est apparu à la position : " + spawnPos);
    }
}
