using UnityEngine;
using TMPro;

public class UpgradeCardDisplay : BaseCardDisplay
{
    [Header("Upgrade UI")]
    public TextMeshProUGUI bonusDamageText;
    public TextMeshProUGUI bonusHealthText;
    // Add more if you want to display all bonuses

    protected override void RefreshUI()
    {
        base.RefreshUI();

        if (_cardData is UpgradeCard upgrade)
        {
            // Only show if bonus > 0, for example
            if (bonusDamageText) 
                bonusDamageText.text = upgrade.bonusDamage != 0 ? $"+{upgrade.bonusDamage} Atk" : "";
            
            if (bonusHealthText) 
                bonusHealthText.text = upgrade.bonusHealth != 0 ? $"+{upgrade.bonusHealth} HP" : "";
        }
    }

    // Upgrades usually don't accept drops, so we accept the default (do nothing) HandleDrop
}
