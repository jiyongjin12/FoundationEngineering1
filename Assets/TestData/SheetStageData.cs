using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SheetStageData
{
    public string ID;
    public int StageSeries;
    public int SubStageIndex; 
    public float StageHealth;
    public List<string> EnemyType;
    public List<int> EnemyLevel;   
    public float MinSpawnTime;
    public float MaxSpawnTime;
}
