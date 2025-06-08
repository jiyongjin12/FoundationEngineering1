using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SheetEnemyDatabase", menuName = "Game/SheetEnemyDatabase")]
public class SheetEnemyDatabase : ScriptableObject
{
    public List<SheetEnemyData> enemies = new List<SheetEnemyData>();
}
