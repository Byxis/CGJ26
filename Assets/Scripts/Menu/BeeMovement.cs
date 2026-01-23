using System.Collections;
using UnityEngine;

public class BeeMovement : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    
    [SerializeField] private GameObject startPosition;
    [SerializeField] private GameObject endPosition;

    private Transform target;

    private Vector3 direction = Vector3.down;

    void Start()
    {
        target = endPosition.transform;
    }

    void FixedUpdate()
    {
        // Calculate the direction based on the rotation around the Z axis
        Vector3 dir = target.position - transform.position;
        direction = dir.normalized;

        transform.Translate(direction * speed * Time.fixedDeltaTime, Space.World);

        if (Vector3.Distance(transform.position, endPosition.transform.position) < 0.1f)
        {
            StartCoroutine(WaitToChangeTarget(startPosition.transform));
        }
        else if (Vector3.Distance(transform.position, startPosition.transform.position) < 0.1f)
        {
            StartCoroutine(WaitToChangeTarget(endPosition.transform));
        }
    }

    IEnumerator WaitToChangeTarget(Transform newTarget)
    {
        yield return new WaitForSeconds(3f);
        this.target = newTarget;
    }
    
}