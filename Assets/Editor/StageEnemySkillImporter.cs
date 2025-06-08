#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Codice.CM.Client.Differences.Graphic;

public class StageEnemySkillImporter : EditorWindow
{
    private const string ENEMY_DB_PATH = "Assets/TestData/Data/Units/EnemyDatabase.asset";
    private const string STAGE_DB_PATH = "Assets/TestData/Data/Units/StageDatabase.asset";
    private const string SKILL_DB_PATH = "Assets/TestData/Data/Units/SkillDatabase.asset";

    string enemyCsvUrl = "https://...&gid=ENEMY_GID&output=csv";
    string stageCsvUrl = "https://...&gid=STAGE_GID&output=csv";
    string skillCsvUrl = "https://...&gid=SKILL_GID&output=csv";

    [MenuItem("Tools/Import Enemy/Stage/Skill CSV")]
    static void OpenWindow()
    {
        GetWindow<StageEnemySkillImporter>("Data CSV Importer");
    }

    void OnGUI()
    {
        GUILayout.Label("CSV URLs (publish each tab as CSV)", EditorStyles.boldLabel);
        enemyCsvUrl = EditorGUILayout.TextField("Enemy CSV URL", enemyCsvUrl);
        stageCsvUrl = EditorGUILayout.TextField("Stage CSV URL", stageCsvUrl);
        skillCsvUrl = EditorGUILayout.TextField("Skill  CSV URL", skillCsvUrl);

        if (GUILayout.Button("Import All"))
            ImportAll();
    }

    void ImportAll()
    {
        var enemyDb = LoadOrCreate<SheetEnemyDatabase>(ENEMY_DB_PATH);
        var stageDb = LoadOrCreate<SheetStageDatabase>(STAGE_DB_PATH);
        var skillDb = LoadOrCreate<SheetSkillDatabase>(SKILL_DB_PATH);

        enemyDb.enemies.Clear();
        stageDb.stages.Clear();
        skillDb.skills.Clear();

        // 1) Enemy
        ParseCsvToList(enemyCsvUrl, ParseEnemyLine, enemyDb.enemies);

        // 2) Stage
        ParseCsvToList(stageCsvUrl, ParseStageLine, stageDb.stages);

        // 3) Skill
        ParseCsvToList(skillCsvUrl, ParseSkillLine, skillDb.skills);

        EditorUtility.SetDirty(enemyDb);
        EditorUtility.SetDirty(stageDb);
        EditorUtility.SetDirty(skillDb);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Import 완료: Enemies={enemyDb.enemies.Count}, Stages={stageDb.stages.Count}, Skills={skillDb.skills.Count}");
    }

    T LoadOrCreate<T>(string path) where T : ScriptableObject
    {
        var db = AssetDatabase.LoadAssetAtPath<T>(path);
        if (db == null)
        {
            var folder = Path.GetDirectoryName(path);
            if (!AssetDatabase.IsValidFolder(folder))
            {
                Directory.CreateDirectory(folder);
                AssetDatabase.Refresh();
            }
            db = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(db, path);
        }
        return db;
    }

    void ParseCsvToList<T>(string url, Func<string[], T> ctor, List<T> list)
    {
        using var www = UnityWebRequest.Get(url);
        var op = www.SendWebRequest();
        while (!op.isDone) { }
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
            {
                Debug.LogWarning($"Line {i + 1} 파싱 실패: {e.Message}");
            }
        }
    }

    // --- CSV → 객체 파서들 ---
    SheetEnemyData ParseEnemyLine(string[] c)
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
            MinSpawnTime = float.Parse(c[9]),
            MaxSpawnTime = float.Parse(c[10])
        };
    }

    SheetStageData ParseStageLine(string[] c)
    {
        return new SheetStageData
        {
            ID = c[0],
            StageID = int.Parse(c[1]),
            StageHealth = int.Parse(c[2]),
            HealthLevel = int.Parse(c[3]),
            EnemyType = c[4].Trim('[', ']').Split(',').ToList(),
            EnemyLevel = c[5].Trim('[', ']').Split(',').Select(int.Parse).ToList(),
            SummonTime = float.Parse(c[6])
        };
    }

    SheetSkillData ParseSkillLine(string[] c)
    {
        bool B(int i) => c[i] == "1";
        int I(int i) => int.Parse(c[i]);
        float F(int i) => float.Parse(c[i]);

        return new SheetSkillData
        {
            ID = c[0],
            RangeType = c[1],
            Damage = I(2),
            RangeAttackCheck = I(3),
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
            AtkSpeedDown = F(14),
            ClearEffects = F(15),
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
}
#endif