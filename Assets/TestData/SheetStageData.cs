using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SheetStageData
{
    public string ID;
    public int StageID;
    public int StageHealth;
    public int HealthLevel;
    public List<string> EnemyType;  
    public List<int> EnemyLevel;   
    //public float SummonTime;
    public float MinSpawnTime;
    public float MaxSpawnTime;
}
