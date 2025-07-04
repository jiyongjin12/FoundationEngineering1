using UnityEngine;

public enum RangeType { Melee, Ranged }

[CreateAssetMenu(menuName = "Skill/UnitSkillData")]
public class UnitSkillData : ScriptableObject
{
    [Header("근접/원거리")]
    public RangeType RangeType;

    public string ID;

    [Space (20)]
    [Header("딜")]
    public float Damage;

    public float Critical_probability; // 치명타 확률
    public float Critical_damage; // 치명타 피해

    [Header("범위")]
    public bool RangeAttackCheck;        // 범위 공격인지 여부
    public float RangeDiameter;          // 범위 공격 지름
    public int AttackCount;              // 공격 유닛 수

    [Header("스킬 발동 확률")]
    public bool UseSkillEffect;          // 스킬 발동 확률을 사용할 것인지
    public float ProcChance;             // 스킬 발동 확률 (%)

    [Header("원거리 여부")]
    public GameObject ProjectilePrefab;  // 원거리 공격 시 던질 오브젝트
    public float ProjectileSpeed;        // 발사체 속도


    [Space(20)]
    [Header("상태 이상 세팅")]
    public bool Slow;                            // 슬로우
    public bool Stun;                            // 스턴
    public bool Nockback;                        // 넉백
    public bool AtkSpeedDown;                    // 공속다운

    public bool ClearEffects;                    // 상태 이상 제거


    [Header("슬로우")]
    public float SlowRatio;                      // %
    public float SlowDuration;                   // 지속시간

    [Header("스턴")]
    public float StunDuration;                   // 지속시간

    [Header("넉백")]
    public float NockbackStrength;               // 가하는 힘

    [Header("공속다운")]
    public float AtkSpeedDownRatio;              // %
    public float AtkSpeedDownDuration;           // 지속시간

    [Header("상태이상 제거 범위")]
    public float EffectRemovalRange;             // 범위
    public int Unitcount;                        // 제거 수
}
