using UnityEngine;

[CreateAssetMenu(fileName = "NewUnit", menuName = "BattleSystem/UnitData")]
public class UnitData : BaseCard
{
    public Sprite icon;
    public GameObject unitPrefab;
    [HideInInspector]
    public UnitStats currentStats;
    public int clicksRequiredToSpawn;
}