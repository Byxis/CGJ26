using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class DeckManager : MonoBehaviour
{
    // ... existing header ...

    public void SaveToInventory()
    {
        Debug.Log($"DeckManager [{GetInstanceID()}]: Saving {deckData.Count} units to Inventory...");
        List<UnitData> unitsToSave = new List<UnitData>();

        for (int i = 0; i < deckData.Count; i++)
        {
            var card = deckData[i];
            if (card == null)
            {
                Debug.LogWarning($"DeckManager: Card at index {i} is NULL!");
                continue;
            }

            Debug.Log($"DeckManager: Card {i} is named '{card.name}' and is of type '{card.GetType().Name}'");

            if (card is UnitData unit)
            {
                unitsToSave.Add(unit);
            }
            else if (card.GetType().Name == "UnitStats")
            {
                Debug.LogError(
                    $"DeckManager: CRITICAL DATA ERROR! Card '{card.name}' is a 'UnitStats' asset. The Inventory ONLY accepts 'UnitData'. Please create a UnitData asset for this unit and update your Resources/Shop!");
            }
            else
            {
                Debug.LogWarning(
                    $"DeckManager: Card '{card.name}' is NOT UnitData! It is {card.GetType().Name}. Skipping.");
            }
        }

        Debug.Log($"DeckManager: Final list has {unitsToSave.Count} units. Sending to Inventaire...");
        FindFirstObjectByType<Inventaire>().OnShopLeave(unitsToSave);
    }
    [Header("References")]
    public GameObject unitCardPrefab;     // Formerly cardDisplayprefab
    public GameObject upgradeCardPrefab;  // NEW
    public Transform deckArea;            // The 'HandArea' object in the canvas

    [Header("Data")]
    public List<BaseCard> deckData = new List<BaseCard>();  // Your list of card assets

    private void Start()
    {
        Debug.Log($"DeckManager [{GetInstanceID()}]: Start called. Loading inventory...");
        LoadDeckFromInventory();
    }

    private void LoadDeckFromInventory()
    {
        var inventory = FindFirstObjectByType<Inventaire>();
        if (inventory != null && inventory.UnitStats != null)
        {
            Debug.Log($"DeckManager [{GetInstanceID()}]: Found Inventaire with {inventory.UnitStats.Count} units.");
            foreach (var unitData in inventory.UnitStats)
            {
                Debug.Log($"DeckManager [{GetInstanceID()}]: Loading unit from inventory: {unitData.name}");
                AddCardToDeck(unitData);
            }
        }
        else
        {
            Debug.LogWarning($"DeckManager [{GetInstanceID()}]: Inventaire not found or UnitStats is null!");
        }
    }

    public void AddCardToDeck(BaseCard data)
    {
        Debug.Log($"DeckManager [{GetInstanceID()}]: AddCardToDeck called for {data.name}");
        // 1. Choose Prefab
        GameObject prefabToUse = unitCardPrefab;
        BaseCard dataToUse = data;

        if (data is UpgradeCard)
        {
            prefabToUse = upgradeCardPrefab;
        }
        else if (data is UnitData unit)
        {
            // IMPORTANT: Clone UnitData so we have a unique instance for this card in our deck
            // This ensures upgrades apply only to this specific card
            dataToUse = Instantiate(unit);
            dataToUse.name = unit.name;  // Optional: keep the inspector name nice

            // Initialize Runtime Stats
            if (unit.currentStats != null)
            {
                // Maintain existing upgrades!
                ((UnitData)dataToUse).currentStats = Instantiate(unit.currentStats);
            }
            else if (unit.unitPrefab != null)
            {
                // Fallback to Base Stats from Prefab
                var controller = unit.unitPrefab.GetComponent<UnitController>();
                if (controller != null && controller.Stats != null)
                {
                    ((UnitData)dataToUse).currentStats = Instantiate(controller.Stats);
                }
            }
        }

        // 2. Spawn the object inside the deckArea
        GameObject newCard = Instantiate(prefabToUse, deckArea, false);

        // 3. Inject Data (Using the CLONE if it was a unit)
        BaseCardDisplay display = newCard.GetComponent<BaseCardDisplay>();
        if (display != null)
        {
            display.SetCardData(dataToUse);
        }

        // 4. Randomize Position
        // Get the dimensions of the area we are spawning into
        RectTransform areaRect = deckArea.GetComponent<RectTransform>();

        // Define a margin so cards don't spawn exactly on the edge
        float padding = 100f;

        // Calculate safe ranges
        // rect.width gives the full width, so we go from -Half to +Half
        float minX = -(areaRect.rect.width / 2f) + padding;
        float maxX = (areaRect.rect.width / 2f) - padding;
        float minY = -(areaRect.rect.height / 2f) + padding;
        float maxY = (areaRect.rect.height / 2f) - padding;

        // Pick random coordinates
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        // Apply the new position
        newCard.transform.localPosition = new Vector3(randomX, randomY, 0);

        // Add to list
        deckData.Add(dataToUse);
    }
}