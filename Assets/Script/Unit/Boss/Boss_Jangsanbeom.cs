using System.Collections;
using UnityEngine;

public class Boss_Jangsanbeom : MonoBehaviour
{
    public UnitData ChildGhost;
    public Transform SpawnPos;

    public Unit Jangsanbeom;
    public int Count;

    public void Update()
    {
        if (Jangsanbeom.KillingUnit)
        {
            StartCoroutine(SpawnChildGost(Count));
        }
    }

    private IEnumerator SpawnChildGost(int count)
    {
        Jangsanbeom.KillingUnit = false;

        for (int i = 0; i < count; i++)
        {
            if (ChildGhost != null && ChildGhost.UnitBody != null)
            {
                var go = Instantiate(ChildGhost.UnitBody, SpawnPos.position, Quaternion.identity);
                if (go.TryGetComponent<Unit>(out var unitComp))
                {
                    unitComp.unitData = ChildGhost;
                }
            }
            yield return null;
        }
    }
}
