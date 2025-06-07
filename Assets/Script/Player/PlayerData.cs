using UnityEngine;

[CreateAssetMenu(menuName = "Player/PlayerData")]
public class PlayerData : ScriptableObject
{
    public int MaxLevel;
    public float MoveSpeed;
    public float MaxMana;
    public float MaxHealth;
    public float NeedGold;

    public float ManaRegenerationTime;

    [Header("스킬")]
    public float MaxChargingTime;
    public float SpandManaCount;
    public float SkillDamage;
    public float SkillSpeed;
    
}
