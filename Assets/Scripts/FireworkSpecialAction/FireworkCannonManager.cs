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
    private CannonRecoilController recoilController;
    private Coroutine timeoutCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        
        // Récupère le contrôleur de recul sur le mesh du canon
        if (cannonMesh != null)
        {
            recoilController = cannonMesh.GetComponent<CannonRecoilController>();
            if (recoilController == null)
            {
                Debug.LogWarning("CannonRecoilController non trouvé sur cannonMesh. Le recul ne fonctionnera pas.");
            }
        }
        
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
        
        // Cooldown plus long avec du random (10-15 secondes au lieu de 3)
        float randomDelay = Random.Range(10f, 15f);
        yield return new WaitForSeconds(randomDelay);


        
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
        // Déclenche le recul programmatique
        if (recoilController != null)
        {
            recoilController.TriggerRecoil();
        }
        
        // Déclenche les autres animations de l'Animator
        if (animator != null) 
        {
            animator.SetTrigger("onFire");
            animator.ResetTrigger("isReady");
        }
        Debug.Log("Missile envoyé !");
    }
}
