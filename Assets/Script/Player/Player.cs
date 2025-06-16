using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [Header("Skill_Cooldown")]
    public float skillCooldown = 2f;           // 스킬 쿨타임
    private float currentCooldown = 0f;        // 남은 쿨타임
    public Image CooldownSlider;               // 쿨타임 표시 슬라이더

    public Animator PlayerMove;
    public SpriteRenderer PlayerBody;

    public Image HpSlider;
    public Image ManaSlider;
    public TMP_Text HpText;
    public TMP_Text ManaText;

    void Awake()
    {
        PlayerHp.HP = playerData.MaxHealth;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentMana = playerData.MaxMana;
        StartCoroutine(ManaRegeneration(playerData.ManaRegenerationTime));
        UpdateUI();
    }

    void Update()
    {
        Movement();
        CooldownTick();
        if (currentCooldown <= 0f && currentMana > playerData.SpandManaCount)
        {
            HandleSkillCharge();
        }
        UpdateUI();
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
            currentCooldown = skillCooldown;
            isCharging = false;
        }
    }

    private void FireChargedSkill()
    {
        float baseDmg = playerData.SkillDamage;
        float finalDmg = baseDmg;
        if (tier100) finalDmg = baseDmg * 2f;
        else if (tier70) finalDmg = baseDmg * 1.5f;

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

        Debug.Log(finalDmg);
    }

    private void CooldownTick()
    {
        if (currentCooldown > 0f)
        {
            currentCooldown -= Time.deltaTime;
            if (currentCooldown < 0f) currentCooldown = 0f;
        }
    }

    public IEnumerator ManaRegeneration(float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);
            currentMana = Mathf.Min(currentMana + 1f, playerData.MaxMana);
            UpdateUI();
            //Debug.Log($"Mana regenerated: {currentMana}/{playerData.MaxMana}");
        }
    }

    // UI
    private void UpdateUI()
    {
        // HP
        float hpNorm = PlayerHp.currentHP / playerData.MaxHealth;
        HpSlider.fillAmount = Mathf.Clamp01(hpNorm);
        HpText.text = $"{PlayerHp.currentHP:0}/{playerData.MaxHealth:0}";

        // Mana
        float manaNorm = currentMana / playerData.MaxMana;
        ManaSlider.fillAmount = Mathf.Clamp01(manaNorm);
        ManaText.text = $"{currentMana:0}/{playerData.MaxMana:0}";

        if (CooldownSlider != null)
        {
            CooldownSlider.fillAmount = currentCooldown / skillCooldown;
        }
    }
}
