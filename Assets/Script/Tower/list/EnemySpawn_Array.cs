using UnityEngine;

[System.Serializable]
public class EnemySpawn_Array
{
    public float HP_Status;        // HP percent threshold
    public string[] E_name;        // UnitData.name to spawn
    public float MinSpawnTime;     // min spawn interval
    public float MaxSpawnTime;     // max spawn interval
}
