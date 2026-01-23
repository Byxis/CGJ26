using UnityEngine;
using UnityEngine.InputSystem;

public class ParallaxCamera : MonoBehaviour
{
    [Header("Déplacement (Translation)")]
    [SerializeField] private float moveIntensity = 0.5f;
    
    [Header("Inclinaison (Rotation)")]
    [SerializeField] private float rotationIntensity = 3.0f; // L'angle de rotation
    
    [Header("Paramètres")]
    [SerializeField] private float smoothness = 5f;

    private Vector3 startPosition;
    private Quaternion startRotation;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        // Ratio de -0.5 (bas/gauche) à 0.5 (haut/droite)
        float mouseX = (mousePos.x / Screen.width) - 0.5f;
        float mouseY = (mousePos.y / Screen.height) - 0.5f;

        // 1. Position : La caméra bouge légèrement
        Vector3 targetPos = startPosition + new Vector3(mouseX * moveIntensity, mouseY * moveIntensity, 0);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothness);

        float rotX = mouseY * rotationIntensity; 
        float rotY = mouseX * rotationIntensity;
        
        Quaternion targetRot = startRotation * Quaternion.Euler(rotX, rotY, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * smoothness);
    }
}