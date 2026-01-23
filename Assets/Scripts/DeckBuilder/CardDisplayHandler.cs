using UnityEngine;
using TMPro; // This is required to talk to the Text objects

public class CardDisplayHandler : MonoBehaviour
{
    // 1. The Data: We hold a reference to the data file
    public CardData cardData; 

    // 2. The UI: We need to know which text objects to update
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI cardCostText;

    // This function will be called by the DeckManager later
    public void SetCardData(CardData data)
    {
        // Store the data
        cardData = data;

        // Update the UI text
        cardNameText.text = data.cardName;
        cardCostText.text = data.cost.ToString(); // Convert number to string
    }
}

