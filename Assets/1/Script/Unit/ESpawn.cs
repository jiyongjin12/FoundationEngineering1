using System.Collections.Generic;
using UnityEngine;

public class ESpawn : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _unitList;

    [SerializeField]
    private Transform SpawnPos;

    //private int currentCustomerCount;
    //public int MaxCustomerCount = 3;

    [SerializeField]
    private float spawnInterval = 3f;
    private float spawnTimer = 0f;

    private void FixedUpdate()
    {
        spawnTimer += Time.fixedDeltaTime;

        if (spawnTimer >= spawnInterval)
        {
            Spawn();
            spawnTimer = 0f;
        }
    }


    private void Spawn()
    {
        if (_unitList.Count == 0) return;

        GameObject customerPrefab = _unitList[Random.Range(0, _unitList.Count)];

        GameObject newCustomer = Instantiate(customerPrefab, SpawnPos.position, Quaternion.identity);
    }
}
