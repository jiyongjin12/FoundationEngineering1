using UnityEngine;
using UnityEngine.UI;

public class MyDeck : MonoBehaviour 
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


    private void Start()
    {
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

            // 버튼 클릭 이벤트 등록
            int index = i; // 클로저 문제 방지
            btn.onClick.AddListener(() => SpawnUnit(index));
        }
    }

    void SpawnUnit(int index)
    {
        if (index < 0 || index >= Custom.Length || Custom[index] == null)
            return;

        GameObject unitObj = Instantiate(Unit, UnitSpawnPos.position, Quaternion.identity); // 생성

        // 생성될 오브젝트에 데이터 할당
        Unit unit = unitObj.GetComponent<Unit>();
        unit.unitData = Custom[index];
    }

}
