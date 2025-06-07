using UnityEngine;
using System.Collections.Generic;

public class SkillProjectile : MonoBehaviour
{
    float damage;
    bool isArea;
    float areaRadius;
    Vector3 direction;
    float speed;
    Unit attacker;
    int attackCount;
    private bool hasCollided = false;

    public void Initialize(float damage, bool isArea, float areaRadius, Vector3 direction, float speed, Unit attacker, int attackCount)
    {
        this.damage = damage;
        this.isArea = isArea;
        this.areaRadius = areaRadius;
        this.direction = direction;
        this.speed = speed;
        this.attacker = attacker;
        this.attackCount = Mathf.Max(1, attackCount);

        hasCollided = false;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasCollided) return;

        bool isTarget = false;
        foreach (var tag in attacker.targetTags)
            if (other.CompareTag(tag)) { isTarget = true; break; }
        if (!isTarget) return;

        hasCollided = true;

        if (isArea)
        {
            // 범위 AoE: 충돌 지점 기준
            Collider[] hits = Physics.OverlapSphere(transform.position, areaRadius);
            var targets = new List<Health>();
            foreach (var hit in hits)
            {
                foreach (var tag in attacker.targetTags)
                    if (hit.CompareTag(tag))
                    {
                        var h = hit.GetComponent<Health>();
                        if (h != null) targets.Add(h);
                        break;
                    }
            }

            targets.Sort((a, b) =>
                Vector3.SqrMagnitude(a.transform.position - transform.position)
                  .CompareTo(Vector3.SqrMagnitude(b.transform.position - transform.position))
            );

            int applyCount = Mathf.Min(attackCount, targets.Count);
            for (int i = 0; i < applyCount; i++)
                attacker.ApplySkillEffect(targets[i], damage);

            targets.Clear();
        }
        else
        {
            // 단일
            var targetHealth = other.GetComponent<Health>();
            if (targetHealth != null)
                attacker.ApplySkillEffect(targetHealth, damage);
        }

        Destroy(gameObject);
    }

    // 디버그용: 도착 시점 범위 시각화
    void OnDrawGizmosSelected()
    {
        if (isArea)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, areaRadius);
        }
    }
}
