using UnityEngine;

[CreateAssetMenu(fileName = "NewUnit", menuName = "BattleSystem/UnitData")]
public class UnitData : ScriptableObject
{
    public Sprite icon;
    public GameObject unitPrefab;

    public int clicksRequiredToSpawn;
}