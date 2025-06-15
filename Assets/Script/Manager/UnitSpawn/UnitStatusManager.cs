using System.Collections.Generic;
using UnityEngine;

public class UnitStatusManager : MonoBehaviour
{
    public static UnitStatusManager Instance { get; private set; }

    [Header("��ü ���� ��� (Inspector���� ����)")]
    public List<UnitData> allUnits;

    [Header("�رݵ� ���� ��� (CurrentLevel �� 1)")]
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

        // unlockedUnits ������Ʈ
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
