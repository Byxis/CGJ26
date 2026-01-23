using UnityEngine;

public enum UnitType
{
    Default,
    SuicideBomber,
    Healer
}

public enum SpecialEffect
{
    None,
    Poison,
    Slow,
    Knockback
}

[CreateAssetMenu(fileName = "NewUnit", menuName = "BattleSystem/UnitStats")]
public class UnitStats : ScriptableObject
{
    public string unitName;
    public float maxHealth;
    public float speed;
    public float damage;
    public float attackRange;
    public float attackCooldown;
    public float clickToSpawn;

    [Header("Special")]
    public UnitType unitType;
    public int clickToKill = 0;

    [Header("Attack effect")]
    public SpecialEffect effectOnHit;
    public float effectDuration;
    public float effectValue;

    [Header("Suicide Bomber")]
    public float explosionRadius;
    public AnimationCurve explosionDamageCurve;
}