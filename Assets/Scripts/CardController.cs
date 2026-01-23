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
    
    [Header("Debug/Test")]
    public UnitData testData;      // Permet de tester sans attendre le Manager

    private int _currentClicks = 0;
    private UnitData _data;

    void Start()
    {
        // Si tu as mis une carte de test dans l'inspecteur, on l'initialise
        if (testData != null)
        {
            Initialize(testData);
        }
    }

    void Update()
    {
        // Effet de rebond : revient à la taille 2 progressivement
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 2f, Time.deltaTime * 10f);
    }

    public void Initialize(UnitData data)
    {
        _data = data;
        
        if (_data == null) return;

        iconImage.sprite = _data.icon;
        progressSlider.maxValue = _data.clicksRequiredToSpawn;
        progressSlider.value = 0;
        _currentClicks = 0;
        
        UpdateVisuals();
    }

    public void OnCardClicked()
    {
        // Sécurité : si aucune donnée n'est chargée, on ne fait rien
        if (_data == null) 
        {
            Debug.LogWarning("La carte n'a pas de UnitData !");
            return;
        }

        Debug.Log("Clic sur : " + _data.unitName);
        
        // Effet visuel de clic (grossissement)
        transform.localScale = Vector3.one * 2.3f;
        
        _currentClicks++;
        UpdateVisuals();

        if (_currentClicks >= _data.clicksRequiredToSpawn)
        {
            SpawnUnit();
        }
    }

    private void UpdateVisuals()
    {
        if (progressSlider == null || fillImage == null) return;

        progressSlider.value = _currentClicks;

        // Calcul du ratio (0 à 1) pour le dégradé
        float normalizedValue = (float)_currentClicks / _data.clicksRequiredToSpawn;
        fillImage.color = progressColor.Evaluate(normalizedValue);
    }

    private void SpawnUnit()
    {
        if (_data.unitPrefab != null && spawnPoint != null)
        {
            // Apparition de l'unité
            Instantiate(_data.unitPrefab, spawnPoint.position, Quaternion.identity);
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