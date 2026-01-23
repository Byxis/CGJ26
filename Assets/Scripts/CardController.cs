using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    [Header("Data")]
    public UnitData unitData;

    [Header("UI Linking")]
    public Image iconImage;
    public Slider progressSlider;
    public Image fillImage;
    public Gradient progressColor;
    public TMPro.TextMeshProUGUI costText;
    public int clicksRequiredToSpawn;

    [Header("Autre")]
    private Transform spawnPoint;

    private int _currentClicks = 0;

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 2f, Time.deltaTime * 10f);
    }

    void Start()
    {
        DeckPanel panel = GetComponentInParent<DeckPanel>();
        if (panel != null)
        {
            this.spawnPoint = panel.spawnPoint;
        }

        if (unitData != null)
        {
            if (iconImage != null)
            {
                iconImage.sprite = unitData.icon;
            }

            if (progressSlider != null)
            {
                progressSlider.minValue = 0;
                progressSlider.maxValue = unitData.clicksRequiredToSpawn;
                progressSlider.value = 0;
            }

            if (costText != null)
            {
                costText.text = unitData.clicksRequiredToSpawn.ToString();
            }
        }
        else
        {
            Debug.LogError("UnitData manquant sur la carte : " + gameObject.name);
        }

        UpdateVisuals();
    }

    public void OnCardClicked()
    {
        if (unitData == null)
            return;

        transform.localScale = Vector3.one * 2.3f;

        _currentClicks++;
        UpdateVisuals();

        if (_currentClicks >= unitData.clicksRequiredToSpawn)
        {
            SpawnUnit();
        }
    }

    private void UpdateVisuals()
    {
        if (progressSlider == null || fillImage == null || unitData == null)
            return;

        progressSlider.value = _currentClicks;

        float normalizedValue = (float)_currentClicks / unitData.clicksRequiredToSpawn;
        fillImage.color = progressColor.Evaluate(normalizedValue);
    }

    private void SpawnUnit()
    {
        if (unitData.unitPrefab != null && spawnPoint != null)
        {
            GameObject g = Instantiate(unitData.unitPrefab, spawnPoint.position, Quaternion.identity);
            g.layer = LayerMask.NameToLayer("TeamPlayer");
        }
        else
        {
            Debug.LogError("Spawn impossible : Prefab ou SpawnPoint manquant sur " + gameObject.name);
        }

        _currentClicks = 0;
        UpdateVisuals();
    }
}