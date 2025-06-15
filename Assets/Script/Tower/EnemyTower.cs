using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyTower : MonoBehaviour
{
    [Header("Enemy Units (UnitData.name must match Enemy_<name> IDs)")]
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
        // Build lookup: key = "Enemy_" + UnitData.name
        unitLookup = new Dictionary<string, UnitData>();
        foreach (var ud in allEnemyUnits)
        {
            if (ud == null || string.IsNullOrEmpty(ud.name)) continue;
            string key = "Enemy_" + ud.name;
            unitLookup[key] = ud;
        }

        // Load stage waves
        var group = StageLoadManager.Instance.SelectedGroup;
        if (group == null || group.Waves == null || group.Waves.Count == 0)
        {
            Debug.LogError("EnemyTower: No stage data loaded!");
            enabled = false;
            return;
        }
        waveDataList = group.Waves;

        // Initialize tower HP from first wave threshold
        HP_Tower.HP = waveDataList[0].StageHealth;
        HP_Tower.currentHP = HP_Tower.HP;
    }

    void Start()
    {
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

                // Trigger wave when HP is below or equal threshold
                if (currHP <= wave.StageHealth)
                {
                    triggeredWaves.Add(i);
                    // Start single spawn loop per wave
                    StartCoroutine(SpawnWaveLoop(i, wave));
                }
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator SpawnWaveLoop(int waveIndex, SheetStageData wave)
    {
        // Prepare spawn list
        List<string> spawnList = new List<string>(wave.EnemyType);
        float prevThreshold = waveIndex > 0 ? waveDataList[waveIndex - 1].StageHealth : float.MaxValue;
        float currThreshold = wave.StageHealth;

        while (HP_Tower.currentHP > 0f)
        {
            float currHP = HP_Tower.currentHP;
            bool inRange = currHP <= prevThreshold && currHP >= currThreshold;
            if (!inRange)
            {
                yield return null;
                continue;
            }

            // Refill spawn list if empty
            if (spawnList.Count == 0)
                spawnList = new List<string>(wave.EnemyType);

            // Pick random enemy ID
            int idx = Random.Range(0, spawnList.Count);
            string enemyID = spawnList[idx];
            spawnList.RemoveAt(idx);

            // Lookup UnitData
            if (!unitLookup.TryGetValue(enemyID, out var data))
            {
                Debug.LogWarning($"SpawnWaveLoop: No UnitData for '{enemyID}'");
                yield return null;
                continue;
            }

            // Wait random interval
            float wait = Random.Range(wave.MinSpawnTime, wave.MaxSpawnTime);
            yield return new WaitForSeconds(wait);

            // Instantiate
            if (data.UnitBody == null)
            {
                Debug.LogError($"SpawnWaveLoop[{enemyID}]: UnitBody is null");
                yield break;
            }
            var go = Instantiate(data.UnitBody, EnemySpawnPos.position, Quaternion.identity);
            if (go.TryGetComponent<Unit>(out var comp))
                comp.unitData = data;
        }
    }
}
