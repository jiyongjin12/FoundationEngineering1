using UnityEngine;

[CreateAssetMenu(menuName = "Unit/UnitData")]
public class UnitData : ScriptableObject
{
    [Header("Unit 정보")]
    public int Faction; // 아군 = 0 적군 = 1

    public string name;

    public float Hp;
    public float Speed;
    public float AttackSpeed;
    public float AttackRange;

    [Header("레벨")]
    public float MaxLevel;

    public GameObject UnitBody;

    [Header("버튼 UI")]
    public Sprite ButtonImage;

    [Header("유닛 스킬_나중에 리스트? 형식으로 바꿀거임")]
    public UnitSkillData UnitDefaultSkill; // 진화 전 스킬
    public UnitSkillData UnitEvolvedSkill; // 진화 후 스킬

    public Animation UnitAnimation;

    [Header("아군 스폰 쿨타임")]
    public float CoolDownTime;

    [Header("적구 스폰 Min/Max타임")]
    public float MinSpawnTime; // 소환?
    public float MaxSpawnTime; // 소환?
}

