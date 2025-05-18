using UnityEngine;

public enum RangeType { Melee, Ranged }

[CreateAssetMenu(menuName = "Skill/UnitSkillData")]
public class UnitSkillData : ScriptableObject
{
    [Header("Attack Type")]
    public RangeType rangeType;

    [Header("Check Status Effectiveness")]
    public bool Slow;                   // 슬로우
    public bool Stun;                   // 스턴
    public bool nockback;               // 넉백

    [Header("Slow")]
    public float slowRatio;             // %
    public float slowDuration;          // 지속시간

    [Header("Stun")]
    public float stunDuration;          // 지속시간

    [Header("knockback")]
    public float nockbackStrength;      // 가하는 힘
    //public float nockbackDuration;    // 지속시간?

    [Header("Damage")]
    public float damage;
}
