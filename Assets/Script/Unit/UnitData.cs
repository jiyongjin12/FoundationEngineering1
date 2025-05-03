using UnityEngine;

[CreateAssetMenu(menuName = "Unit/UnitData")]
public class UnitData : ScriptableObject
{
    public int Faction; // 아군 = 0 적군 = 1

    public float Damage;
    public float Hp;
    public float Speed;
    public float AttackSpeed;
    public float AttackRange;
}
