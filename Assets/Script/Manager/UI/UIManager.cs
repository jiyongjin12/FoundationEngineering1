using UnityEngine;
using DamageNumbersPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Deck Button")]
    public DamageNumber damageNumberPrefab;

    private void Start()
    {
        Instance = this;
    }

    void Update()
    {
        //On leftclick.
        if (Input.GetMouseButtonDown(0))
        {
            //Spawn new popup at transform.position with a random number between 0 and 100.
            DamageNumber damageNumber = damageNumberPrefab.Spawn(transform.position, Random.Range(1, 100));
        }
    }
}
