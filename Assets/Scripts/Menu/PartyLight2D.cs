using UnityEngine;
using UnityEngine.Rendering.Universal; // Obligatoire pour manipuler la Light 2D

public class PartyLight2D : MonoBehaviour
{
    [Header("Réglages du Balayage")]
    public float vitesse = 2.0f;      
    public float amplitude = 45.0f;  
    public float angleDepart = 0.0f;

    [Header("Réglages de Couleur (Optionnel)")]
    public bool changerCouleur = true;
    public float vitesseCouleur = 1.0f;

    private Light2D _light2D;

    void Start()
    {
        _light2D = GetComponent<Light2D>();
    }

    void FixedUpdate()
    {
        float angle = angleDepart + Mathf.Sin(Time.time * vitesse) * amplitude;
        
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (changerCouleur && _light2D != null)
        {
            _light2D.color = Color.HSVToRGB(Mathf.PingPong(Time.time * vitesseCouleur, 1), 0.8f, 1f);
        }
    }
}