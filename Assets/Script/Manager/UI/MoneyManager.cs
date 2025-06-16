using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    public int Coins = 1000;    // 시작 코인

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool TrySpend(int amount)
    {
        if (Coins >= amount)
        {
            Coins -= amount;
            Debug.Log($"[MoneyManager] Spent {amount}. Remaining: {Coins}");
            return true;
        }
        Debug.LogWarning("돈부족");
        return false;
    }

    public void Earn(int amount)
    {
        Coins += amount;
        Debug.Log($"[MoneyManager] Earned {amount}. Total: {Coins}");
    }
}
