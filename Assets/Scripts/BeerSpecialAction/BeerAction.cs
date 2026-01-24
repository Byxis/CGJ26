using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class BeerAction : MonoBehaviour, IPointerClickHandler
{
    [Header("Dégâts")]
    public float damage = 50f;

    public Transform finalPoint;
    public float speed = 2f;
    private AnimationManager manager;
    private bool isMoving = false;
    private Vector3 originalPosition;
    private bool canClick = false;


    public void Setup(Transform target, AnimationManager mgr)
    {
        // Ignore le target passé en paramètre, on va chercher dynamiquement
        manager = mgr;
    }

    private Transform FindFurthestEnemy()
    {
        float minX = float.MaxValue;
        Transform closest = null;

        // Chercher tous les objets avec le layer TeamEnemy
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == LayerMask.NameToLayer("TeamEnemy"))
            {
                if (obj.transform.position.x < minX)
                {
                    minX = obj.transform.position.x;
                    closest = obj.transform;
                }
            }
        }

        return closest;
    }

    public void EnableClick() 
    {
        canClick = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isMoving && canClick)
        {
            // Trouve la cible dynamiquement au moment du clic
            finalPoint = FindFurthestEnemy();

            if (finalPoint == null)
            {
                Debug.LogWarning("Aucun ennemi trouvé !");
                return;
            }

            if (manager != null)
            {
                manager.CancelTimeout();
                manager.OnBeerSent();
            }

            transform.SetParent(null);
            StartCoroutine(FlyToTable());
        }
    }

    void OnDrawGizmos()
    {
        if (finalPoint) {
            Vector3 startPoint = transform.position;
            Vector3 dynamicControlPoint = (startPoint + finalPoint.position) / 2 + Vector3.up * 3.0f;

            if (finalPoint != null && dynamicControlPoint != null)
            {
                Gizmos.color = Color.white;
                Vector3 depart = transform.position;
                for (float i = 0; i <= 1; i += 0.1f)
                {
                    Vector3 m1 = Vector3.Lerp(depart, dynamicControlPoint, i);
                    Vector3 m2 = Vector3.Lerp(dynamicControlPoint, finalPoint.position, i);
                    Gizmos.DrawSphere(Vector3.Lerp(m1, m2, i), 0.05f);
                }
            }
        }
    }

    IEnumerator FlyToTable()
    {
        isMoving = true;
        float t = 0;
        Vector3 startPos = transform.position;
        // Point de contrôle pour l'effet courbe (Bézier)

        Vector3 targetPos = finalPoint.position;

        Vector3 controlPoint = (startPos + targetPos) / 2 + Vector3.up * 2.0f;

        

        while (t < 1.0f)
        {
            t += Time.deltaTime * speed;
            // Calcul courbe de Bézier simple
            Vector3 m1 = Vector3.Lerp(startPos, controlPoint, t);
            Vector3 m2 = Vector3.Lerp(controlPoint, targetPos, t);
            transform.position = Vector3.Lerp(m1, m2, t);
            yield return null;
        }

        // Impact - Infliger les dégâts à la cible
        if (finalPoint != null)
        {
            UnitController targetUnit = finalPoint.GetComponent<UnitController>();
            if (targetUnit != null)
            {
                targetUnit.TakeDamage(damage);
                Debug.Log($"Bière inflige {damage} dégâts à {finalPoint.name}");
            }
        }

        // 4. Fin de vie
        yield return new WaitForSeconds(1f);
        if (manager != null) manager.StartNewCycle();
        Destroy(gameObject);
    }      
}
