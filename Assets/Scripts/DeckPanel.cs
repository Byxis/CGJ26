using UnityEngine;

public class DeckPanel : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject cardPrefab;

    void Start()
    {
        FindFirstObjectByType<Inventaire>().UnitStats.ForEach(unitStat =>
        {
            GameObject card = Instantiate(cardPrefab, this.transform);
            card.GetComponent<CardController>().Initialize(unitStat);
        });
    }
}
