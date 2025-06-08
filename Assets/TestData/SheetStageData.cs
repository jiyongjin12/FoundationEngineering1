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
    public List<string> EnemyType;    // ex. ["Enemy_HungryGhost", ...]
    public List<int> EnemyLevel;   // ex. [1,1,1]
    public float SummonTime;
}
