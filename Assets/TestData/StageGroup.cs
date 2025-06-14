using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StageGroup
{
    public int StageSeries;             // main stage id: 1,2,3...
    public List<SheetStageData> Waves;   // waves: 1_1,1_2,1_3
}
