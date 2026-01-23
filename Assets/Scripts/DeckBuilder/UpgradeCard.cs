using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "DeckBuilder/UpgradeCard")]
public class UpgradeCard : BaseCard
{
    [Header("Stat Bonuses")]
    public int bonusHealth;
    public int bonusDamage;
    public int bonusSpeed;
    public int bonusAttackRange;
    public int bonusAttackCooldown; // Negative is faster usually, depends on logic
}
