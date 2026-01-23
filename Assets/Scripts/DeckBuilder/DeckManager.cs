using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;        // <--- NEEDED for HorizontalLayoutGroup
using System.Collections;    // <--- NEEDED for IEnumerator & Coroutines

public class DeckManager : MonoBehaviour
{
    [Header("References")]
    public GameObject cardDisplayprefab; // The blue prefab you just made
    public Transform deckArea;    // The 'HandArea' object in the canvas
    
    [Header("Data")]
    public List<CardData> deckData = new List<CardData>(); // Your list of card assets

    public void AddCardToDeck(CardData data)
    {
        // 1. Spawn the object inside the deckArea
        GameObject newCard = Instantiate(cardDisplayprefab, deckArea, false);

        // 2. Inject Data
        CardDisplayHandler display = newCard.GetComponent<CardDisplayHandler>();
        if(display != null) 
        {
            display.SetCardData(data);
        }

        // 3. Randomize Position
        // Get the dimensions of the area we are spawning into
        RectTransform areaRect = deckArea.GetComponent<RectTransform>();
        
        // Define a margin so cards don't spawn exactly on the edge
        float padding = 50f; 

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
    }
}