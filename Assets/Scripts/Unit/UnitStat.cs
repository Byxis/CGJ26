using UnityEngine;

[CreateAssetMenu(fileName = "NewUnit", menuName = "BattleSystem/UnitStats")]
public class UnitStats : ScriptableObject
{
    public string unitName;
    public float maxHealth;
    public float speed;
    public float damage;
    public float attackRange;
    public float attackCooldown;
}