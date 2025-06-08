using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SheetSkillDatabase", menuName = "Game/SheetSkillDatabase")]
public class SheetSkillDatabase : ScriptableObject
{
    public List<SheetSkillData> skills = new List<SheetSkillData>();
}
