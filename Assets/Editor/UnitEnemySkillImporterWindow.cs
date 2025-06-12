#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Codice.CM.Client.Differences.Graphic;

public class UnitEnemySkillImporterWindow : EditorWindow
{
    private const string UNIT_DB_PATH = "Assets/TestData/Data/Units/UnitDatabase.asset";
    private const string ENEMY_DB_PATH = "Assets/TestData/Data/Units/EnemyDatabase.asset";
    private const string SKILL_DB_PATH = "Assets/TestData/Data/Units/SkillDatabase.asset";

    private string unitCsvUrl = "https://docs.google.com/spreadsheets/d/e/.../pub?output=csv&gid=UNIT_GID";
    private string enemyCsvUrl = "https://docs.google.com/spreadsheets/d/e/.../pub?output=csv&gid=ENEMY_GID";
    private string skillCsvUrl = "https://docs.google.com/spreadsheets/d/e/.../pub?output=csv&gid=SKILL_GID";

    [MenuItem("Tools/Import Unit/Enemy/Skill CSV")]
    public static void ShowUnitEnemySkillWindow()
    {
        GetWindow<UnitEnemySkillImporterWindow>("Unit/Enemy/Skill Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("CSV → Databases", EditorStyles.boldLabel);
        unitCsvUrl = EditorGUILayout.TextField("Unit CSV URL", unitCsvUrl);
        enemyCsvUrl = EditorGUILayout.TextField("Enemy CSV URL", enemyCsvUrl);
        skillCsvUrl = EditorGUILayout.TextField("Skill CSV URL", skillCsvUrl);

        if (GUILayout.Button("Import All"))
            ImportAll();
    }

    private void ImportAll()
    {
        var unitDb = LoadOrCreate<SheetUnitDatabase>(UNIT_DB_PATH);
        var enemyDb = LoadOrCreate<SheetEnemyDatabase>(ENEMY_DB_PATH);
        var skillDb = LoadOrCreate<SheetSkillDatabase>(SKILL_DB_PATH);

        unitDb.units.Clear();
        enemyDb.enemies.Clear();
        skillDb.skills.Clear();

        ParseCsvToList(unitCsvUrl, ParseUnitLine, unitDb.units);
        ParseCsvToList(enemyCsvUrl, ParseEnemyLine, enemyDb.enemies);
        ParseCsvToList(skillCsvUrl, ParseSkillLine, skillDb.skills);

        EditorUtility.SetDirty(unitDb);
        EditorUtility.SetDirty(enemyDb);
        EditorUtility.SetDirty(skillDb);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Import 완료: Units={unitDb.units.Count}, Enemies={enemyDb.enemies.Count}, Skills={skillDb.skills.Count}");
    }

    private void ParseCsvToList<T>(string url, Func<string[], T> ctor, List<T> list)
    {
        using var www = UnityWebRequest.Get(url);
        var op = www.SendWebRequest(); while (!op.isDone) { }
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"CSV 다운로드 실패 [{url}]: {www.error}");
            return;
        }

        var lines = www.downloadHandler.text
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < lines.Length; i++)
        {
            var cols = lines[i].Split(',');
            try { list.Add(ctor(cols)); }
            catch (Exception e)
            { Debug.LogWarning($"Line {i + 1} 파싱 실패: {e.Message}"); }
        }
    }

    private SheetUnitDatas ParseUnitLine(string[] c)
    {
        return new SheetUnitDatas
        {
            ID = c[0],
            Name = c[1],
            Faction = int.Parse(c[2]),
            OpenPrice = int.Parse(c[3]),
            LevelupPrice = int.Parse(c[4]),
            MaxLevel = int.Parse(c[5]),
            NeedMana = int.Parse(c[6]),
            Hp = int.Parse(c[7]),
            AttackSpeed = float.Parse(c[8]),
            AttackRange = float.Parse(c[9]),
            Speed = float.Parse(c[10]),
            UnitDefaultSkill = c[11],
            UnitEvolvedSkill = c[12],
            CooldownTime = float.Parse(c[13])
        };
    }

    private SheetEnemyData ParseEnemyLine(string[] c)
    {
        return new SheetEnemyData
        {
            ID = c[0],
            Name = c[1],
            Faction = int.Parse(c[2]),
            StartLevel = int.Parse(c[3]),
            MaxLevel = int.Parse(c[4]),
            Hp = int.Parse(c[5]),
            AttackSpeed = float.Parse(c[6]),
            AttackRange = float.Parse(c[7]),
            Speed = float.Parse(c[8]),
        };
    }

    private SheetSkillData ParseSkillLine(string[] c)
    {
        bool B(int i) => c[i] == "1";
        int I(int i) => int.Parse(c[i]);
        float F(int i) => float.Parse(c[i]);
        return new SheetSkillData
        {
            ID = c[0],
            RangeType = c[1],
            Damage = I(2),
            RangeAttackCheck = B(3),
            RangeDiameter = I(4),
            UseSkillEffect = B(5),
            ProcChance = F(6),
            Critical_probability = F(7),
            Critical_Damage = F(8),
            ProjectilesSpeed = F(9),
            AttackCount = I(10),
            Slow = B(11),
            Stun = B(12),
            Nockback = B(13),
            AtkSpeedDown = B(14),
            ClearEffects = B(15),
            SlowRatio = F(16),
            SlowDuration = F(17),
            StunDuration = F(18),
            NockbackStrength = F(19),
            AtkSpeedDownRatio = F(20),
            AtkSpeedDownDuration = F(21),
            EffectRemovalRange = F(22),
            Unitcount = I(23)
        };
    }

    private T LoadOrCreate<T>(string path) where T : ScriptableObject
    {
        var db = AssetDatabase.LoadAssetAtPath<T>(path);
        if (db == null)
        {
            var folder = Path.GetDirectoryName(path);
            if (!AssetDatabase.IsValidFolder(folder)) Directory.CreateDirectory(folder);
            db = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(db, path);
            AssetDatabase.Refresh();
        }
        return db;
    }
}
#endif