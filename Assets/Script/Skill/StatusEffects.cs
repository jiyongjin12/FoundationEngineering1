using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class StatusEffects
{
    public static void ApplySlow(Health targetHealth, float ratio, float duration)
    {
        if (targetHealth.TryGetComponent<Unit>(out var unit))
        {
            unit.StartCoroutine(SlowCoroutine(unit, ratio, duration));
        }
    }

    private static IEnumerator SlowCoroutine(Unit unit, float ratio, float duration)
    {
        Debug.Log("슬로우 확인");
        FXManager.Instance.PlayEffect("FX_Slow", unit.transform, duration);
        unit.ChangeColorEffect(unit.gameObject, Color.blue, duration);
        unit.SlowSpeed = ratio / 100;
        yield return new WaitForSeconds(duration);
        unit.SlowSpeed = 1;
    }


    public static void ApplyStun(Health targetHealth, float duration)
    {
        if (targetHealth.TryGetComponent<Unit>(out var unit))
        {
            unit.StartCoroutine(StunCoroutine(unit, duration));
        }
    }

    private static IEnumerator StunCoroutine(Unit unit, float duration)
    {
        FXManager.Instance.PlayEffect("FX_Sturn", unit.transform, duration);
        unit.ChangeColorEffect(unit.gameObject, Color.yellow, duration);

        unit.StunCheck = true;
        yield return new WaitForSeconds(duration);
        unit.StunCheck = false;
        Debug.Log("스턴 확인");
    }


    public static void ApplyKnockback(Health targetHealth, Vector3 direction, float strength)
    {
        if (targetHealth.TryGetComponent<Unit>(out var unit))
        {
            unit.StartCoroutine(nockbackCoroutine(unit, direction, strength, .3f));
        }
        else if (targetHealth.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForce(direction.normalized * strength, ForceMode.Impulse);
        }
    }

    private static IEnumerator nockbackCoroutine(Unit unit, Vector3 direction, float strength, float duration)
    {
        FXManager.Instance.PlayEffect("FX_Knockback", unit.transform, duration);
        unit.ChangeColorEffect(unit.gameObject, Color.yellow, duration);

        // 넉백 시작점·종료점 계산
        Vector3 startPos = unit.transform.position;
        Vector3 endPos = startPos + direction.normalized * strength;

        // 이동과 공격 비활성화
        unit.NuckBack_Sutn = true;

        float timer = 0f;
        while (timer < duration)
        {
            float t = timer / duration;
            unit.transform.position = Vector3.Lerp(startPos, endPos, t);
            timer += Time.deltaTime;
            yield return null;
        }

        // 최종 위치 보정
        unit.transform.position = endPos;

        // 원래 상태 복구
        unit.NuckBack_Sutn = false;
    }


    public static void ApplyAttackSpeedDown(Health targetHealth, float percent, float duration)
    {
        if (targetHealth.TryGetComponent<Unit>(out var unit))
        {
            if (unit.atkSpeedDownCoroutine != null)
            {
                unit.StopCoroutine(unit.atkSpeedDownCoroutine);
            }

            // 새 코루틴 시작 및 핸들 저장
            unit.atkSpeedDownCoroutine = unit.StartCoroutine(AttackSpeedDownCoroutine(unit, percent, duration));
        }
    }

    private static IEnumerator AttackSpeedDownCoroutine(Unit unit, float percent, float duration)
    {
        FXManager.Instance.PlayEffect("FX_Slow", unit.transform, duration);
        unit.ChangeColorEffect(unit.gameObject, Color.blue, duration);
        unit.AtkSpeedMultiplier = 1 + percent / 100f;

        yield return new WaitForSeconds(duration);

        unit.AtkSpeedMultiplier = 1f;
        unit.atkSpeedDownCoroutine = null;
    }

    public static void TryCleanseNearbyAllies(Unit self, float radius, int count)
    {
        radius = 200;
        Collider[] hits = Physics.OverlapSphere(self.transform.position, radius);
        List<Unit> affectedAllies = new List<Unit>();

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<Unit>(out var unit))
            {
                if (unit == self) continue;
                if (unit.CompareTag(self.tag) && unit.HasAnyStatusEffect())
                {
                    affectedAllies.Add(unit);
                }
            }
        }

        if (affectedAllies.Count == 0)
        {
            Debug.Log("주변에 상태이상 아군 없음, 자기 자신에게 해제 적용");
            self.ClearAllStatusEffects();
        }
        else
        {
            int actualCount = Mathf.Min(count, affectedAllies.Count);
            List<Unit> selectedTargets = new List<Unit>();

            while (selectedTargets.Count < actualCount)
            {
                int index = Random.Range(0, affectedAllies.Count);
                Unit chosen = affectedAllies[index];

                if (!selectedTargets.Contains(chosen))
                {
                    selectedTargets.Add(chosen);
                }
            }

            foreach (var unit in selectedTargets)
            {
                FXManager.Instance.PlayEffect("FX_Clean", unit.transform);
                unit.ChangeColorEffect(unit.gameObject, Color.white);
                unit.ClearAllStatusEffects();
            }
        }
    }
}
