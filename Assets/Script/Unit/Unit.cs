using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;

public class Unit : MonoBehaviour
{
    public UnitData unitData;
    public Health HP_unit;

    public float finalDamage;

    public bool inCombat = false;
    [HideInInspector] public string[] targetTags;
    private float attackTimer = 0f;

    private Vector3 Direction;

    private Health targetUnit;
    //[SerializeField]
    private UnitSkillData unitSkill;

    #region 상태이상 값들                                     
    [HideInInspector] public float SlowSpeed = 1;          // 관련 이펙트들은 지정한 값이 아니라면 발동하게 해두면 편하겠구먼
    [HideInInspector] public bool StunCheck;
    [HideInInspector] public bool NuckBack_Sutn = false;
    #endregion

    #region 공속 감소 
    public Coroutine atkSpeedDownCoroutine;
    public float AtkSpeedMultiplier = 1;
    public float FinalAtkTimer;
    #endregion


    public bool check;

    void Start()
    {
        this.name = unitData.name;

        if (unitData.Faction == 1)
        {
            this.tag = "Enemy";
            targetTags = new[] { "Ally", "Player" };
        }   
        else
        {
            this.tag = "Ally";
            targetTags = new[] { "Enemy", "Tower" };
        }

        HP_unit.HP = unitData.Hp;
        attackTimer = 0f;

        unitSkill = unitData.UnitDefaultSkill;


        SlowSpeed = 1;
        StunCheck = false;
        AtkSpeedMultiplier = 1;
    }

    void Update()
    {
        DetectTarget();// 공격범위 탐지

        if (!StunCheck && !NuckBack_Sutn)
        {
            if (!inCombat)
            {
                Move();
            }
            else
            {
                Attack();
            }
        }

        check = HasAnyStatusEffect();
    }

    void FixedUpdate()
    {
        // Faction에 따른 이동 방향 설정
        Direction = (unitData.Faction == 0) ? Vector3.right : Vector3.left;
    }

    void Move()
    {
        transform.Translate(Direction * unitData.Speed * SlowSpeed * Time.deltaTime, Space.World);
    }

    void DetectTarget()
    {
        float range = unitData.AttackRange;
        Vector3 origin = transform.position;

        // 이미 타겟이 있으면 유효성 검사
        if (targetUnit != null)
        {
            Vector3 toTarget = targetUnit.transform.position - origin;
            float dist = toTarget.magnitude;
            float dot = Vector3.Dot(toTarget.normalized, Direction);
            if (dist <= range && dot > 0)
            {
                inCombat = true;
                return; // 기존 타겟 유지
            }
        }

        // 새 타겟 탐색
        Collider[] hits = Physics.OverlapSphere(origin, range);
        Health closestHp = null;
        float minDist = float.MaxValue;

        foreach (var col in hits)
        {
            bool isTarget = false;
            foreach (var tag in targetTags)
                if (col.CompareTag(tag)) { isTarget = true; break; }
            if (!isTarget) continue;

            Vector3 toCol = col.transform.position - origin;
            if (Vector3.Dot(toCol.normalized, Direction) <= 0)
                continue; // 뒤 또는 옆 제외

            float d = toCol.sqrMagnitude;
            if (d < minDist)
            {
                // 가장 가까운 적 선택
                minDist = d;
                closestHp = col.GetComponent<Health>();
            }
        }

        if (closestHp != null)
        {
            targetUnit = closestHp;
            inCombat = true;
        }
        else
        {
            targetUnit = null;
            inCombat = false;
        }
    }

    void Attack()
    {
        if (targetUnit == null) inCombat = false;

        attackTimer += Time.deltaTime;
        FinalAtkTimer = unitData.AttackSpeed * AtkSpeedMultiplier;
        if (attackTimer >= FinalAtkTimer)
        {
            UseSkill();
            attackTimer = 0f;
        }
    }

    void UseSkill()
    {
        // 물리 데미지 적용
        finalDamage = unitSkill.Damage; // <ㅡ 여기에 치명타 부분 추가해야함

        if (Random.value < unitSkill.Critical_probability / 100f)
        {
            finalDamage *= unitSkill.Critical_damage / 100f;
            Debug.Log("치명타!");
        }

        // 근거리 처리
        if (unitSkill.RangeType == RangeType.Melee)
        {
            // 근거리 단일
            if (!unitSkill.RangeAttackCheck)
            {
                ApplySkillEffect(targetUnit, finalDamage);
            }
            // 근거리 범위
            else
            {
                var targets = new List<Health>();
                targets.Add(targetUnit);

                Collider[] hits = Physics.OverlapSphere(transform.position, unitData.AttackRange);
                var extras = new List<Health>();
                foreach (var hit in hits)
                {
                    bool isTarget = false;
                    foreach (var tag in targetTags)
                        if (hit.CompareTag(tag)) { isTarget = true; break; }

                    if (!isTarget || hit.gameObject == targetUnit.gameObject) 
                        continue;

                    var h = hit.GetComponent<Health>(); 
                    if (h != null) 
                        extras.Add(h);
                }
                extras.Sort((a, b) =>
                    Vector3.SqrMagnitude(a.transform.position - targetUnit.transform.position)
                        .CompareTo(Vector3.SqrMagnitude(b.transform.position - targetUnit.transform.position))
                );
                int needed = unitSkill.AttackCount - 1;
                for (int i = 0; i < Mathf.Min(needed, extras.Count); i++)
                    targets.Add(extras[i]);

                foreach (var h in targets)
                    ApplySkillEffect(h, finalDamage);
            }
        }
        // 원거리 처리
        else if (unitSkill.RangeType == RangeType.Ranged)
        {
            // 투사체 미설정 시 그냥 단일 처리
            if (unitSkill.ProjectilePrefab == null)
            {
                ApplySkillEffect(targetUnit, finalDamage);
            }
            // 투사체를 이용한 단일/범위 처리
            else
            {
                var proj = Instantiate(unitSkill.ProjectilePrefab, transform.position, Quaternion.identity);
                proj.GetComponent<SkillProjectile>().Initialize(
                    damage: finalDamage,
                    isArea: unitSkill.RangeAttackCheck,
                    areaRadius: unitSkill.RangeDiameter * 0.5f,
                    direction: (targetUnit.transform.position - transform.position).normalized,
                    speed: unitSkill.ProjectileSpeed,
                    attacker: this,
                    attackCount: unitSkill.AttackCount
                );
            }
        }
    }

    public void ApplySkillEffect(Health target, float damage) // 상태효과
    {
        target.TakeDamage(damage);

        if (target.CompareTag("Player") || target.CompareTag("Tower")) // Player/Tower 태그 상태이상 없이 데미지만
            return;

        if (unitSkill.UseSkillEffect && Random.value > unitSkill.ProcChance / 100) // 스킬 사용 확률
            return;

        if (unitSkill.Nockback)
            StatusEffects.ApplyKnockback(target, Direction, unitSkill.NockbackStrength);
        if (unitSkill.Slow)
            StatusEffects.ApplySlow(target, unitSkill.SlowRatio, unitSkill.SlowDuration);
        if (unitSkill.Stun)
            StatusEffects.ApplyStun(target, unitSkill.StunDuration);
        if (unitSkill.AtkSpeedDown)
            StatusEffects.ApplyAttackSpeedDown(target, unitSkill.AtkSpeedDownRatio, unitSkill.AtkSpeedDownDuration);
        if (unitSkill.ClearEffects)
            StatusEffects.TryCleanseNearbyAllies(this, unitSkill.EffectRemovalRange, unitSkill.Unitcount);
    }

    public bool HasAnyStatusEffect()  // 상태이상 확인
    {
        return atkSpeedDownCoroutine != null || SlowSpeed < 1f || StunCheck;
    }

    public void ClearAllStatusEffects()  // 상태이상 해제
    {
        if (atkSpeedDownCoroutine != null)
        {
            StopCoroutine(atkSpeedDownCoroutine);
            atkSpeedDownCoroutine = null;
        }
        AtkSpeedMultiplier = 1f;

        SlowSpeed = 1f;

        StunCheck = false;
       
    }

    void OnDrawGizmosSelected()
    {
        if (unitSkill != null && unitSkill.RangeType == RangeType.Melee && unitSkill.RangeAttackCheck)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, unitData.AttackRange);
        }

        // 유닛 사정거리 시각화
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Direction * unitData.AttackRange);
        Gizmos.DrawWireSphere(transform.position, unitData.AttackRange);
    }
}
