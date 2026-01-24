using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopSlot : MonoBehaviour
{
    [Header("References")]
    public Transform cardContainer; // Where the card spawns
    public Button buyButton;        // The button on the slot itself

    private BaseCard myCardData;
    private ShopManager myManager;

    public void Initialize(BaseCard data, GameObject realCardPrefab, ShopManager manager)
    {
        myCardData = data;
        myManager = manager;

        // 1. Clean up old visuals
        foreach (Transform child in cardContainer) Destroy(child.gameObject);

        // 2. Spawn the visual card
        // Note: realCardPrefab IS the chosen prefab from ShopManager (Unit or Upgrade)
        GameObject visualCard = Instantiate(realCardPrefab, cardContainer, false);

        // 3. Pass data to the prefab and let IT handle the display
        // Try BaseCardDisplay first (the new system)
        BaseCardDisplay display = visualCard.GetComponent<BaseCardDisplay>();
        if (display != null)
        {
            // For the SHOP display, we also clone Units so that if we later add functionality 
            // to "preview upgrade" or similar, we don't mess with the asset.
            // BUT we buy the original asset (so we get a fresh unit when we buy).
            BaseCard displayData = data;
            if (data is UnitStats unit)
            {
                displayData = Instantiate(unit);
            }
            
            display.SetCardData(displayData);
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