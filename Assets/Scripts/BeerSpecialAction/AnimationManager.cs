using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class AnimationManager : MonoBehaviour
{
    private Animator animator;
    public GameObject beerPrefab; 
    public Transform handTransform;
    public Transform targetPoint;
    private Coroutine timeoutCoroutine;
    void Start()
    {
        animator = GetComponent<Animator>();
        StartNewCycle(); 
    }

    public void StartNewCycle()
    {
        StartCoroutine(PlayAnimationSequence(5.0f));
    }
    
    IEnumerator PlayAnimationSequence(float delai)
    {
        

        GameObject newBeer = Instantiate(beerPrefab, handTransform);
        newBeer.name = "Beer";
        newBeer.transform.localPosition = Vector3.zero;


        BeerAction script = newBeer.GetComponent<BeerAction>();
        if (script != null)
        {
            script.Setup(targetPoint, this);
            script.finalPoint = targetPoint;
        }


        yield return new WaitForSeconds(delai);

        animator.SetTrigger("raiseBeer");
        if (script != null)
        {
            script.EnableClick();
        }

        timeoutCoroutine = StartCoroutine(WaitAndCancel(newBeer, 5f));
    }

    IEnumerator WaitAndCancel(GameObject beer, float timeout)
    {
        yield return new WaitForSeconds(timeout);

        // Si on arrive ici, c'est que le joueur n'a pas cliqué
        if (beer != null)
        {
            if (animator != null)
            {
                animator.SetTrigger("returnHand");
            }
            yield return new WaitForSeconds(1.5f);
            Destroy(beer);
            StartNewCycle(); // On relance un cycle propre
        }
    }

    public void CancelTimeout()
{
    if (timeoutCoroutine != null)
    {
        StopCoroutine(timeoutCoroutine);
    }
}

    public void OnBeerSent()
    {
        animator.SetTrigger("returnHand");
    }
}
