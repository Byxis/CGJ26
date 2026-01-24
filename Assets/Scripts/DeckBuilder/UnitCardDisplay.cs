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
            if(damageText) damageText.text = unit.damage.ToString();
            if(healthText) healthText.text = unit.maxHealth.ToString();
            if(rangeText) rangeText.text = unit.attackRange.ToString();
            if(cdText) cdText.text = unit.attackCooldown.ToString();
            if(speedText) speedText.text = unit.speed.ToString();
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
            unit.maxHealth += upgrade.bonusHealth;
            unit.damage += upgrade.bonusDamage;
            unit.speed += upgrade.bonusSpeed;
            unit.attackRange += upgrade.bonusAttackRange;
            unit.attackCooldown += upgrade.bonusAttackCooldown;
            
            RefreshUI();
        }
    }
}
