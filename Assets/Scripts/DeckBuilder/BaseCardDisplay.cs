using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public abstract class BaseCardDisplay : MonoBehaviour, IDropHandler
{
    // Common UI (assigned in Inspector)
    [Header("Common UI")]
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI cardCostText;

    // Data
    protected BaseCard _cardData;
    public BaseCard CardData => _cardData; // Public getter for checking type

    // Setup function
    public virtual void SetCardData(BaseCard data)
    {
        _cardData = data;
        RefreshUI();
    }

    protected virtual void RefreshUI()
    {
        if (_cardData == null) return;
        if (cardNameText) cardNameText.text = _cardData.cardName;
        if (cardCostText) cardCostText.text = _cardData.cost.ToString();
    }

    // --- IDropHandler Implementation ---
    // Moved here so ANY card (Unit or Upgrade) could potentially receive a drop
    // though realistically only Units receive Upgrades, placing it here allows flexibility
    // or we could override it in UnitCardDisplay. 
    // For now, I'll put the *check* here, but the *logic* might need to be specific.
    // Actually, only UnitCards can *receive* an upgrade. UpgradeCards don't receive anything.
    // So IDropHandler fits better on UnitCardDisplay? 
    // YES. But the Base needs to be compatible with Draggable usually? No, Draggable cares about Drag. DropZone cares about Drop.
    // So UnitCardDisplay will implement IDropHandler. BaseCardDisplay won't, to avoid UpgradeCards swallowing drops they can't use.
    
    // WAIT: If I remove IDropHandler from here, I must add it to UnitCardDisplay.
    // I will NOT implement IDropHandler in Base.
    
    public void OnDrop(PointerEventData eventData) 
    {
        // Virtual drop handler? 
        // Let's implement virtual so child classes can override drop behavior.
        HandleDrop(eventData);
    }

    protected virtual void HandleDrop(PointerEventData eventData)
    {
        // By default, do nothing.
    }
}
