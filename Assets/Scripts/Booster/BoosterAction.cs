using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoosterAction : MonoBehaviour, IPointerClickHandler
{
    public float lifetime = 3.0f;
    public static int clickMultiplier = 1;
    private static float bonusEndTime = 0f;
    
    // Réinitialiser au chargement du script
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()
    {
        clickMultiplier = 1;
        bonusEndTime = 0f;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
    
    void Update()
    {
        // Vérifier si le bonus doit se terminer
        if (bonusEndTime > 0f && Time.time >= bonusEndTime)
        {
            clickMultiplier = 1;
            bonusEndTime = 0f;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Activer le bonus pour 5 secondes
        clickMultiplier = 2;
        bonusEndTime = Time.time + 5f;
        
        // Désactiver le collider pour qu'on ne puisse plus cliquer
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        
        // Cacher visuellement le booster
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
        
        // Détruire le GameObject immédiatement après activation
        Destroy(gameObject, 0.1f);
    }
}
