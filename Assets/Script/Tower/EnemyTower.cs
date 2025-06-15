using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyTower : MonoBehaviour
{
    [Header("Enemy Units (UnitData.name must match suffix, ID format: Enemy_<name>)")]
    public UnitData[] allEnemyUnits;

    [Header("Tower Health Component")]
    public Health HP_Tower;

    [Header("Spawn Position")]
    public Transform EnemySpawnPos;

    private Dictionary<string, UnitData> unitLookup;
    private List<SheetStageData> waveDataList;
    private HashSet<int> triggeredWaves = new HashSet<int>();

    void Awake()
    {
        Debug.Log("EnemyTower.Awake: Initializing lookup and stage data");

        // Build lookup: key = "Enemy_" + UnitData.name
        unitLookup = new Dictionary<string, UnitData>();
        foreach (var ud in allEnemyUnits)
        {
            if (ud == null || string.IsNullOrEmpty(ud.name)) continue;
            string key = "Enemy_" + ud.name;
            unitLookup[key] = ud;
            Debug.Log($"Lookup added: '{key}' -> UnitData '{ud.name}'");
        }
        Debug.Log($"Total lookup entries: {unitLookup.Count}");

        // Load stage waves
        var group = StageLoadManager.Instance.SelectedGroup;
        if (group == null || group.Waves == null)
        {
            Debug.LogError("EnemyTower: No StageGroup loaded!"); enabled = false; return;
        }
        waveDataList = group.Waves;
        Debug.Log($"Loaded StageGroup: series {group.StageSeries}, total waves = {waveDataList.Count}");

        // HP setup
        HP_Tower.HP = waveDataList[0].StageHealth;
        HP_Tower.currentHP = HP_Tower.HP;
        Debug.Log($"EnemyTower: HP set to {HP_Tower.HP}");
    }

    void Start()
    {
        // Begin wave check loop
        StartCoroutine(WaveCheckLoop());
    }

    private IEnumerator WaveCheckLoop()
    {
        while (HP_Tower.currentHP > 0f)
        {
            float currHP = HP_Tower.currentHP;
            for (int i = 0; i < waveDataList.Count; i++)
            {
                if (triggeredWaves.Contains(i)) continue;
                var wave = waveDataList[i];
                if (currHP <= wave.StageHealth)
                {
                    Debug.Log($"[Wave {i}] Triggered at HP={currHP:F1} ≤ {wave.StageHealth}");
                    triggeredWaves.Add(i);
                    // Start individual spawn loops
                    foreach (var enemyID in wave.EnemyType)
                    {
                        Debug.Log($"[Wave {i}] Preparing SpawnLoop for ID='{enemyID}'");
                        StartCoroutine(SpawnLoop(wave, enemyID));
                    }
                }
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator SpawnLoop(SheetStageData wave, string enemyID)
    {
        // Determine thresholds
        int idx = waveDataList.IndexOf(wave);
        float prevThreshold = idx > 0 ? waveDataList[idx - 1].StageHealth : float.MaxValue;
        float currThreshold = wave.StageHealth;
        float minTime = wave.MinSpawnTime;
        float maxTime = wave.MaxSpawnTime;

        Debug.Log($"SpawnLoop for '{enemyID}' started (wave {idx}): range ({currThreshold}, {prevThreshold}]");

        // Check UnitData lookup before looping
        if (!unitLookup.ContainsKey(enemyID))
        {
            Debug.LogError($"SpawnLoop: Lookup missing key '{enemyID}'. Available keys: {string.Join(",", unitLookup.Keys)}");
            yield break;
        }
        var data = unitLookup[enemyID];

        while (HP_Tower.currentHP > 0f)
        {
            float currHP = HP_Tower.currentHP;
            bool inRange = currHP <= prevThreshold && currHP >= currThreshold;
            Debug.Log($"SpawnLoop[{enemyID}]: currHP={currHP:F1}, inRange={inRange}");

            if (inRange)
            {
                float wait = Random.Range(minTime, maxTime);
                Debug.Log($"[{enemyID}] waiting {wait:F2}s before spawn");
                yield return new WaitForSeconds(wait);

                if (data.UnitBody == null)
                {
                    Debug.LogError($"SpawnLoop[{enemyID}]: UnitBody is null");
                    yield break;
                }

                var go = Instantiate(data.UnitBody, EnemySpawnPos.position, Quaternion.identity);
                if (go.TryGetComponent<Unit>(out var comp))
                {
                    comp.unitData = data;
                    Debug.Log($"SpawnLoop[{enemyID}]: spawned");
                }
                else
                {
                    Debug.LogWarning($"SpawnLoop[{enemyID}]: spawned object missing Unit component");
                }
            }
            else
            {
                yield return null;
            }
        }
    }
}
