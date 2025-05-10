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
    private Health targetUnit;
    private float attackTimer = 0f;

    private Vector3 Direction;

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
        RaycastHit hit;

        // 범위 탐지
        if (Physics.Raycast(transform.position, Direction, out hit, unitData.AttackRange))
        {
            if (hit.collider.CompareTag(targetTag))
            {
                // 타겟 발견, 전투 상태 전환
                if (!inCombat)
                {
                    inCombat = true;
                    targetUnit = hit.collider.GetComponent<Health>();
                }
            }
            
        }
        else
        {
            // 사정거리에 없으면 전투 상태 종료
            inCombat = false;
            targetUnit = null;
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
