using Unity.VisualScripting;
using UnityEngine;

public class Unit : MonoBehaviour
{
    //public int Faction; // 아군 = 0 적군 = 1

    //public float Damage;
    //public float Hp;
    //public float Speed;
    //public float AttackSpeed;
    //public float AttackRange;

    public UnitData unitData;
    public Health HP_unit;

    private string targetTag;
    private bool inCombat = false;
    private float attackTimer = 0f;

    private Vector3 Direction;

    private Health targetUnit;
    [SerializeField]private Unit targetUnit_unitcode; // 이거 굳이긴 한데 귀찮으니까

    void Start()
    {
        targetTag = (unitData.Faction == 0) ? "Enemy" : "Ally"; // 공격 목표 지정

        HP_unit.HP = unitData.Hp;
        attackTimer = 0f;
    }

    void Update()
    {
        DetectTarget();// 공격범위 탐지

        if (!inCombat)
        {
            Move();
        }
        else
        {
            Attack();
        }
    }

    void FixedUpdate()
    {
        // Faction에 따른 이동 방향 설정
        Direction = (unitData.Faction == 0) ? Vector3.right : Vector3.left;
    }

    void Move()
    {
        transform.Translate(Direction * unitData.Speed * Time.deltaTime, Space.World);
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
        //Unit closestUnit = null;
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
                //closestUnit = col.GetComponent<Unit>();
            }
        }

        if (closestHp != null)
        {
            targetUnit = closestHp;
            //targetUnit_unitcode = closestUnit;
            inCombat = true;
        }
        else
        {
            targetUnit = null;
            //targetUnit_unitcode = null;
            inCombat = false;
        }
    }

    void Attack()
    {
        if (targetUnit == null) inCombat = false;

        attackTimer += Time.deltaTime;
        if (attackTimer >= unitData.AttackSpeed)
        {
            targetUnit.TakeDamage(unitData.Damage);
            attackTimer = 0f;
        }
    }

    // 사정거리 시각화
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Direction * unitData.AttackRange);
    }
}
