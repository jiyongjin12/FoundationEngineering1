using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class Unit : MonoBehaviour
{
    public UnitData unitData;
    public Health HP_unit;

    public float finalDamage;

    public bool inCombat = false;
    [HideInInspector] public string[] targetTags;
    public float attackTimer = 0f;

    private Vector3 Direction;

    private Health targetUnit;
    //[SerializeField]
    private UnitSkillData unitSkill;

    #region 상태이상 값들                                     
    [HideInInspector] public float SlowSpeed = 1;          // 관련 이펙트들은 지정한 값이 아니라면 발동하게 해두면 편하겠구먼
    [HideInInspector] public bool StunCheck;
    [HideInInspector] public bool NuckBack_Sutn = false;
    [HideInInspector] public bool CriticalCheck; // 크리티컬 체크
    #endregion

    #region 공속 감소 
    public Coroutine atkSpeedDownCoroutine;
    public float AtkSpeedMultiplier = 1;
    public float FinalAtkTimer;
    #endregion

    public bool check;
    private float currentFlashAmount;

    public Animator Animation;

    // 보스
    public bool KillingUnit;
    public float FinalRange = 0;

    [SerializeField] private Image hpImage;
    [SerializeField] private Image hpEffectImage;

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
        FinalRange = 0;
    }


    void Update()
    {
        DetectTarget();// 공격범위 탐지

        if (!StunCheck && !NuckBack_Sutn)
        {
            if (!inCombat)
            {
                Move();
                Animation.SetBool("IsAttack", false);
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

        // HP바
        if (hpImage != null)
        {
            hpImage.fillAmount = HP_unit.currentHP / HP_unit.HP;
            if (hpEffectImage.fillAmount > hpImage.fillAmount)
                hpEffectImage.fillAmount -= 0.05f;
            else
                hpEffectImage.fillAmount = hpImage.fillAmount;
        }
    }

    void Move()
    {
        transform.Translate(Direction * unitData.Speed * SlowSpeed * Time.deltaTime, Space.World);
    }

    void DetectTarget()
    {
        float range = unitData.AttackRange + FinalRange;
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
        Animation.SetBool("IsAttack", false);

        attackTimer += Time.deltaTime;
        FinalAtkTimer = unitData.AttackSpeed * AtkSpeedMultiplier;
        if (attackTimer >= FinalAtkTimer)
        {
            //UseSkill();
            Animation.SetBool("IsAttack", true);
        }
    }

    public void Attack_Anim()
    {
        UseSkill();  // 여기에 에니메이션?
        attackTimer = 0f;
    }

    public void UseSkill()
    {
        // 물리 데미지 적용
        finalDamage = unitSkill.Damage; // <ㅡ 여기에 치명타 부분 추가해야함

        if (Random.value < unitSkill.Critical_probability / 100f)
        {
            finalDamage *= unitSkill.Critical_damage / 100 + 1;
            CameraShake.Instance.Shake(2, 0.5f);
            CriticalCheck = true;
            Debug.Log("치명타!");
        }
        else
        {
            CriticalCheck = false;
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
                Debug.Log(unitData.AttackRange);
                Collider[] hits = Physics.OverlapSphere(transform.position, unitData.AttackRange);
                var candidates = new List<Health>();

                Debug.Log(hits);

                // 태그 & 전방 필터링
                foreach (var hit in hits)
                {
                    bool isTarget = false;
                    foreach (var tag in targetTags)
                        if (hit.CompareTag(tag)) { isTarget = true; break; }
                    if (!isTarget) continue;

                    Vector3 toHit = hit.transform.position - transform.position;
                    if (Vector3.Dot(toHit.normalized, Direction) <= 0)
                        continue; // 뒤/옆 제외

                    var h = hit.GetComponent<Health>();
                    if (h != null)
                        candidates.Add(h);
                }

                // 거리순 정렬
                candidates.Sort((a, b) =>
                    Vector3.SqrMagnitude(a.transform.position - transform.position)
                        .CompareTo(Vector3.SqrMagnitude(b.transform.position - transform.position))
                );

                // attackCount 만큼만 공격
                int applyCount = Mathf.Min(unitSkill.AttackCount, candidates.Count);
                for (int i = 0; i < applyCount; i++)
                {
                    ApplySkillEffect(candidates[i], finalDamage);
                }
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
        if (target == null)
        {
            return;
        }

        target.TakeDamage(damage);

        if (!CriticalCheck)
            UIManager.Instance.damageNumberPrefab.Spawn(target.transform.position, damage);
        else
            UIManager.Instance.damageCriNumberPrefab.Spawn(target.transform.position, damage);

        ChangeColorEffect(target.gameObject, Color.red);
        FXManager.Instance.PlayEffect("FX_Hit", target.gameObject.transform);
        SkillEffect(target.gameObject);

        if (target.currentHP <= 0f)
        {
            KillingUnit = true;
        }

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

    // 특정 몹들 공격 이펙트
    public void SkillEffect(GameObject target)
    {
        if (unitSkill == null) return;

        if (unitData.name == "Cat")
            FXManager.Instance.PlayLocalEffect("FX_Lightning", target.transform);

        if (unitData.name == "ChildGhost")
            FXManager.Instance.PlayEffect("FX_Bomb", gameObject.transform);

        if (unitData.name == "Monkey")
            FXManager.Instance.PlayLocalEffect("FX_Bomb", target.transform);

        if (unitData.name == "WindGod")
            FXManager.Instance.PlayLocalEffect("FX_WaterBomb", target.transform);

        if (unitData.name == "Dragon")
            FXManager.Instance.PlayLocalEffect("FX_ElectricBomb", target.transform);
    }

    // 히트 이펙트 or 힐 이펙트 or 상태이상 이펙트
    public IEnumerator ChangeColorEffectApply(GameObject target, Color color, float duration)
    {
        if (currentFlashAmount > 0) yield return null; // 중복 실행 방지

        Material material = target.GetComponentInChildren<SpriteRenderer>().material; // 코드 구데기인데 나중에 최적화

        currentFlashAmount = 0f;
        float elapsedTime = 0f;
        material.SetColor("_MainColor", color);

        while (elapsedTime < duration)
        {
            Debug.Log("히트" + currentFlashAmount);
            elapsedTime += Time.deltaTime;

            currentFlashAmount = Mathf.Lerp(1, 0, (elapsedTime / duration));
            material.SetFloat("_ColorAmount", currentFlashAmount);

            yield return null;
        }
    }

    public void ChangeColorEffect(GameObject target, Color color, float duration = 0.25f)
    {
        StartCoroutine(ChangeColorEffectApply(target.gameObject, color, duration));
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
