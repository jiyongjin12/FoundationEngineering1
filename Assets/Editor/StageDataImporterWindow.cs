#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

public class StageDataImporterWindow : EditorWindow
{
    private const string STAGE_DB_PATH = "Assets/TestData/Data/Units/StageDatabase.asset";
    private string stageCsvUrl = "https://docs.google.com/spreadsheets/d/e/2PACX-1vSjt3Sg-0kCponFFLmQl6CkcSPQCq0G-h2znJC2kPsF4No4ju5eSaCzDIT8Bn8B-jK3M1CEN-rm7fV1/pub?gid=95588700&single=true&output=csv";

    [MenuItem("Tools/Import Stage CSV")]
    public static void ShowStageWindow() => GetWindow<StageDataImporterWindow>("Stage Data Importer");

    private void OnGUI()
    {
        GUILayout.Label("CSV → StageDatabase", EditorStyles.boldLabel);
        stageCsvUrl = EditorGUILayout.TextField("Stage CSV URL", stageCsvUrl);
        if (GUILayout.Button("Import Stage Data")) ImportStageCsv();
    }

    private void ImportStageCsv()
    {
        var db = LoadOrCreate<SheetStageDatabase>(STAGE_DB_PATH);
        db.stages.Clear();

        var lines = DownloadCsv(stageCsvUrl);
        if (lines == null || lines.Count <= 1)
        {
            Debug.LogWarning("CSV 데이터가 없거나 헤더만 존재합니다.");
        }
        else
        {
            for (int i = 1; i < lines.Count; i++)
            {
                string rawLine = lines[i];
                string[] tokens = SplitCsvLine(rawLine);
                Debug.Log($"[StageImporter] Line {i + 1} token count={tokens.Length}: {string.Join(" | ", tokens)}");

                try
                {
                    var data = ParseStageLine(tokens, i + 1);
                    db.stages.Add(data);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[StageImporter] Line {i + 1} 파싱 실패: {e.Message}");
                }
            }
        }

        EditorUtility.SetDirty(db);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Stage Import 완료: {db.stages.Count}개 스테이지 데이터 저장됨.");
    }

    private List<string> DownloadCsv(string url)
    {
        using var www = UnityWebRequest.Get(url);
        var op = www.SendWebRequest(); while (!op.isDone) { }
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"CSV 다운로드 실패 [{url}]: {www.error}");
            return null;
        }
        return www.downloadHandler.text
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .ToList();
    }

    // 브라켓 내 콤마를 무시하는 커스텀 분리 함수
    private string[] SplitCsvLine(string line)
    {
        var tokens = new List<string>();
        var sb = new StringBuilder();
        int bracketDepth = 0;

        foreach (char c in line)
        {
            if (c == '[') { bracketDepth++; sb.Append(c); }
            else if (c == ']') { bracketDepth--; sb.Append(c); }
            else if (c == ',' && bracketDepth == 0)
            {
                tokens.Add(sb.ToString()); sb.Clear();
            }
            else sb.Append(c);
        }
        tokens.Add(sb.ToString());
        return tokens.Select(s => s.Trim()).ToArray();
    }

    private SheetStageData ParseStageLine(string[] tokens, int lineNumber)
    {
        if (tokens.Length < 8)
            throw new FormatException($"필드 개수 부족: 예상 8개, 실제 {tokens.Length}개");

        int idx = 0;
        var id = tokens[idx++];
        if (!int.TryParse(tokens[idx++], out var stageId))
            throw new FormatException($"StageID 파싱 실패: '{tokens[idx - 1]}'");
        if (!int.TryParse(tokens[idx++], out var stageHealth))
            throw new FormatException($"StageHealth 파싱 실패: '{tokens[idx - 1]}'");
        if (!int.TryParse(tokens[idx++], out var healthLevel))
            throw new FormatException($"HealthLevel 파싱 실패: '{tokens[idx - 1]}'");

        // 문자열 & 대괄호, 쌍따옴표 제거
        var rawType = tokens[idx++].Trim().Trim('"').Trim('[', ']');
        var typeList = rawType.Length == 0
            ? new List<string>()
            : rawType.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                     .Select(s => s.Trim()).ToList();

        var rawLevel = tokens[idx++].Trim().Trim('"').Trim('[', ']');
        var levelList = rawLevel.Length == 0
            ? new List<int>()
            : rawLevel.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                      .Select(s =>
                      {
                          if (int.TryParse(s.Trim(), out var lv)) return lv;
                          throw new FormatException($"EnemyLevel 파싱 실패: '{s}' at line {lineNumber}");
                      })
                      .ToList();

        if (!float.TryParse(tokens[idx++], out var minSpawn))
            throw new FormatException($"MinSpawnTime 파싱 실패: '{tokens[idx - 1]}'");
        if (!float.TryParse(tokens[idx++], out var maxSpawn))
            throw new FormatException($"MaxSpawnTime 파싱 실패: '{tokens[idx - 1]}'");

        return new SheetStageData
        {
            ID = id,
            StageID = stageId,
            StageHealth = stageHealth,
            HealthLevel = healthLevel,
            EnemyType = typeList,
            EnemyLevel = levelList,
            MinSpawnTime = minSpawn,
            MaxSpawnTime = maxSpawn
        };
    }

    private T LoadOrCreate<T>(string path) where T : ScriptableObject
    {
        var db = AssetDatabase.LoadAssetAtPath<T>(path);
        if (db == null)
        {
            var folder = Path.GetDirectoryName(path);
            if (!AssetDatabase.IsValidFolder(folder))
                Directory.CreateDirectory(folder);

            db = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(db, path);
            AssetDatabase.Refresh();
        }
        return db;
    }
}
#endif