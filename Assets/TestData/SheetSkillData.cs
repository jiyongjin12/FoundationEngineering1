using System;
using UnityEngine;

[Serializable]
public class SheetSkillData
{
    public string ID;
    public string RangeType;
    public int Damage;
    public bool RangeAttackCheck;
    public int RangeDiameter;
    public bool UseSkillEffect;
    public float ProcChance;
    public float Critical_probability;
    public float Critical_Damage;
    public float ProjectilesSpeed;
    public int AttackCount;
    public bool Slow;
    public bool Stun;
    public bool Nockback;
    public bool AtkSpeedDown;
    public bool ClearEffects;
    public float SlowRatio;
    public float SlowDuration;
    public float StunDuration;
    public float NockbackStrength;
    public float AtkSpeedDownRatio;
    public float AtkSpeedDownDuration;
    public float EffectRemovalRange;
    public int Unitcount;
}
