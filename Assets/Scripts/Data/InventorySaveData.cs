using System;
using System.Collections.Generic;

[Serializable]
public class UnitSaveData
{
    public string resourceName;  // Filename in Resources/UnitCardPool to load Base keys

    // Mutable Stats
    public float currentHealth;
    public float currentSpeed;
    public float currentDamage;
    public float currentRange;
    public float currentCooldown;

    // Special
    public int clickToKill;

    // Attack Effect
    public int effectOnHit;  // Cast SpecialEffect to int
    public float effectDuration;
    public float effectValue;

    // Suicide Bomber
    public float explosionRadius;

    // Invocable
    public int numberOfInvocable;
}

[Serializable]
public class InventorySaveData
{
    public int gold;
    public List<UnitSaveData> savedUnits = new List<UnitSaveData>();
}
