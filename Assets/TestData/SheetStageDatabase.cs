using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SheetStageDatabase", menuName = "Game/SheetStageDatabase")]
public class SheetStageDatabase : ScriptableObject
{
    public List<StageGroup> stages = new List<StageGroup>();
}
