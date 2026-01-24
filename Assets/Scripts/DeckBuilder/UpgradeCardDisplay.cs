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
            if (upgrade.usePercentage)
            {
                // Display as Percentage
                if (bonusDamageText) 
                    bonusDamageText.text = Mathf.Abs(upgrade.bonusDamage) > 0.001f ? $"{upgrade.bonusDamage * 100:0.#}% Atk" : "";
                
                if (bonusHealthText) 
                    bonusHealthText.text = Mathf.Abs(upgrade.bonusHealth) > 0.001f ? $"{upgrade.bonusHealth * 100:0.#}% HP" : "";
            }
            else
            {
                // Display as Flat
                // Using nested ternary for sign handling to avoid "+-5"
                if (bonusDamageText) 
                    bonusDamageText.text = Mathf.Abs(upgrade.bonusDamage) > 0.001f ? (upgrade.bonusDamage > 0 ? "+" : "") + $"{upgrade.bonusDamage:0.#} Atk" : "";
                
                if (bonusHealthText) 
                    bonusHealthText.text = Mathf.Abs(upgrade.bonusHealth) > 0.001f ? (upgrade.bonusHealth > 0 ? "+" : "") + $"{upgrade.bonusHealth:0.#} HP" : "";
            }
        }
    }

    // Upgrades usually don't accept drops, so we accept the default (do nothing) HandleDrop
}
