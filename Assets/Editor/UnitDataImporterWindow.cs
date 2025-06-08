#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System;
using System.IO;

public class UnitDataImporterWindow : EditorWindow
{
    private const string assetPath = "Assets/TestData/Data/Units/UnitDatabase.asset";
    private string csvUrl = "https://docs.google.com/spreadsheets/d/e/2PACX-.../pub?output=csv";

    [MenuItem("Tools/Import UnitDatabase From CSV")]
    public static void ShowWindow()
    {
        GetWindow<UnitDataImporterWindow>("UnitDatabase Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("CSV → UnitDatabase", EditorStyles.boldLabel);
        csvUrl = EditorGUILayout.TextField("CSV URL", csvUrl);

        if (GUILayout.Button("Import Now"))
            ImportCsvToDatabase();
    }

    private void ImportCsvToDatabase()
    {
        // 1) Database 에셋 불러오기 or 새로 생성
        var db = AssetDatabase.LoadAssetAtPath<SheetUnitDatabase>(assetPath);
        if (db == null)
        {
            // 폴더 생성
            var folder = Path.GetDirectoryName(assetPath);
            if (!AssetDatabase.IsValidFolder(folder))
            {
                Directory.CreateDirectory(folder);
                AssetDatabase.Refresh();
            }

            db = ScriptableObject.CreateInstance<SheetUnitDatabase>();
            AssetDatabase.CreateAsset(db, assetPath);
        }

        // 2) CSV 다운로드 (동기)
        var www = UnityWebRequest.Get(csvUrl);
        var op = www.SendWebRequest();
        while (!op.isDone) { }

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"CSV 다운로드 실패: {www.error}");
            return;
        }

        // 3) 파싱
        var text = www.downloadHandler.text;
        var lines = text.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 2)
        {
            Debug.LogWarning("CSV에 데이터가 없습니다.");
            return;
        }

        db.units.Clear();
        for (int i = 1; i < lines.Length; i++)
        {
            var cols = lines[i].Split(',');
            if (cols.Length < 14) continue;

            var data = new SheetUnitDatas
            {
                ID = cols[0],
                Name = cols[1],
                Faction = int.Parse(cols[2]),
                OpenPrice = int.Parse(cols[3]),
                LevelupPrice = int.Parse(cols[4]),
                MaxLevel = int.Parse(cols[5]),
                NeedMana = int.Parse(cols[6]),
                Hp = int.Parse(cols[7]),
                AttackSpeed = float.Parse(cols[8]),
                AttackRange = float.Parse(cols[9]),
                Speed = float.Parse(cols[10]),
                UnitDefaultSkill = cols[11],
                UnitEvolvedSkill = cols[12],
                CooldownTime = float.Parse(cols[13])
            };
            db.units.Add(data);
        }

        // 4) 저장 & 리프레시
        EditorUtility.SetDirty(db);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Import 완료: {db.units.Count}개 유닛이 UnitDatabase에 저장되었습니다.");
    }
}
#endif