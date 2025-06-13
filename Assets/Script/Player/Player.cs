using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerData playerData;
    public Health PlayerHp;

    Rigidbody rb;

    public float currentMana;

    [Header("Skill")]
    public GameObject skillProjectilePrefab; // 던질 투사체 프리팹
    public Transform skillSpawnPoint;        // 투사체 생성 위치
    private bool isCharging = false;
    private float chargeTime = 0f;
    public bool tier30, tier70, tier100;

    public Animator PlayerMove;
    public SpriteRenderer PlayerBody;

    void Awake()
    {
        PlayerHp.HP = playerData.MaxHealth;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentMana = playerData.MaxMana;
        StartCoroutine(ManaRegeneration(playerData.ManaRegenerationTime));
    }

    void Update()
    {
        Movement();
        if (currentMana > playerData.SpandManaCount)
        {
            HandleSkillCharge();
        }
    }

    private void Movement()
    {
        float x = Input.GetAxis("Horizontal");
        Vector3 inputDir = new Vector3(x, 0, 0);

        rb.linearVelocity = inputDir * playerData.MoveSpeed;

        bool isMoving = !Mathf.Approximately(x, 0f);
        PlayerMove.SetBool("IsWork", isMoving);

        Vector3 scale = PlayerBody.transform.localScale;
        if (!isMoving || x >= 0f)
        {
            scale.x = 1f;
        }
        else if (x < 0f)
        {
            scale.x = -1f;
        }
        PlayerBody.transform.localScale = scale;
    }

    private void HandleSkillCharge()
    {
        // 스킬 차징 시작
        if (Input.GetKeyDown(KeyCode.J) && PlayerHp.currentHP > 0)
        {
            isCharging = true;
            chargeTime = 0f;
            tier30 = tier70 = tier100 = false;
        }

        // 차징 중
        if (isCharging)
        {
            chargeTime += Time.deltaTime;
            float maxT = playerData.MaxChargingTime;

            // 티어 플래그 세팅
            if (!tier30 && chargeTime >= maxT * 0.3f)
                tier30 = true;
            if (!tier70 && chargeTime >= maxT * 0.7f)
                tier70 = true;
            if (!tier100 && chargeTime >= maxT)
            {
                tier100 = true;
                chargeTime = maxT; // 최대 클램프
            }
        }

        // 스킬 발사 (키 떼면)
        if (isCharging && Input.GetKeyUp(KeyCode.J))
        {
            FireChargedSkill();
            currentMana -= playerData.SpandManaCount;
            isCharging = false;
        }
    }

    private void FireChargedSkill()
    {
        // 1) 마나 소모 (있다면)
        // TODO: playerData.MaxMana, SpandManaCount 사용

        // 2) 데미지 계산
        float baseDmg = playerData.SkillDamage;
        float finalDmg = baseDmg;
        if (tier100) finalDmg = baseDmg * 2f;
        else if (tier70) finalDmg = baseDmg * 1.5f;

        // 3) 투사체 생성
        if (skillProjectilePrefab != null && skillSpawnPoint != null)
        {
            var proj = Instantiate(
                skillProjectilePrefab,
                skillSpawnPoint.position,
                Quaternion.identity
            );
            var sp = proj.GetComponent<PlayerArrow>();
            if (sp != null)
            {
                // 바라보는 방향 X축 기준으로 발사
                Vector3 fireDir = Vector3.right;
                sp.Initialize(finalDmg, fireDir, playerData.SkillSpeed);
            }
        }

        // 디버그
        Debug.Log(finalDmg);
    }

    public IEnumerator ManaRegeneration(float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);
            currentMana = Mathf.Min(currentMana + 1f, playerData.MaxMana);
            Debug.Log($"Mana regenerated: {currentMana}/{playerData.MaxMana}");
        }
    }
}
