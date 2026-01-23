using UnityEngine;

[CreateAssetMenu(fileName = "NewUnit", menuName = "GameJam/UnitData")]
public class UnitData : ScriptableObject
{
    [Header("Infos de base")]
    public string unitName;
    public Sprite icon; // L'image affichée sur la carte
    
    [Header("Gameplay")]
    public GameObject unitPrefab; // Le bonhomme qui va spawn
    public int clicksRequiredToSpawn; // Ex: 10, 50, 100
}