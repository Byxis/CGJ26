using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopSlot : MonoBehaviour
{
    [Header("References")]
    public Transform cardContainer; // Where the card spawns
    public Button buyButton;        // The button on the slot itself

    private CardData myCardData;
    private ShopManager myManager;

    public void Initialize(CardData data, GameObject realCardPrefab, ShopManager manager)
    {
        myCardData = data;
        myManager = manager;

        // 1. Clean up old visuals
        foreach (Transform child in cardContainer) Destroy(child.gameObject);

        // 2. Spawn the visual card
        GameObject visualCard = Instantiate(realCardPrefab, cardContainer, false);

        // 3. Pass data to the prefab and let IT handle the display
        CardDisplayHandler display = visualCard.GetComponent<CardDisplayHandler>();
        if (display != null)
        {
            display.SetCardData(data);
        }

        // 4. Disable Raycasts on the spawned card so we can click the shop button behind it
        CanvasGroup group = visualCard.GetComponent<CanvasGroup>();
        if (group == null) group = visualCard.AddComponent<CanvasGroup>();
        group.blocksRaycasts = false; 

        // 5. Setup Buy Logic
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(OnBuyClick);
    }

    void OnBuyClick()
    {
        myManager.TryBuyCard(myCardData, this.gameObject);
    }
}