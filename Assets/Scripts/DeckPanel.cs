using UnityEngine;

public class DeckPanel : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject cardPrefab;

    void Start()
    {
        var inventory = FindFirstObjectByType<Inventaire>();

        if (inventory == null)
        {
            Debug.LogError("[DeckPanel] Inventaire not found!");
            return;
        }

        if (inventory.UnitStats == null || inventory.UnitStats.Count == 0)
        {
            Debug.LogWarning("[DeckPanel] Inventory is empty! Adding default ant.");
            if (inventory.fourmi != null)
            {
                inventory.UnitStats = new System.Collections.Generic.List<UnitData> { inventory.fourmi };
            }
            else
            {
                Debug.LogError("[DeckPanel] Default ant (fourmi) is not assigned in Inventaire!");
                return;
            }
        }

        foreach (var unitStat in inventory.UnitStats)
        {
            if (unitStat == null)
            {
                Debug.LogWarning("[DeckPanel] Skipping null unit in inventory.");
                continue;
            }

            GameObject card = Instantiate(cardPrefab, this.transform);
            CardController controller = card.GetComponent<CardController>();

            if (controller != null)
            {
                controller.Initialize(unitStat);
            }
            else
            {
                Debug.LogError("[DeckPanel] CardController not found on instantiated card prefab!");
            }
        }
    }
}
