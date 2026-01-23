using UnityEngine;
using UnityEngine.InputSystem;

public class ParallaxLayer : MonoBehaviour
{
    [Tooltip("0 = bouge pas, 1 = suit la souris à 100%")]
    [Range(0f, 2f)]
    [SerializeField] private float movementFactor = 0.1f;
    [SerializeField] private float smoothness = 5f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void FixedUpdate()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        // Calcul du décalage (-0.5 à 0.5)
        float mouseX = (mousePos.x / Screen.width) - 0.5f;
        float mouseY = (mousePos.y / Screen.height) - 0.5f;

        // On calcule la cible : la position de départ + le décalage souris * facteur
        Vector3 targetPos = startPosition + new Vector3(mouseX * movementFactor, mouseY * movementFactor, 0);

        // Appliquer le mouvement
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothness);
    }
}
