using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "DeckBuilder/UpgradeCard")]
public class UpgradeCard : BaseCard
{
    [Header("Stat Bonuses")]
    [Tooltip("If Use Percentage is TRUE, these values are treated as ratios (0.1 = 10%). If FALSE, they are flat values.")]
    public float bonusHealth;
    public float bonusDamage;
    public float bonusSpeed;
    public float bonusAttackRange;
    public float bonusAttackCooldown; // Negative is faster usually

    public bool usePercentage;
}
