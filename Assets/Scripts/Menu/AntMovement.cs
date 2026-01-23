using UnityEngine;

public class AntMovement : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private GameObject endPosition;

    private Vector3 direction = Vector3.left;

    void Start()
    {
        endPosition = GameObject.FindWithTag("AntEnd");
    }

    void FixedUpdate()
    {
        // Calculate the direction based on the rotation around the Z axis
        Vector3 dir = endPosition.transform.position - transform.position;
        direction = dir.normalized;

        transform.Translate(direction * speed * Time.fixedDeltaTime, Space.World);

        // Check for boundaries
        if (Vector3.Distance(transform.position, endPosition.transform.position) < 0.1f)
        {
            Destroy(gameObject);
        }
    }
}