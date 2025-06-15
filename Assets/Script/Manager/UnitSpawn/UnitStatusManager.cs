using System.Collections.Generic;
using UnityEngine;

public class UnitStatusManager : MonoBehaviour
{
    public static UnitStatusManager Instance { get; private set; }

    [Header("초기 사용 가능 유닛 (ScriptableObject)")]
    public List<UnitData> initialUnits;

    private Dictionary<string, UnitStatus> unitStatuses = new Dictionary<string, UnitStatus>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 초기 유닛들에 대한 상태를 name 기준으로 생성
        foreach (var ud in initialUnits)
        {
            if (ud == null || string.IsNullOrEmpty(ud.name)) continue;
            if (!unitStatuses.ContainsKey(ud.name))
                unitStatuses.Add(ud.name, new UnitStatus(ud));
        }
    }

    public UnitStatus GetStatus(string unitName)
    {
        unitStatuses.TryGetValue(unitName, out var status);
        return status;
    }

    public void AddUnit(UnitData ud)
    {
        if (ud == null || string.IsNullOrEmpty(ud.name)) return;
        if (!unitStatuses.ContainsKey(ud.name))
            unitStatuses[ud.name] = new UnitStatus(ud);
    }

    public bool TryLevelUp(string unitName)
    {
        if (unitStatuses.TryGetValue(unitName, out var status))
        {
            if (status.CurrentLevel < status.Data.MaxLevel)
            {
                status.CurrentLevel++;
                Debug.Log($"Unit '{unitName}' leveled up to {status.CurrentLevel}");
                return true;
            }
            else
            {
                Debug.LogWarning($"Unit '{unitName}' is already at max level ({status.CurrentLevel})");
            }
        }
        return false;
    }

    public class UnitStatus
    {
        public UnitData Data { get; private set; }
        public int CurrentLevel { get; set; }

        public UnitStatus(UnitData data)
        {
            Data = data;
            CurrentLevel = 1;
        }
    }
}
