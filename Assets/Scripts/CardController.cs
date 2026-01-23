using UnityEngine;
using UnityEngine.UI;
using TMPro; // Si vous utilisez TextMeshPro pour le texte (optionnel)

public class CardController : MonoBehaviour
{
    [Header("UI Linking")]
    public Image iconImage;       // Lier l'Image "Icon" ici
    public Slider progressSlider; // Lier le Slider "ClickGauge" ici
    // public TextMeshProUGUI costText; // Décommenter si vous voulez afficher le texte

    [Header("Data (A remplir manuellement ou par code)")]
    public UnitData unitData;     // Glissez votre ScriptableObject ici (Soldat, Tank...)
    public Transform spawnPoint;  // L'endroit où l'unité va apparaître dans le monde

    private int _currentClicks = 0;

    void Start()
    {
        // Initialisation visuelle au lancement du jeu
        if(unitData != null)
        {
            SetupCard();
        }
    }

    void SetupCard()
    {
        // On met à jour l'image
        iconImage.sprite = unitData.icon;
        
        // On configure la jauge
        progressSlider.maxValue = unitData.clicksRequiredToSpawn;
        progressSlider.value = 0;
        _currentClicks = 0;
    }

    // Cette fonction sera appelée quand on clique sur le bouton
    public void OnClick()
    {
        // 1. On incrémente
        _currentClicks++;

        // 2. Mise à jour visuelle immédiate
        progressSlider.value = _currentClicks;

        // 3. Vérification : est-ce qu'on a atteint le but ?
        if (_currentClicks >= unitData.clicksRequiredToSpawn)
        {
            SpawnUnit();
        }
    }

    void SpawnUnit()
    {
        // Instancier le prefab de l'unité à la position du spawnPoint
        if (unitData.unitPrefab != null && spawnPoint != null)
        {
            Instantiate(unitData.unitPrefab, spawnPoint.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Il manque le Prefab ou le SpawnPoint sur la carte !");
        }

        // Reset de la carte après le spawn
        _currentClicks = 0;
        progressSlider.value = 0;
    }
}