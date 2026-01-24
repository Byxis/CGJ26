using UnityEngine;

public enum UnitType
{
    Default,
    SuicideBomber,
    Healer,
    Jumping,
    Base,
    Pusher,
    Summoner
}

public enum SpecialEffect
{
    None,
    Poison,
    Slow,
    Knockback
}

[CreateAssetMenu(fileName = "NewUnit", menuName = "BattleSystem/UnitStats")]
public class UnitStats : BaseCard
{
    public Sprite icon;
    public int clicksRequiredToSpawn;
    public float maxHealth;
    public float speed;
    public float damage;
    public float attackRange;
    public float attackCooldown;

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

    [Header("Ranged")]
    public GameObject projectilePrefab;

    [Header("Invocable")]
    public GameObject invocablePrefab;
    public int numberOfInvocable;
}