using UnityEngine;
using System.Collections;

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
        unit.StunCheck = true;
        //unit.enabled = false;
        yield return new WaitForSeconds(duration);
        unit.StunCheck = false;
        //unit.enabled = true;
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
        Debug.Log("넉백 확인");
    }

    private static IEnumerator nockbackCoroutine(Unit unit, Vector3 direction, float strength, float duration)
    {
        // 넉백 시작점·종료점 계산
        Vector3 startPos = unit.transform.position;
        Vector3 endPos = startPos + direction.normalized * strength;

        // 이동과 공격 비활성화
        unit.StunCheck = true;

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
        unit.StunCheck = false;
    }
}
