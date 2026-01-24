using UnityEngine;

public class FireworkTrailEffect : MonoBehaviour
{
    void Start()
    {
        CreateTrailParticles();
    }
    
    void CreateTrailParticles()
    {
        GameObject particleObj = new GameObject("TrailParticles");
        particleObj.transform.SetParent(transform);
        particleObj.transform.localPosition = Vector3.zero;
        
        ParticleSystem ps = particleObj.AddComponent<ParticleSystem>();
        
        // Arrêter le ParticleSystem avant de le configurer
        ps.Stop();
        
        var main = ps.main;
        main.startLifetime = 0.5f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.1f, 0.3f); // Très lent pour rester groupées
        main.startSize = 0.05f; // Même taille que l'explosion
        main.startColor = new Color(1f, 0.3f, 0f, 1f); // Orange/rouge
        main.maxParticles = 200; // Même nombre que l'explosion
        main.loop = true;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        
        var emission = ps.emission;
        emission.rateOverTime = 400; // Beaucoup de particules générées
        
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.05f; // Très petit rayon = très concentré
        
        // Fade out
        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(new Color(1f, 0.5f, 0f), 0f),
                new GradientColorKey(Color.red, 1f)
            },
            new GradientAlphaKey[] { 
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);
        
        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = 15;
        
        ps.Play();
    }
}
