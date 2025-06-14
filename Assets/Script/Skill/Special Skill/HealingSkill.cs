using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingSkill : MonoBehaviour
{
    public float interval = 5f; // 간격(초)

    [Range(0f, 100f)]
    public float healPercent = 8f; // %

    public float range = 5f; // 범위

    public int maxTargets = 3; // 회복할 최대 아군 수

    private void Start()
    {
        StartCoroutine(HealRoutine());
    }

    private IEnumerator HealRoutine()
    {
        // 무한 루프: interval마다 치유
        while (true)
        {
            yield return new WaitForSeconds(interval);

            // 범위 안의 콜라이더들 검사
            Collider[] hits = Physics.OverlapSphere(transform.position, range);
            List<Health> alliesToHeal = new List<Health>();

            // 태그가 "Ally"인 유닛을 수집
            foreach (var hit in hits)
            {
                if (!hit.CompareTag("Ally"))
                    continue;

                var healthComp = hit.GetComponent<Health>();
                if (healthComp == null)
                    continue;

                if (healthComp.currentHP < healthComp.HP)
                {
                    alliesToHeal.Add(healthComp);
                    if (alliesToHeal.Count >= maxTargets)
                        break;
                }
            }

            // 수집된 아군 유닛들을 치유
            foreach (var allyHealth in alliesToHeal)
            {
                HealUnit(allyHealth);
            }
        }
    }

    private void HealUnit(Health targetHealth)
    {
        FXManager.Instance.PlayEffect("FX_Heal", targetHealth.transform.transform);

        // Health 컴포넌트가 maxHP와 currentHP를 관리한다고 가정
        float maxHP = targetHealth.HP;
        float currentHP = targetHealth.currentHP;
        float healAmount = maxHP * (healPercent / 100f);

        float newHP = Mathf.Min(currentHP + healAmount, maxHP);
        targetHealth.currentHP = newHP;
    }

    // 디버그: 씬에서 범위를 시각적으로 확인하기 위한 기즈모
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
