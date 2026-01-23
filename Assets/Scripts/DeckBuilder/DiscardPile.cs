using UnityEngine;
using UnityEngine.EventSystems;

public class DiscardBin : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        // 1. Identify what was dropped
        GameObject droppedCard = eventData.pointerDrag;

        // 2. Safety check: Is it actually a card?
        if (droppedCard != null)
        {
            Debug.Log("Card destroyed: " + droppedCard.name);
            
            // 3. DESTROY IT!
            Destroy(droppedCard);
        }
    }
}