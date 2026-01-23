using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    [Header("UI Linking")]
    public Image iconImage;
    public Slider progressSlider;
    public Image fillImage;        
    public Gradient progressColor; 

    [Header("Settings")]
    public Transform spawnPoint;   // Glisse un objet vide "SpawnPoint" de ta scène ici
    
    [Header("Autre")]
    public GameObject unitController;
    public int click_to_spawn;

    private int _currentClicks = 0;

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 2f, Time.deltaTime * 10f);
    }

    void Start()
    {
        if (progressSlider != null)
        {
            progressSlider.minValue = 0;
            progressSlider.maxValue = click_to_spawn;
            progressSlider.value = 0;
        }
        UpdateVisuals();
    }

    public void OnCardClicked()
    {
        
        // Effet visuel de clic (grossissement)
        transform.localScale = Vector3.one * 2.3f;
        
        _currentClicks++;
        UpdateVisuals();

        if (_currentClicks >= click_to_spawn)
        {
            SpawnUnit();
        }
    }

    private void UpdateVisuals()
    {
        if (progressSlider == null || fillImage == null) return;

        progressSlider.value = _currentClicks;

        // Calcul du ratio (0 à 1) pour le dégradé
        float normalizedValue = (float)_currentClicks / click_to_spawn;
        fillImage.color = progressColor.Evaluate(normalizedValue);
    }

    private void SpawnUnit()
    {
        if (unitController != null && spawnPoint != null)
        {
            // Apparition de l'unité
            GameObject g = Instantiate(unitController, spawnPoint.position, Quaternion.identity);
            g.layer = LayerMask.NameToLayer("TeamPlayer");
            Debug.Log("UNITÉ SPAWN !");
        }
        else
        {
            Debug.LogError("Spawn impossible : Prefab ou SpawnPoint manquant sur " + gameObject.name);
        }

        // Reset
        _currentClicks = 0;
        UpdateVisuals();
    }
}