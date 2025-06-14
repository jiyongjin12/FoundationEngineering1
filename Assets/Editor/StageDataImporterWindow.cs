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

    [MenuItem("Tools/Import Stage Group CSV")]
    public static void ShowWindow() => GetWindow<StageDataImporterWindow>("Stage Group Importer");

    private void OnGUI() {
        GUILayout.Label("CSV → StageGroupDatabase", EditorStyles.boldLabel);
        stageCsvUrl = EditorGUILayout.TextField("Stage CSV URL", stageCsvUrl);
        if (GUILayout.Button("Import and Group Data")) ImportAndGroup();
    }

    private void ImportAndGroup() {
        var db = LoadOrCreate<SheetStageDatabase>(STAGE_DB_PATH);
        db.stages.Clear();

        // download
        var text = DownloadCsv(stageCsvUrl);
        var lines = text.Split(new[]{"\r\n","\n"}, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 2) {
            Debug.LogWarning("No data in CSV");
            return;
        }
        // parse all waves
        var allWaves = new List<SheetStageData>();
        for (int i = 1; i < lines.Length; i++) {
            var cols = SplitCsvLine(lines[i]);
            var wave = new SheetStageData();
            wave.ID = cols[0];
            // parse "Stage_1_2"
            var parts = cols[0].Split('_');
            wave.StageSeries = int.Parse(parts[1]);
            wave.SubStageIndex = int.Parse(parts[2]);
            wave.StageHealth = float.Parse(cols[2]);
            // skip healthLevel if needed
            // parse types
            wave.EnemyType = cols[4].Trim('"','[',']')
                .Split(new[]{','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(s=>s.Trim()).ToList();
            wave.EnemyLevel = cols[5].Trim('"','[',']')
                .Split(new[]{','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(s=>int.Parse(s.Trim())).ToList();
            wave.MinSpawnTime = float.Parse(cols[6]);
            wave.MaxSpawnTime = float.Parse(cols[7]);

            allWaves.Add(wave);
        }
        // group by StageSeries
        var groups = allWaves.GroupBy(w => w.StageSeries)
            .OrderBy(g => g.Key);
        foreach (var g in groups) {
            var sg = new StageGroup();
            sg.StageSeries = g.Key;
            sg.Waves = g.OrderBy(w=>w.SubStageIndex).ToList();
            db.stages.Add(sg);
        }

        EditorUtility.SetDirty(db);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Imported and grouped {db.stages.Count} stage groups, total waves: {allWaves.Count}");
    }

    private string DownloadCsv(string url) {
        using var www = UnityWebRequest.Get(url);
        var op = www.SendWebRequest(); while(!op.isDone){};
        return www.downloadHandler.text;
    }

    private string[] SplitCsvLine(string line) {
        // same bracket-aware split as before
        var list = new List<string>(); var sb=new System.Text.StringBuilder(); int depth=0;
        foreach(char c in line){ if(c=='[') {depth++; sb.Append(c);} else if(c==']'){depth--; sb.Append(c);} else if(c==','&& depth==0){list.Add(sb.ToString()); sb.Clear();} else sb.Append(c);} list.Add(sb.ToString());
        return list.ToArray();
    }

    private T LoadOrCreate<T>(string path) where T:ScriptableObject{
        var asset = AssetDatabase.LoadAssetAtPath<T>(path);
        if (asset==null){
            asset = ScriptableObject.CreateInstance<T>();
            var folder = Path.GetDirectoryName(path);
            if(!AssetDatabase.IsValidFolder(folder)) { Directory.CreateDirectory(folder); }
            AssetDatabase.CreateAsset(asset,path);
        }
        return asset;
    }
}
#endif