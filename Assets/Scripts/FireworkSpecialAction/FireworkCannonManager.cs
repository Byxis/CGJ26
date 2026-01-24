using UnityEngine;
using System.Collections;

public class FireworkCannonManager : MonoBehaviour
{
    [Header("Références")]
    private Animator animator;
    public GameObject fireworkPrefab; 
    public Transform launchPoint; 
    public Transform targetPoint;
    public Transform cannonMesh;
    private Coroutine timeoutCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        StartNewCycle();
    }

    public void StartNewCycle()
    {
        StartCoroutine(FireworkSequence());
    }

    IEnumerator FireworkSequence()
    {
        GameObject currentFirework = Instantiate(fireworkPrefab, launchPoint);
        currentFirework.transform.localPosition = Vector3.zero;

        FireworkAction script = currentFirework.GetComponent<FireworkAction>();
        if (script != null) {
            script.Setup(targetPoint, this);
        }
        
        
        yield return new WaitForSeconds(3.0f);


        
        if (script != null)
        {
            script.EnableClick(); // On autorise enfin le joueur à cliquer
            if (animator != null) {
                animator.SetTrigger("isReady");
            }

            
            //script.StartTrembling(); // On lance l'effet visuel de vibration
        }
        timeoutCoroutine = StartCoroutine(WaitAndLaunchAuto(currentFirework, 5f));
    }

    IEnumerator WaitAndLaunchAuto(GameObject firework, float timeout)
    {
        yield return new WaitForSeconds(timeout);

        if (firework != null)
        {
            FireworkAction script = firework.GetComponent<FireworkAction>();
            if (script != null && !script.IsMoving()) 
            {
                if (animator != null) {
                    animator.SetTrigger("onFire");
                    animator.ResetTrigger("isReady");
                }
                script.LaunchVertical(); // Nouvelle fonction pour le tir vers le haut
            }
        }
    }

    public void CancelTimeout()
    {
        if (timeoutCoroutine != null)
        {
            StopCoroutine(timeoutCoroutine);
        }
    }

    public void OnFireworkSent()
    {
        // On consomme le trigger de recul
        if (animator != null) 
        {
            animator.SetTrigger("onFire"); 
            // Optionnel : on s'assure que isReady est "éteint"
            animator.ResetTrigger("isReady");
        }
        Debug.Log("Missile envoyé !");
    }
}
