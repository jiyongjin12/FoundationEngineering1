using System.Collections.Generic;
using UnityEngine;

public class UnitSpawn : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _unitList;

    [SerializeField]
    private Transform SpawnPos;

    public void Spawn()
    {
        if (_unitList.Count == 0) return;

        GameObject customerPrefab = _unitList[Random.Range(0, _unitList.Count)];

        GameObject newCustomer = Instantiate(customerPrefab, SpawnPos.position, Quaternion.identity);
    }
}
