using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    [Header("Paramètres de l'explosion - style Fireworks")]
    public int particleCount = 200;
    
    void Start()
    {
        CreateFireworksExplosion();
        Destroy(gameObject, 2.5f);
    }
    
    void CreateFireworksExplosion()
    {
        GameObject particleSystemObj = new GameObject("FireworksExplosion");
        particleSystemObj.transform.position = transform.position;
        particleSystemObj.transform.parent = transform;
        
        ParticleSystem ps = particleSystemObj.AddComponent<ParticleSystem>();
        
        // Arrêter le ParticleSystem avant de le configurer
        ps.Stop();
        
        // Configuration principale - reproduit le prefab Fireworks
        var main = ps.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.3f, 1.78f); // Variable comme le prefab
        main.startSpeed = new ParticleSystem.MinMaxCurve(1.75f, 2.25f); // Vitesse lente
        main.startSize = 0.05f; // Particules très petites
        main.startColor = Color.red; // Rouge vif
        main.maxParticles = particleCount;
        main.loop = false;
        main.duration = 0.2f;
        main.gravityModifier = 0.05f; // Légère gravité comme le prefab
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        
        // Émission - Burst instantané comme un feu d'artifice
        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, (short)particleCount)
        });
        
        // Forme - Sphérique pour une explosion omnidirectionnelle
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.1f;
        
        // Couleur sur la durée de vie - Fade out progressif comme le prefab
        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(Color.red, 0f), 
                new GradientColorKey(Color.red, 1f)
            },
            new GradientAlphaKey[] { 
                new GradientAlphaKey(1f, 0f),      // Opaque au début
                new GradientAlphaKey(1f, 0.47f),    // Reste opaque jusqu'à 47%
                new GradientAlphaKey(0f, 1f)        // Fade out complet à la fin
            }
        );
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);
        
        // Renderer
        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = 100;
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        
        ps.Play();
    }
}
