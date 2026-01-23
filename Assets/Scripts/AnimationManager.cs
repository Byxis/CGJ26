using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();

        InvokeRepeating("PlayRaiseBeer", 2.0f, 5.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlayRaiseBeer()
    {
        animator.SetTrigger("RaiseBeer");
    }
}
