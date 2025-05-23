using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;

public class Unit : MonoBehaviour
{
    public UnitData unitData;
    public Health HP_unit;

    public bool inCombat = false;
    [HideInInspector] public string targetTag;
    private float attackTimer = 0f;

    private Vector3 Direction;

    private Health targetUnit;
    //[SerializeField]
    private UnitSkillData unitSkill;

    #region 상태이상 값들
    [HideInInspector] public float SlowSpeed = 1;
    [HideInInspector] public bool StunCheck;
    #endregion

    void Start()
    {
        targetTag = (unitData.Faction == 0) ? "Enemy" : "Ally"; // 공격 목표 지정

        HP_unit.HP = unitData.Hp;
        attackTimer = 0f;

        unitSkill = unitData.UnitDefaultSkill;


        SlowSpeed = 1;
        StunCheck = false;
    }

    void Update()
    {
        DetectTarget();// 공격범위 탐지

        if (!StunCheck)
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
        #region 레이케스트-주석-관통x
        //RaycastHit hit;

        //// 범위 탐지
        //if (Physics.Raycast(transform.position, Direction, out hit, unitData.AttackRange))
        //{
        //    if (hit.collider.CompareTag(targetTag))
        //    {
        //        // 타겟 발견, 전투 상태 전환
        //        if (!inCombat)
        //        {
        //            inCombat = true;
        //            targetUnit = hit.collider.GetComponent<Health>();
        //        }
        //    }

        //}
        //else
        //{
        //    // 사정거리에 없으면 전투 상태 종료
        //    inCombat = false;
        //    targetUnit = null;
        //}
        #endregion

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
            if (!col.CompareTag(targetTag))
                continue;

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
        if (attackTimer >= unitData.AttackSpeed)
        {
            UseSkill();
            attackTimer = 0f;
        }
    }

    void UseSkill()
    {
        // 물리 데미지 적용
        float finalDamage = unitSkill.Damage; // <ㅡ 여기에 치명타 부분 추가해야함
        //targetUnit.TakeDamage(finalDamage);

        // 2) 원거리일 경우 투사체 발사, 근접은 즉시 효과 적용
        if (unitSkill.RangeType == RangeType.Ranged && unitSkill.ProjectilePrefab != null)
        {
            // 투사체 인스턴스화
            var proj = Instantiate(unitSkill.ProjectilePrefab, transform.position, Quaternion.identity);

            // 투사체 초기화
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
        else
        {
            // 근접 혹은 투사체 없음 → 즉시 상태이상 효과만 적용
            ApplySkillEffect(targetUnit, finalDamage);
        }

        //Debug.Log("공격");
    }

    //void UseSkill()
    //{
    //    // 물리 데미지 적용
    //    float finalDamage = unitSkill.Damage; // <ㅡ 여기에 치명타 부분 추가해야함
    //    targetUnit.TakeDamage(finalDamage);

    //    ApplySkillEffect();

    //    //Debug.Log("공격");
    //}

    public void ApplySkillEffect(Health target, float damage) // 상태효과
    {
        target.TakeDamage(damage);

        if (unitSkill.UseSkillEffect && Random.value > unitSkill.ProcChance / 100) // 스킬 사용 확률
            return;


        if (unitSkill.Nockback)
            StatusEffects.ApplyKnockback(targetUnit, Direction, unitSkill.NockbackStrength);
        if (unitSkill.Slow)
            StatusEffects.ApplySlow(targetUnit, unitSkill.SlowRatio, unitSkill.SlowDuration);
        if (unitSkill.Stun)
            StatusEffects.ApplyStun(targetUnit, unitSkill.StunDuration);
    }


    // 사정거리 시각화
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Direction * unitData.AttackRange);
    }
}
