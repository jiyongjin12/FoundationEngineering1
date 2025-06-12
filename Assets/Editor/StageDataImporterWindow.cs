#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class StageDataImporterWindow : EditorWindow
{
    private const string STAGE_DB_PATH = "Assets/TestData/Data/Units/StageDatabase.asset";
    private string stageCsvUrl = "https://docs.google.com/spreadsheets/d/e/.../pub?output=csv&gid=STAGE_GID";

    [MenuItem("Tools/Import Stage CSV")]
    public static void ShowStageWindow()
    {
        GetWindow<StageDataImporterWindow>("Stage Data Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("CSV → StageDatabase", EditorStyles.boldLabel);
        stageCsvUrl = EditorGUILayout.TextField("Stage CSV URL", stageCsvUrl);

        if (GUILayout.Button("Import Stage Data"))
            ImportStageCsv();
    }

    private void ImportStageCsv()
    {
        var db = LoadOrCreate<SheetStageDatabase>(STAGE_DB_PATH);
        db.stages.Clear();
        ParseCsvToList(stageCsvUrl, ParseStageLine, db.stages);

        EditorUtility.SetDirty(db);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Stage Import 완료: {db.stages.Count}개 스테이지 데이터 저장됨.");
    }

    // 범용 CSV 파서 유틸
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

    private SheetStageData ParseStageLine(string[] c)
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