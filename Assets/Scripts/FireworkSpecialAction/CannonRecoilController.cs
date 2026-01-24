using UnityEngine;
using System.Collections;

public class CannonRecoilController : MonoBehaviour
{
    [Header("Paramètres de recul")]
    [Tooltip("Distance du recul sur l'axe Y")]
    public float recoilDistance = -0.8f;
    
    [Tooltip("Durée du recul en secondes")]
    public float recoilDuration = 0.083333f; // 5 frames à 60fps
    
    [Tooltip("Durée du retour en secondes")]
    public float returnDuration = 0.25f; // 15 frames à 60fps (20 - 5)
    
    private Vector3 originalLocalPosition;
    private Coroutine recoilCoroutine;
    
    void Start()
    {
        // Sauvegarde la position locale initiale
        originalLocalPosition = transform.localPosition;
    }
    
    /// <summary>
    /// Déclenche l'animation de recul de manière relative à la position actuelle
    /// </summary>
    public void TriggerRecoil()
    {
        // Si un recul est déjà en cours, on l'arrête
        if (recoilCoroutine != null)
        {
            StopCoroutine(recoilCoroutine);
        }
        
        recoilCoroutine = StartCoroutine(RecoilSequence());
    }
    
    private IEnumerator RecoilSequence()
    {
        // Position de départ (position actuelle)
        Vector3 startPosition = transform.localPosition;
        
        // Position de recul (relative à la position actuelle)
        Vector3 recoilPosition = startPosition + new Vector3(0, recoilDistance, 0);
        
        // Phase 1 : Recul
        float elapsed = 0f;
        while (elapsed < recoilDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / recoilDuration);
            transform.localPosition = Vector3.Lerp(startPosition, recoilPosition, t);
            yield return null;
        }
        
        // S'assurer qu'on atteint exactement la position de recul
        transform.localPosition = recoilPosition;
        
        // Phase 2 : Retour à la position de départ
        elapsed = 0f;
        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / returnDuration);
            // On utilise une courbe d'ease-out pour un retour plus naturel
            float smoothT = 1f - Mathf.Pow(1f - t, 3f);
            transform.localPosition = Vector3.Lerp(recoilPosition, startPosition, smoothT);
            yield return null;
        }
        
        // S'assurer qu'on revient exactement à la position de départ
        transform.localPosition = startPosition;
        
        recoilCoroutine = null;
    }
    
    /// <summary>
    /// Réinitialise la position à la position originale sauvegardée au Start
    /// </summary>
    public void ResetToOriginalPosition()
    {
        if (recoilCoroutine != null)
        {
            StopCoroutine(recoilCoroutine);
            recoilCoroutine = null;
        }
        transform.localPosition = originalLocalPosition;
    }
}
