using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    [Header("Settings")]
    public int maxCapacity = 20; 
    public string zoneName = "Zone";
    public bool allowUpgrades = true; // Default to true, disable for HandArea

    public void OnDrop(PointerEventData eventData)
    {
        GameObject draggedCard = eventData.pointerDrag;

        if (draggedCard != null)
        {
            Draggable draggable = draggedCard.GetComponent<Draggable>();
            
            if (draggable != null)
            {
                // Check if UpgradeCards are allowed here
                if (!allowUpgrades)
                {
                     BaseCardDisplay display = draggedCard.GetComponent<BaseCardDisplay>();
                     if (display != null && display.CardData is UpgradeCard)
                     {
                         Debug.Log($"{zoneName} does not accept Upgrades.");
                         return; 
                     }
                }

                if (transform.childCount >= maxCapacity)
                {
                    Debug.Log($"{zoneName} is Full!");
                    return; 
                }

                Debug.Log($"Dropped card into {zoneName}");
                
                // 1. UPDATE PARENT
                draggable.originalParent = this.transform;
                draggedCard.transform.SetParent(this.transform);
                
                // 2. SIGN THE RECEIPT (Tell Draggable we caught it)
                draggable.wasDropped = true; 
            }
        }
    }
}