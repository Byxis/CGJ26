using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Transform originalParent;
    [HideInInspector] public bool wasDropped; // <--- NEW FLAG
    
    private Vector3 originalLocalPos; 
    private Transform rootCanvas;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        // We still need this to float the card, but we won't rely on it for logic anymore
        rootCanvas = GetComponentInParent<Canvas>().transform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalLocalPos = transform.localPosition;
        
        wasDropped = false; // <--- RESET FLAG

        transform.SetParent(rootCanvas);
        transform.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // NEW CHECK: Did a DropZone sign the receipt?
        // If 'wasDropped' is true, we do NOTHING (let the card stay where it was dropped).
        if (!wasDropped)
        {
            // If false, no one caught it. Snap back.
            transform.SetParent(originalParent);
            transform.localPosition = originalLocalPos;
            transform.localScale = Vector3.one; 
        }
    }
}