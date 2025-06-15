using UnityEngine;
using DamageNumbersPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Deck Button")]
    public DamageNumber damageNumberPrefab;
    public DamageNumber damageCriNumberPrefab;
    public DamageNumber healNumberPrefab;

    private void Start()
    {
        Instance = this;
    }
}
