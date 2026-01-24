using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [Header("Configuration")]
    public string unitResourcesPath = "UnitCardPool";
    public string upgradeResourcesPath = "UpgradeCardPool";

    [Header("UI References")]
    public TextMeshProUGUI goldText;  // Drag Gold Text here
    public Button rerollButton;       // Drag Reroll Button here
    public Transform shopContainer;   // Drag Shop Panel here

    [Header("Prefabs")]
    public GameObject shopSlotPrefab;
    public GameObject unitCardPrefab;     // FORMERLY realCardPrefab
    public GameObject upgradeCardPrefab;  // NEW

    [Header("References")]
    public DeckManager deckManager;  // (Or HandManager, whichever you are using)

    private BaseCard[] fullCardPool;

    [Header("Economy")]
    [Header("Economy")]
    public int rerollCost = 2;

    void Start()
    {
        // 1. Initialize Economy (UI only)
        UpdateGoldUI();

        // 2. Setup Reroll Button Click Listener
        if (rerollButton != null)
        {
            rerollButton.onClick.RemoveAllListeners();
            rerollButton.onClick.AddListener(OnRerollClick);

            // Optional: Update button text to show cost
            TextMeshProUGUI btnText = rerollButton.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
                btnText.text = $"Reroll ({rerollCost}g)";
        }

        // 3. Load Cards (Load both pools and merge)
        var units = Resources.LoadAll<BaseCard>(unitResourcesPath);
        var upgrades = Resources.LoadAll<BaseCard>(upgradeResourcesPath);

        // Merge arrays
        List<BaseCard> mergedList = new List<BaseCard>();
        if (units != null)
            mergedList.AddRange(units);
        if (upgrades != null)
            mergedList.AddRange(upgrades);

        fullCardPool = mergedList.ToArray();

        if (fullCardPool.Length == 0)
            Debug.LogError(
                $"No cards found! Check Resources folders: '{unitResourcesPath}' and '{upgradeResourcesPath}'");

        // 4. Free Reroll on Start
        PerformReroll();
    }

    // --- BUTTON CLICK HANDLER ---
    // This is linked to the button. It checks money BEFORE rerolling.
    public void OnRerollClick()
    {
        int gold = Inventaire.Instance != null ? Inventaire.Instance.currentGold : 0;

        if (gold >= rerollCost)
        {
            // Pay the cost
            if (Inventaire.Instance != null)
                Inventaire.Instance.currentGold -= rerollCost;

            UpdateGoldUI();

            // Perform the reroll
            PerformReroll();
        }
        else
        {
            Debug.Log("Not enough gold to reroll!");
        }
    }

    // --- ACTUAL REROLL LOGIC ---
    // This just does the work (separate so we can call it for free in Start)
    private void PerformReroll()
    {
        // 1. Turn ON layout to organize
        HorizontalLayoutGroup layout = shopContainer.GetComponent<HorizontalLayoutGroup>();
        if (layout != null)
            layout.enabled = true;

        // 2. Clear old slots
        foreach (Transform child in shopContainer) Destroy(child.gameObject);

        // 3. Spawn 5 new slots
        for (int i = 0; i < 5; i++)
        {
            if (fullCardPool.Length == 0)
                break;

            BaseCard randomData = fullCardPool[Random.Range(0, fullCardPool.Length)];
            GameObject newSlotObj = Instantiate(shopSlotPrefab, shopContainer);

            ShopSlot newSlotScript = newSlotObj.GetComponent<ShopSlot>();
            if (newSlotScript != null)
            {
                // CHOOSE PREFAB BASED ON TYPE
                GameObject prefabToUse = unitCardPrefab;
                if (randomData is UpgradeCard)
                    prefabToUse = upgradeCardPrefab;

                newSlotScript.Initialize(randomData, prefabToUse, this);
            }
        }

        // 4. Turn OFF layout to freeze positions
        StartCoroutine(DisableShopLayout());
    }

    IEnumerator DisableShopLayout()
    {
        yield return new WaitForEndOfFrame();
        HorizontalLayoutGroup layout = shopContainer.GetComponent<HorizontalLayoutGroup>();
        if (layout != null)
            layout.enabled = false;
    }

    // --- BUY LOGIC ---
    public void TryBuyCard(BaseCard card, GameObject slotObject)
    {
        int gold = Inventaire.Instance != null ? Inventaire.Instance.currentGold : 0;

        // Check if player can afford the card
        if (gold >= card.cost)
        {
            Debug.Log($"Bought {card.cardName} for {card.cost}");

            // 1. Pay Gold
            if (Inventaire.Instance != null)
                Inventaire.Instance.currentGold -= card.cost;

            UpdateGoldUI();

            // 2. Add to Deck/Hand
            deckManager.AddCardToDeck(card);

            // 3. Destroy Shop Slot (leave empty gap)
            Destroy(slotObject);
        }
        else
        {
            Debug.Log($"Not enough gold! Need {card.cost}, have {gold}");
        }
    }

    // Helper to keep UI in sync
    void UpdateGoldUI()
    {
        if (goldText != null && Inventaire.Instance != null)
        {
            goldText.text = Inventaire.Instance.currentGold + "g";
        }
    }
}