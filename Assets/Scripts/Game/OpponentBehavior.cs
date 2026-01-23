using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct UnitSpawnEntry
{
    public UnitData unitData;
    [Range(0f, 100f)]
    public float spawnWeight;
}

[System.Serializable]
public struct OpponentLevelConfig
{
    public int minLevel;
    public int maxLevel;
    public float clickGenerationSpeed;
    public float clickRandomness;
    public float clickRandomFrequency;
    public List<UnitSpawnEntry> deck;
}

public class OpponentBehavior : MonoBehaviour
{
    [Header("Configuration")]
    public Transform spawnPoint;
    public Transform opponentBase;
    public List<OpponentLevelConfig> levelConfigs;

    [Header("Debug / State")]
    public int currentLevelIndex = 0;
    public float accumulatedClicks = 0f;
    public UnitData currentTargetUnit;

    private bool isLevelActive = true;
    private float noiseOffset;

    void Start()
    {
        noiseOffset = Random.Range(0f, 100f);

        if (levelConfigs == null || levelConfigs.Count == 0)
        {
            Debug.LogError("Aucun niveau configuré pour l'OpponentBehavior !");
            isLevelActive = false;
            return;
        }

        StartLevel(0);
    }

    void StartLevel(int index)
    {
        currentLevelIndex = index;
        accumulatedClicks = 0f;
        PickNextUnitToSpawn();
    }

    void FixedUpdate()
    {
        if (!isLevelActive)
            return;

        if (opponentBase == null)
        {
            // TODO: Logique de changement de niveau ou Game Over
            isLevelActive = false;
            return;
        }

        OpponentLevelConfig currentConfig = GetConfigForLevel(currentLevelIndex);

        float noise = Mathf.PerlinNoise((Time.time * currentConfig.clickRandomFrequency) + noiseOffset, 0f);
        float varianceMultiplier = (noise - 0.5f) * 2f;

        float currentSpeed =
            Mathf.Max(0f, currentConfig.clickGenerationSpeed + (varianceMultiplier * currentConfig.clickRandomness));

        accumulatedClicks += currentSpeed * Time.deltaTime;

        if (currentTargetUnit != null)
        {
            if (accumulatedClicks >= currentTargetUnit.clicksRequiredToSpawn)
            {
                SpawnUnit(currentTargetUnit);
                accumulatedClicks -= currentTargetUnit.clicksRequiredToSpawn;
                PickNextUnitToSpawn();
            }
        }
    }

    void SpawnUnit(UnitData data)
    {
        if (data.unitPrefab == null)
            return;
        if (spawnPoint == null)
            return;

        GameObject newUnit = Instantiate(data.unitPrefab, spawnPoint.position, Quaternion.identity);
        newUnit.layer = LayerMask.NameToLayer("TeamEnemy");
    }

    void PickNextUnitToSpawn()
    {
        OpponentLevelConfig currentConfig = GetConfigForLevel(currentLevelIndex);
        if (currentConfig.deck == null || currentConfig.deck.Count == 0)
        {
            currentTargetUnit = null;
            return;
        }

        float totalWeight = 0f;
        foreach (var entry in currentConfig.deck)
        {
            totalWeight += entry.spawnWeight;
        }

        float randomValue = Random.Range(0f, totalWeight);
        float cursor = 0f;

        foreach (var entry in currentConfig.deck)
        {
            cursor += entry.spawnWeight;
            if (cursor >= randomValue)
            {
                currentTargetUnit = entry.unitData;
                return;
            }
        }

        currentTargetUnit = currentConfig.deck[0].unitData;
    }

    OpponentLevelConfig GetConfigForLevel(int level)
    {
        foreach (var config in levelConfigs)
        {
            if (level >= config.minLevel && level <= config.maxLevel)
            {
                return config;
            }
        }

        Debug.LogWarning($"Aucune config trouvée pour le niveau {level}. Utilisation de la dernière config.");
        return levelConfigs[levelConfigs.Count - 1];
    }
}
