using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ConnectionSheetData))]
public class ConnectionSheetData : MonoBehaviour
{
    [Header("Runtime Data Arrays")]
    public UnitData[] unitData;
    public UnitData[] enemyData;
    public UnitSkillData[] skillData;

    [Header("Sheet Databases")]
    public SheetEnemyDatabase SheetEnemyData;
    public SheetUnitDatabase SheetUnitData;
    public SheetSkillDatabase SheetSkillData;

    [Header("Skill Assets")]
    public UnitSkillData[] skillAssets;  // Inspector 에 UnitSkillData 에셋을 드래그

    void Awake()
    {
        LoadAllData();
    }

    public void LoadAllData()
    {
        LoadUnitData();
        LoadEnemyData();
        LoadSkillData();
    }

    void LoadUnitData()
    {
        var sheets = SheetUnitData.units;
        int len = Mathf.Min(unitData.Length, sheets.Count);

        for (int i = 0; i < len; i++)
        {
            var sheet = sheets[i];
            var ud = unitData[i];

            ud.Faction = sheet.Faction;
            ud.name = sheet.Name;
            ud.OpenPrice = sheet.OpenPrice;
            ud.LevelupPrice = sheet.LevelupPrice;
            ud.MaxLevel = sheet.MaxLevel;
            ud.Speed = sheet.Speed;
            ud.Hp = sheet.Hp;
            ud.AttackSpeed = sheet.AttackSpeed;
            ud.AttackRange = sheet.AttackRange;
            ud.CoolDownTime = sheet.CooldownTime;

            // 이제 sheet.UnitDefaultSkill 은 string ID
            //ud.UnitDefaultSkill = FindSkillAsset(sheet.UnitDefaultSkill);
            //ud.UnitEvolvedSkill = FindSkillAsset(sheet.UnitEvolvedSkill);
        }
    }

    void LoadEnemyData()
    {
        var sheets = SheetEnemyData.enemies;
        int len = Mathf.Min(enemyData.Length, sheets.Count);

        for (int i = 0; i < len; i++)
        {
            var sheet = sheets[i];
            var ud = enemyData[i];

            ud.Faction = sheet.Faction;
            ud.name = sheet.Name;
            ud.Hp = sheet.Hp;
            ud.AttackSpeed = sheet.AttackSpeed;
            ud.AttackRange = sheet.AttackRange;
            ud.Speed = sheet.Speed;

            //ud.UnitDefaultSkill = FindSkillAsset(sheet.EnemyDefaultSkill);
        }
    }

    /// <summary>
    /// skillAssets 배열에서 ID(또는 이름)으로 UnitSkillData를 찾아 반환
    /// </summary>
    UnitSkillData FindSkillAsset(string skillID)
    {
        if (string.IsNullOrEmpty(skillID))
            return null;

        var skill = skillAssets.FirstOrDefault(s => s != null && s.name == skillID);
        if (skill == null)
            Debug.LogWarning($"UnitSkillData '{skillID}' not found in skillAssets array.");
        return skill;
    }


    void LoadSkillData()
    {
        var sheets = SheetSkillData.skills;
        int len = Mathf.Min(skillData.Length, sheets.Count);

        for (int i = 0; i < len; i++)
        {
            var sheet = sheets[i];
            var sd = skillData[i];

            // -- 숫자/불리언 필드만 채워넣기 (참조형은 건너뜁니다) --

            //sd.RangeType = sheet.RangeType;
            sd.Damage = sheet.Damage;
            sd.Critical_probability = sheet.Critical_probability;
            //sd.Critical_damage = sheet.Critical_damage;

            sd.RangeAttackCheck = sheet.RangeAttackCheck;
            sd.RangeDiameter = sheet.RangeDiameter;
            sd.AttackCount = sheet.AttackCount;

            sd.UseSkillEffect = sheet.UseSkillEffect;
            sd.ProcChance = sheet.ProcChance;

            // ProjectilePrefab, ProjectileSpeed는 건너뜀

            sd.Slow = sheet.Slow;
            sd.SlowRatio = sheet.SlowRatio;
            sd.SlowDuration = sheet.SlowDuration;

            sd.Stun = sheet.Stun;
            sd.StunDuration = sheet.StunDuration;

            sd.Nockback = sheet.Nockback;
            sd.NockbackStrength = sheet.NockbackStrength;

            sd.AtkSpeedDown = sheet.AtkSpeedDown;
            sd.AtkSpeedDownRatio = sheet.AtkSpeedDownRatio;
            sd.AtkSpeedDownDuration = sheet.AtkSpeedDownDuration;

            sd.ClearEffects = sheet.ClearEffects;
            sd.EffectRemovalRange = sheet.EffectRemovalRange;
            sd.Unitcount = sheet.Unitcount;
        }
    }

}
