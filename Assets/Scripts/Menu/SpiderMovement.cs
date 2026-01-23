using System.Collections;
using UnityEngine;

public class SpiderMovement : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    
    [SerializeField] private GameObject startPosition;
    [SerializeField] private GameObject endPosition;
    [SerializeField] private LineRenderer lineRenderer;

    private Transform target;

    private Vector3 direction = Vector3.down;
    private float ropeLength;
    private float ropeMaxLength;
    private bool wait = false;

    void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.startColor = Color.white;
            lineRenderer.endColor = Color.white;

            // Ensure a valid material so the line is not pink (missing shader).
            if (lineRenderer.sharedMaterial == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
                if (shader == null)
                {
                    shader = Shader.Find("Sprites/Default");
                }

                if (shader != null)
                {
                    lineRenderer.material = new Material(shader);
                    lineRenderer.material.color = Color.white;
                }
            }
        }

        ropeLength = 0f;
        ropeMaxLength = Vector3.Distance(startPosition.transform.position, endPosition.transform.position);
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

        UpdateRope();
    }

    IEnumerator WaitToChangeTarget(Transform newTarget)
    {
        this.wait = true;
        yield return new WaitForSeconds(3f);
        this.target = newTarget;
        this.wait = false;
    }

    // Draws a rope: grows while descending, retracts while ascending.
    private void UpdateRope()
    {
        if (lineRenderer == null) return;

        Vector3 top = startPosition.transform.position;
        Vector3 bottom = endPosition.transform.position;
        Vector3 axis = (bottom - top).normalized;

        bool goingDown = target == endPosition.transform;

        // Project spider position onto the rope axis to measure its progress from the top point.
        float projected = Vector3.Dot(transform.position - top, axis);
        projected = Mathf.Clamp(projected, 0f, ropeMaxLength);

        if (goingDown)
        {
            ropeLength = Mathf.Max(ropeLength, projected); // grow to current reach
        }
        else
        {
            ropeLength = Mathf.Max(0f, ropeLength - speed * Time.fixedDeltaTime); // retract while going up
        }

        Vector3 ropeEnd = top + axis * ropeLength;

        lineRenderer.SetPosition(0, top);
        lineRenderer.SetPosition(1, ropeEnd);
    }
}