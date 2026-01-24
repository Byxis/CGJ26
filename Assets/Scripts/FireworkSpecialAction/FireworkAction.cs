using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class FireworkAction : MonoBehaviour, IPointerClickHandler
{
    [Header("Réglages")]
    public float speed = 1.5f;
    public float amplitudeTremblement = 0.05f;

    private Transform targetPoint;
    private FireworkCannonManager manager;
    
    private bool isMoving = false;
    private bool canClick = false;
    private Coroutine trembleCoroutine;
    public bool IsMoving()
    {
        return isMoving;
    }

    public void Setup(Transform target, FireworkCannonManager mgr)
    {
        targetPoint = target;
        manager = mgr;
    }

    public void EnableClick() 
    {
        canClick = true;
    }

    public void StartTrembling()
    {
        trembleCoroutine = StartCoroutine(TrembleRoutine());
    }

    IEnumerator TrembleRoutine()
    {
        Vector3 posInitiale = transform.localPosition;
        while (!isMoving)
        {
            transform.localPosition = posInitiale + Random.insideUnitSphere * amplitudeTremblement;
            yield return null;
        }
        transform.localPosition = posInitiale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clic détecté sur : " + gameObject.name);
        if (!isMoving && canClick)
        {
            if (manager != null)
            {
                manager.CancelTimeout();
                manager.OnFireworkSent();
            }

            if (trembleCoroutine != null)
            {
                StopCoroutine(trembleCoroutine);
            }
            transform.SetParent(null);
            StartCoroutine(FlyToTargetCurve());
        }
    }

    IEnumerator FlyToTargetCurve()
    {
        isMoving = true;
        float t = 0;
        Vector3 startPos = transform.position;
        Vector3 targetPos = targetPoint.position;

        Vector3 controlPoint = (startPos + targetPos) / 2 + Vector3.up * 4.0f;

        while (t < 1.0f)
        {
            Vector3 previousPos = transform.position;

            t += Time.deltaTime * speed;
            Vector3 m1 = Vector3.Lerp(startPos, controlPoint, t);
            Vector3 m2 = Vector3.Lerp(controlPoint, targetPos, t);
            Vector3 currentPos = Vector3.Lerp(m1, m2, t);
            //transform.LookAt(Vector3.Lerp(m1, m2, t + 0.01f));

            // --- CALCUL DE LA ROTATION ---
        // On calcule la direction vers laquelle on va
        Vector3 direction = currentPos - previousPos;
        transform.position = currentPos;

        if (direction != Vector3.zero)
        {
            // On calcule l'angle en degrés
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            // On applique la rotation sur l'axe Z
            // Note : Soustrais 90 si ton triangle pointe vers le haut par défaut
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f); 
        }

            yield return null;
        }

        FinaliserVol();
    }


    public void LaunchVertical()
    {
        if (trembleCoroutine != null) {
            StopCoroutine(trembleCoroutine);
        }

        canClick = false; 
        transform.SetParent(null);
        StartCoroutine(FlyToSky());
    }

    IEnumerator FlyToSky()
    {
        isMoving = true;
        float t = 0;
        while (t < 2.5f) 
        {
            transform.Translate(Vector3.up * (speed * 2) * Time.deltaTime, Space.World);
            t += Time.deltaTime;
            yield return null;
        }
        FinaliserVol();
    }

    void FinaliserVol()
    {
        // On pourrait instancier un effet de particules ici
        if (manager != null) {
            manager.StartNewCycle();
        }
        Destroy(gameObject);
    }
}
