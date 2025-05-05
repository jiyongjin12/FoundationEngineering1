using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitSpawnManager : MonoBehaviour 
{
    [Header("Costom Unit")]
    public UnitData[] Custom = new UnitData[10];

    [Header("Deck Button")]
    public GameObject DeckButton;
    public Transform ButtonSpawnPos;
    public int ButtonCount;

    [Header("Spawn")]
    public Transform UnitSpawnPos;
    public GameObject Unit;

    private Button[] deckButtons;

    private void Start()
    {
        deckButtons = new Button[ButtonCount];
        StartButtonSetting();
    }

    void StartButtonSetting()
    {
        for (int i = 0; i < ButtonCount; i++)
        {
            GameObject btnObj = Instantiate(DeckButton, ButtonSpawnPos);
            btnObj.name = $"DeckButton_{i}";

            Image img = btnObj.GetComponent<Image>();
            Button btn = btnObj.GetComponent<Button>();

            if (i < Custom.Length && Custom[i] != null)
            {
                img.sprite = Custom[i].ButtonImage;
            }

            deckButtons[i] = btn;

            // 버튼 클릭 이벤트 등록
            int index = i; // 클로저 문제 방지
            btn.onClick.AddListener(() => OnDeckButtonClicked(index));
        }
    }

    void SpawnUnit(int index)
    {
        GameObject unitObj = Instantiate(Unit, UnitSpawnPos.position, Quaternion.identity); // 생성

        // 생성될 오브젝트에 데이터 할당
        Unit unit = unitObj.GetComponent<Unit>();
        unit.unitData = Custom[index];
    }


    void OnDeckButtonClicked(int index)
    {
        // 유효성 검사 및 CoolDown 중인지 확인
        if (index < 0 || index >= Custom.Length || Custom[index] == null)
            return;

        // 스폰
        SpawnUnit(index);

        // 버튼 비활성화 및 쿨다운 시작
        deckButtons[index].interactable = false;
        StartCoroutine(CooldownRoutine(index));
    }

    IEnumerator CooldownRoutine(int index)
    {
        float cd = Custom[index].CoolDownTime;
        yield return new WaitForSeconds(cd);
        deckButtons[index].interactable = true;
    }
}
