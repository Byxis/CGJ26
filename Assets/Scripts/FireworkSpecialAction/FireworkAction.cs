using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class FireworkAction : MonoBehaviour, IPointerClickHandler
{
    [Header("Dégâts AOE")]
    public float damage = 4.0f;
    public float explosionRadius = 2f;
    public AnimationCurve damageFalloff = AnimationCurve.Linear(0, 1, 1, 0.3f);

    [Header("Réglages")]
    public float speed = 3f;
    public float amplitudeTremblement = 0.05f;

    [Header("Effets visuels")]
    [Tooltip("Prefab de l'effet de trainée")]
    public GameObject trailEffectPrefab;

    [Tooltip("Prefab de l'effet d'explosion")]
    public GameObject explosionEffectPrefab;

    private Transform targetPoint;
    private FireworkCannonManager manager;

    private bool isMoving = false;
    private bool canClick = false;
    private Coroutine trembleCoroutine;
    private GameObject trailEffectInstance;

    void Start()
    {
        // Désactiver le sprite renderer moche
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        // Créer l'effet de trainée depuis le prefab
        if (trailEffectPrefab != null)
        {
            trailEffectInstance = Instantiate(trailEffectPrefab, transform.position, Quaternion.identity);
            trailEffectInstance.transform.SetParent(transform);
            trailEffectInstance.transform.localPosition = Vector3.zero;
        }
        else
        {

            CreateProjectileParticles();
        }
    }

    void CreateProjectileParticles()
    {
        GameObject particleObj = new GameObject("ProjectileParticles");
        particleObj.transform.SetParent(transform);
        particleObj.transform.localPosition = Vector3.zero;

        ParticleSystem projectileParticles = particleObj.AddComponent<ParticleSystem>();

        // Arrêter le ParticleSystem avant de le configurer
        projectileParticles.Stop();

        var main = projectileParticles.main;
        main.startLifetime = 0.5f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.1f, 0.3f); // Très lent pour rester groupées
        main.startSize = 0.05f; // Même taille que l'explosion
        main.startColor = new Color(1f, 0.3f, 0f, 1f); // Orange/rouge
        main.maxParticles = 200; // Même nombre que l'explosion
        main.loop = true;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = projectileParticles.emission;
        emission.rateOverTime = 400; // Beaucoup de particules générées

        var shape = projectileParticles.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.05f; // Très petit rayon = très concentré

        // Fade out
        var colorOverLifetime = projectileParticles.colorOverLifetime;
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

        var renderer = projectileParticles.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = 15;

        projectileParticles.Play();
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    public void Setup(Transform target, FireworkCannonManager mgr)
    {
        // Ignore le target passé en paramètre, on va chercher dynamiquement
        manager = mgr;
    }

    private Transform FindFurthestEnemy()
    {
        float minX = float.MaxValue;
        Transform closest = null;

        // Chercher tous les objets avec le layer TeamEnemy
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == LayerMask.NameToLayer("TeamEnemy"))
            {
                if (obj.transform.position.x < minX)
                {
                    minX = obj.transform.position.x;
                    closest = obj.transform;
                }
            }
        }

        return closest;
    }

    public void EnableClick()
    {
        canClick = true;
    }

    public void StartTrembling()
    {
        trembleCoroutine = StartCoroutine(TrembleRoutine());
    }

    IEnumerator TrembleRoutine()
    {
        Vector3 posInitiale = transform.localPosition;
        while (!isMoving)
        {
            transform.localPosition = posInitiale + Random.insideUnitSphere * amplitudeTremblement;
            yield return null;
        }
        transform.localPosition = posInitiale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clic détecté sur : " + gameObject.name);
        if (!isMoving && canClick)
        {
            // Trouve la cible dynamiquement au moment du clic
            targetPoint = FindFurthestEnemy();

            if (targetPoint == null)
            {
                Debug.LogWarning("Aucun ennemi trouvé !");
                return;
            }

            if (manager != null)
            {
                manager.CancelTimeout();
                manager.OnFireworkSent();
            }

            if (trembleCoroutine != null)
            {
                StopCoroutine(trembleCoroutine);
            }
            transform.SetParent(null);
            StartCoroutine(FlyToTargetCurve());
        }
    }

    IEnumerator FlyToTargetCurve()
    {
        isMoving = true;
        float t = 0;
        Vector3 startPos = transform.position;
        Vector3 targetPos = targetPoint.position;

        Vector3 controlPoint = (startPos + targetPos) / 2 + Vector3.up * 4.0f;

        while (t < 1.0f)
        {
            Vector3 previousPos = transform.position;

            t += Time.deltaTime * speed;
            Vector3 m1 = Vector3.Lerp(startPos, controlPoint, t);
            Vector3 m2 = Vector3.Lerp(controlPoint, targetPos, t);
            Vector3 currentPos = Vector3.Lerp(m1, m2, t);
            //transform.LookAt(Vector3.Lerp(m1, m2, t + 0.01f));

            // --- CALCUL DE LA ROTATION ---
            // On calcule la direction vers laquelle on va
            Vector3 direction = currentPos - previousPos;
            transform.position = currentPos;

            if (direction != Vector3.zero)
            {
                // On calcule l'angle en degrés
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // On applique la rotation sur l'axe Z
                // Note : Soustrais 90 si ton triangle pointe vers le haut par défaut
                transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
            }

            yield return null;
        }

        FinaliserVol();
    }


    public void LaunchVertical()
    {
        if (trembleCoroutine != null)
        {
            StopCoroutine(trembleCoroutine);
        }

        canClick = false;
        transform.SetParent(null);
        StartCoroutine(FlyToSky());
    }

    IEnumerator FlyToSky()
    {
        isMoving = true;
        float t = 0;
        while (t < 2.5f)
        {
            transform.Translate(Vector3.up * (speed * 2) * Time.deltaTime, Space.World);
            t += Time.deltaTime;
            yield return null;
        }
        FinaliserVol();
    }

    void FinaliserVol()
    {
        // Explosion AOE - Infliger les dégâts dans le rayon
        Vector3 explosionPos = transform.position;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionPos, explosionRadius, LayerMask.GetMask("TeamEnemy"));

        int hitCount = 0;
        foreach (Collider2D collider in colliders)
        {
            UnitController targetUnit = collider.GetComponent<UnitController>();
            if (targetUnit != null)
            {
                // Calculer les dégâts en fonction de la distance (centre = max, bords = réduits)
                float distance = Vector2.Distance(explosionPos, collider.transform.position);
                float normalizedDistance = Mathf.Clamp01(distance / explosionRadius);
                float damageMultiplier = damageFalloff.Evaluate(normalizedDistance);
                float finalDamage = damage * damageMultiplier;

                targetUnit.TakeDamage(finalDamage);
                hitCount++;
            }
        }

        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, explosionPos, Quaternion.identity);
        }
        else
        {
            GameObject explosionObj = new GameObject("Explosion");
            explosionObj.transform.position = explosionPos;
            explosionObj.AddComponent<ExplosionEffect>();
        }

        if (manager != null)
        {
            manager.StartNewCycle();
        }
        Destroy(gameObject);
    }
}
