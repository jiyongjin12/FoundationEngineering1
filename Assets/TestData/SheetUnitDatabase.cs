using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SheetUnitDatas", menuName = "Game/SheetUnitDatas")]
public class SheetUnitDatabase : ScriptableObject
{
    public List<SheetUnitDatas> units = new List<SheetUnitDatas>();
}
