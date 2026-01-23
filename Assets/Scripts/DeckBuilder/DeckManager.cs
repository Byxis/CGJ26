using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;        
using System.Collections;    

public class DeckManager : MonoBehaviour
{
    [Header("References")]
    public GameObject unitCardPrefab;    // Formerly cardDisplayprefab
    public GameObject upgradeCardPrefab; // NEW
    public Transform deckArea;    // The 'HandArea' object in the canvas
    
    [Header("Data")]
    public List<BaseCard> deckData = new List<BaseCard>(); // Your list of card assets

    public void AddCardToDeck(BaseCard data)
    {
        // 1. Choose Prefab
        GameObject prefabToUse = unitCardPrefab;
        BaseCard dataToUse = data; // Default to using the passed data (e.g. Upgrades)

        if(data is UpgradeCard) 
        {
            prefabToUse = upgradeCardPrefab;
        }
        else if (data is UnitStats unit)
        {
            // IMPORTANT: Clone UnitStats so we have a unique instance for this card in our deck
            // This ensures upgrades apply only to this specific card
            dataToUse = Instantiate(unit);
            dataToUse.name = unit.name; // Optional: keep the inspector name nice
        }

        // 2. Spawn the object inside the deckArea
        GameObject newCard = Instantiate(prefabToUse, deckArea, false);

        // 3. Inject Data (Using the CLONE if it was a unit)
        BaseCardDisplay display = newCard.GetComponent<BaseCardDisplay>();
        if(display != null) 
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