using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    [Header("Settings")]
    public int maxCapacity = 20; 
    public string zoneName = "Zone";

    public void OnDrop(PointerEventData eventData)
    {
        GameObject draggedCard = eventData.pointerDrag;

        if (draggedCard != null)
        {
            Draggable draggable = draggedCard.GetComponent<Draggable>();
            
            if (draggable != null)
            {
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