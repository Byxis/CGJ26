using UnityEngine;
using UnityEngine.InputSystem;

public class ParallaxCamera : MonoBehaviour
{
    [Header("Déplacement (Translation)")]
    [SerializeField] private float moveIntensity = 0.5f;
    
    [Header("Paramètres")]
    [SerializeField] private float smoothness = 5f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        // Ratio de -0.5 (bas/gauche) à 0.5 (haut/droite)
        float mouseX = (mousePos.x / Screen.width) - 0.5f;
        float mouseY = (mousePos.y / Screen.height) - 0.5f;

        // Position : La caméra bouge légèrement sur les côtés
        Vector3 targetPos = startPosition + new Vector3(mouseX * moveIntensity, mouseY * moveIntensity, 0);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothness);
    }
}