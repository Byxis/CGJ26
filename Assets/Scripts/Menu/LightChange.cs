using UnityEngine;
using UnityEngine.Rendering.Universal; // Obligatoire pour Light2D

public class LightChange : MonoBehaviour
{
    private Light2D light2D;
    private Light light3D;

    [Header("Réglages")]
    public float speed = 0.5f; // Vitesse du changement
    public float saturation = 0.8f; // Intensité de la couleur (0 à 1)
    public float value = 1.0f; // Luminosité (1 = Néon brillant)

    void Start()
    {
        // On récupère le composant quel que soit le type de lumière
        light2D = GetComponent<Light2D>();
        light3D = GetComponent<Light>();
    }

    void Update()
    {
        // On calcule une teinte qui boucle à l'infini entre 0 et 1
        float hue = (Time.time * speed) % 1.0f;

        // On convertit HSV en couleur RGB (très utile pour les néons)
        Color newColor = Color.HSVToRGB(hue, saturation, value);

        // On applique la couleur
        if (light2D != null) light2D.color = newColor;
        if (light3D != null) light3D.color = newColor;
    }
}