using UnityEngine;

public class BoosterSpawner : MonoBehaviour
{
   public GameObject boosterPrefab;
    public float checkInterval = 5f;
    [Range(0, 100)]
    public int spawnChance = 10;
    public Vector2 spawnRangeX = new Vector2(-6, 6);
    public Vector2 spawnRangeY = new Vector2(-3, 3);
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
        float x = Random.Range(spawnRangeX.x, spawnRangeX.y);
        float y = Random.Range(spawnRangeY.x, spawnRangeY.y);
        Vector3 spawnPos = new Vector3(x, y, 0);

        Instantiate(boosterPrefab, spawnPos, Quaternion.identity);
        Debug.Log("Un Booster est apparu !");
    }
}
