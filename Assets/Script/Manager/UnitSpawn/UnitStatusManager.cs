using System.Collections.Generic;
using UnityEngine;

public class UnitStatusManager : MonoBehaviour
{
    public static UnitStatusManager Instance { get; private set; }

    [Header("전체 유닛 목록 (Inspector에서 설정)")]
    public List<UnitData> allUnits;

    [Header("해금된 유닛 목록 (CurrentLevel ≥ 1)")]
    public List<UnitData> unlockedUnits = new List<UnitData>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (allUnits == null) return;

        // unlockedUnits 업데이트
        unlockedUnits.Clear();
        foreach (var ud in allUnits)
        {
            if (ud != null && ud.CurrentLevel >= 1)
            {
                unlockedUnits.Add(ud);
            }
        }
    }
}
