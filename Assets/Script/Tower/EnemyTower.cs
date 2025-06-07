using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTower : MonoBehaviour
{
    [Header("Enemy")]
    public UnitData[] allEnemyUnits;

    [Header("Setting")]
    public float HP;
    public Health HP_Tower;

    [SerializeField] private EnemySpawn_Array[] Spawns;
    [SerializeField] private GameObject Enemy_body;
    [SerializeField] private Transform EnemySpawnPos;

    // 이름 → UnitData 빠른 조회용
    private Dictionary<string, UnitData> unitLookup;

    private void Awake()
    {
        HP_Tower.currentHP = HP;

        // UnitData 딕셔너리 초기화
        unitLookup = new Dictionary<string, UnitData>(allEnemyUnits.Length);
        foreach (var data in allEnemyUnits)
        {
            if (data != null && !string.IsNullOrEmpty(data.name) && !unitLookup.ContainsKey(data.name))
                unitLookup.Add(data.name, data);
        }
    }

    private void Start()
    {
        for (int i = 0; i < Spawns.Length; i++)
        {
            for (int u = 0; u < Spawns[i].E_name.Length; u++)
            {
                string unitName = Spawns[i].E_name[u];
                StartCoroutine(SpawnUnitLoop(i, unitName));
            }
        }
    }

    private IEnumerator SpawnUnitLoop(int spIndex, string unitName)
    {
        var cfg = Spawns[spIndex];
        // 이전 HP 임계치는 여전히 참조
        float prevThreshold = (spIndex > 0) ? Spawns[spIndex - 1].HP_Status : 100f;

        while (HP_Tower.currentHP > 0f)
        {
            // 현재 체력 %
            float hpPercent = HP_Tower.currentHP / HP * 100f;
            bool inRange = (spIndex == 0)
                ? (hpPercent <= prevThreshold && hpPercent >= cfg.HP_Status)
                : (hpPercent < prevThreshold && hpPercent >= cfg.HP_Status);

            if (inRange && unitLookup.TryGetValue(unitName, out var data))
            {
                // data에 정의된 min/max 로 랜덤 대기
                float wait = Random.Range(data.MinSpawnTime, data.MaxSpawnTime);
                Debug.Log($"[{unitName}] will spawn in {wait:F2}s");
                yield return new WaitForSeconds(wait);

                // 소환
                var go = Instantiate(data.UnitBody, EnemySpawnPos.position, Quaternion.identity);
                if (go.TryGetComponent<Unit>(out var unitComp))
                    unitComp.unitData = data;
            }
            else
            {
                // 범위 밖이거나 data 누락 시 매 프레임 대기
                yield return null;
            }
        }
    }
}
