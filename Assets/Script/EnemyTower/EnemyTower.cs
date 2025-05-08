using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTower : MonoBehaviour
{
    [Header("Enemy")]
    public UnitData[] allEnemyUnits;

    [Header("Setting")]
    public float HP;

    [SerializeField] private float CurrentHp;
    [SerializeField] private EnemySpawn_Array[] Spawns;
    [SerializeField] private GameObject Enemy_body;
    [SerializeField] private Transform EnemySpawnPos;

    // 이름 → UnitData 빠른 조회용
    private Dictionary<string, UnitData> unitLookup;

    private void Awake()
    {
        CurrentHp = HP;

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
            StartCoroutine(SpawnRoutine(i));
        }
    }

    private IEnumerator SpawnRoutine(int index) // 어우 코드 더러워랑
    {
        var cfg = Spawns[index];
        float prevThreshold = (index > 0) ? Spawns[index - 1].HP_Status : 100f;

        while (CurrentHp > 0f)
        {
            float hpPercent = CurrentHp / HP * 100f;
            bool inRange = false;

            if (index == 0)
            {
                // maxHP 이하, cfg.HP_Status 이상
                inRange = (hpPercent <= prevThreshold && hpPercent >= cfg.HP_Status);
            }
            else
            {
                // prevThreshold 미만, cfg.HP_Status 이상
                inRange = (hpPercent < prevThreshold && hpPercent >= cfg.HP_Status);
            }

            if (inRange)
            {
                yield return new WaitForSeconds(cfg.Second);
                foreach (var unitName in cfg.E_name)
                    TrySpawnUnit(unitName);
            }
            else
            {
                yield return null;
            }
        }
    }

    private void TrySpawnUnit(string unitName)
    {
        if (!unitLookup.TryGetValue(unitName, out var data))
        {
            Debug.LogWarning($"UnitData not found for name '{unitName}'");
            return;
        }

        // 적 유닛 생성
        var go = Instantiate(Enemy_body, EnemySpawnPos.position, Quaternion.identity);
        if (go.TryGetComponent<Unit>(out var unitComp))
        {
            unitComp.unitData = data;
        }
    }

    public void TakeDamage(float damage)
    {
        CurrentHp -= damage;
        if (CurrentHp <= 0f)
        {
            Debug.Log("Enemy Tower Destroyed");
            Destroy(gameObject);
        }
    }
}
