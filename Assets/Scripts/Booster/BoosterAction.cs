using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoosterAction : MonoBehaviour, IPointerClickHandler
{
    public float lifetime = 3.0f;
    public static int clickMultiplier = 1;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("BOOSTER CLIQUE ! Bonus activé - Clics x2 pendant 5 secondes !");
        
        StartCoroutine(ActivateDoubleClickBonus());
        
        Destroy(gameObject);
    }

    private IEnumerator ActivateDoubleClickBonus()
    {
        clickMultiplier = 2;
        yield return new WaitForSeconds(5f);
        clickMultiplier = 1;
        Debug.Log("Bonus terminé - Retour à la normale");
    }
}
