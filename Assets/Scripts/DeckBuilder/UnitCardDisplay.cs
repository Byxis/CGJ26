using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class UnitCardDisplay : BaseCardDisplay
{
    [Header("Unit UI")]
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI rangeText;
    public TextMeshProUGUI cdText;
    public TextMeshProUGUI speedText;

    protected override void RefreshUI()
    {
        base.RefreshUI();

        if (_cardData is UnitStats unit)
        {
            if(damageText) damageText.text = unit.damage.ToString("0.#");
            if(healthText) healthText.text = unit.maxHealth.ToString("0.#");
            if(rangeText) rangeText.text = unit.attackRange.ToString("0.#");
            if(cdText) cdText.text = unit.attackCooldown.ToString("0.#");
            if(speedText) speedText.text = unit.speed.ToString("0.#");
        }
    }

    // Handle dropping Upgrades onto this Unit
    protected override void HandleDrop(PointerEventData eventData)
    {
        GameObject droppedObj = eventData.pointerDrag;
        if (droppedObj == null) return;

        // Check if dropped object is a Card Display
        BaseCardDisplay droppedCardDisplay = droppedObj.GetComponent<BaseCardDisplay>();
        
        // SUPPORT LEGACY/TRANSITION:
        // logic to check for UpgradeCard data type
        BaseCard droppedData = null;
        if (droppedCardDisplay != null) droppedData = droppedCardDisplay.CardData;

        if (droppedData is UpgradeCard upgrade)
        {
            // Valid Upgrade!
            // Notify Draggable
            Draggable draggable = droppedObj.GetComponent<Draggable>();
            if(draggable != null) draggable.wasDropped = true;

            Debug.Log($"Applied Upgrade {upgrade.cardName} to Unit {_cardData.cardName}");
            
            ApplyUpgrade(upgrade);

            Destroy(droppedObj);
        }
    }

    private void ApplyUpgrade(UpgradeCard upgrade)
    {
        if (_cardData is UnitStats unit)
        {
            // Direct modification (Prototype style)
            if (upgrade.usePercentage)
            {
                // Apply as RATIO (e.g., 0.1 = +10% of current value)
                unit.maxHealth += unit.maxHealth * upgrade.bonusHealth;
                unit.damage += unit.damage * upgrade.bonusDamage;
                unit.speed += unit.speed * upgrade.bonusSpeed;
                unit.attackRange += unit.attackRange * upgrade.bonusAttackRange;
                unit.attackCooldown += unit.attackCooldown * upgrade.bonusAttackCooldown; 
            }
            else
            {
                // Apply as FLAT Value
                unit.maxHealth += upgrade.bonusHealth;
                unit.damage += upgrade.bonusDamage;
                unit.speed += upgrade.bonusSpeed;
                unit.attackRange += upgrade.bonusAttackRange;
                unit.attackCooldown += upgrade.bonusAttackCooldown;
            }
            
            RefreshUI();
        }
    }
}
