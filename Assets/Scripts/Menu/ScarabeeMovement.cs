using UnityEngine;

public class ScarabeeMovement : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private GameObject endPosition;

    private Vector2 direction = Vector2.right;

    void Start()
    {
        endPosition = GameObject.FindWithTag("ScarabeeEnd");
    }

    void FixedUpdate()
    {
        // Calculate the direction based on the rotation around the Z axis
        Vector2 dir = (Vector2)endPosition.transform.position - (Vector2)transform.position;
        direction = dir.normalized;

        transform.Translate(direction * speed * Time.fixedDeltaTime, Space.World);

        // Check for boundaries
        if (Vector2.Distance(transform.position, endPosition.transform.position) < 0.1f)
        {
            Destroy(gameObject);
        }
    }
}