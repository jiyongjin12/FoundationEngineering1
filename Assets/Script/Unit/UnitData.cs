using UnityEngine;

[CreateAssetMenu(menuName = "Unit/UnitData")]
public class UnitData : ScriptableObject
{
    [Header("Unit 정보")]
    public int Faction; // 아군 = 0 적군 = 1

    public string name;

    //public float Damage;  스킬쪽으로 빠짐
    public float Hp;
    public float Speed;
    public float AttackSpeed;
    public float AttackRange;

    public float Critical_probability; // 치명타 확률
    public float Critical_damage; // 치명타 피해

    public float CoolDownTime; // 소환?

    [Header("버튼 UI")]
    public Sprite ButtonImage;

    [Header("유닛 스킬_나중에 리스트? 형식으로 바꿀거임")]
    public UnitSkillData UnitDefaultSkill; // 진화 전 스킬
    public UnitSkillData UnitEvolvedSkill; // 진화 후 스킬
}

