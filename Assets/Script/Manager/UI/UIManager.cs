using UnityEngine;
using UnityEngine.UI;
using DamageNumbersPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("DamageNumber")]
    public DamageNumber damageNumberPrefab;
    public DamageNumber damageCriNumberPrefab;
    public DamageNumber healNumberPrefab;

    [Header("Win Lose UI")]
    public CanvasGroup Lose_UI;
    public CanvasGroup Win_UI;

    private void Start()
    {
        Instance = this;
    }
}
