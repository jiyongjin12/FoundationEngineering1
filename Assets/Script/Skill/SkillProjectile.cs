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
        // 이미 한 번 처리했다면 무시
        if (hasCollided)
            return;

        // 지정된 진영 태그가 아니면 무시
        if (!other.CompareTag(attacker.targetTag))
            return;

        // 이제 첫 충돌을 처리
        hasCollided = true;

        // 1) 첫 번째 목표에 데미지·효과 적용
        var targetHealth = other.GetComponent<Health>();
        if (targetHealth != null)
        {
            attacker.ApplySkillEffect(targetHealth, damage);
        }

        // 2) 범위 공격일 때만 추가 처리
        if (isArea)
        {
            // 범위 내 모든 적 수집
            Collider[] hits = Physics.OverlapSphere(transform.position, areaRadius);
            var extras = new List<Health>();

            foreach (var hit in hits)
            {
                // 자신(첫 타겟)은 건너뛰고, 진영 태그 확인
                if (!hit.CompareTag(attacker.targetTag) || hit.gameObject == other.gameObject)
                    continue;

                var h = hit.GetComponent<Health>();
                if (h != null)
                    extras.Add(h);
            }

            // 거리 기준 오름차순 정렬
            extras.Sort((a, b) =>
                Vector3.SqrMagnitude(a.transform.position - transform.position)
                    .CompareTo(Vector3.SqrMagnitude(b.transform.position - transform.position)));

            // attackCount-1 만큼만 추가 공격
            int needed = attackCount - 1;
            for (int i = 0; i < Mathf.Min(needed, extras.Count); i++)
            {
                attacker.ApplySkillEffect(extras[i], damage);
            }
        }

        // 충돌 처리 후 파괴
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
