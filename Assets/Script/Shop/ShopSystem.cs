using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class ShopSystem : MonoBehaviour
{
    [Header("Unit Configuration")]
    [Tooltip("상점에서 판매할 UnitData 목록 (Inspector에서 설정)")]
    public List<UnitData> unitDataList;

    [Tooltip("Unit 버튼 프리팹 (버튼 자식에 TMP_Text가 있어야 함)")]
    public Button unitButtonPrefab;
    [Tooltip("버튼들을 배치할 부모 Transform (GridLayout 권장)")]
    public Transform buttonParent;

    [Header("Popup UI Elements")]
    public GameObject detailPopup;
    public TMP_Text hpText;
    public TMP_Text levelText;
    public TMP_Text priceText;
    public Button buyButton;
    public Button closeButton;

    private UnitData selectedUnit;
    private bool isPopupOpen = false;

    void Start()
    {
        // 버튼 동적 생성
        if (unitDataList == null || unitButtonPrefab == null || buttonParent == null)
        {
            Debug.LogError("ShopSystem: 필수 설정이 누락되었습니다! unitDataList, unitButtonPrefab, buttonParent를 확인하세요.");
            return;
        }

        for (int i = 0; i < unitDataList.Count; i++)
        {
            int idx = i;
            var ud = unitDataList[i];
            var btn = Instantiate(unitButtonPrefab, buttonParent);
            btn.name = ud.name;

            // 버튼 라벨 설정
            var label = btn.GetComponentInChildren<TMP_Text>();
            if (label != null)
                label.text = ud.name;
            else
                Debug.LogWarning($"ShopSystem: 버튼 프리팹에 TMP_Text 컴포넌트가 없습니다. 유닛 '{ud.name}'");

            btn.onClick.AddListener(() => OnUnitButtonClicked(idx));
        }

        // 팝업 및 버튼 초기화
        if (detailPopup == null) Debug.LogError("ShopSystem: detailPopup가 할당되지 않음");
        else detailPopup.SetActive(false);

        if (buyButton == null) Debug.LogError("ShopSystem: buyButton이 할당되지 않음");
        else buyButton.onClick.AddListener(OnBuyClicked);

        if (closeButton == null) Debug.LogError("ShopSystem: closeButton이 할당되지 않음");
        else closeButton.onClick.AddListener(ClosePopup);
    }

    void Update()
    {
        // 팝업 열려있으면 실시간 갱신
        if (isPopupOpen && selectedUnit != null)
            RefreshPopup();
    }

    /// <summary>
    /// 유닛 버튼 클릭 시 호출
    /// </summary>
    void OnUnitButtonClicked(int index)
    {
        if (index < 0 || index >= unitDataList.Count)
        {
            Debug.LogError($"ShopSystem: 잘못된 버튼 인덱스 {index}");
            return;
        }
        selectedUnit = unitDataList[index];
        isPopupOpen = true;
        detailPopup.SetActive(true);
        RefreshPopup();
    }

    /// <summary>
    /// 팝업에 선택된 유닛 정보 표시
    /// </summary>
    void RefreshPopup()
    {
        if (selectedUnit == null)
        {
            Debug.LogError("ShopSystem: RefreshPopup 호출되었지만 selectedUnit이 null");
            return;
        }

        // HP
        if (hpText != null)
            hpText.text = selectedUnit.Hp.ToString();
        else
            Debug.LogError("ShopSystem: hpText가 할당되지 않음");

        // Level
        int currLv = selectedUnit.CurrentLevel;
        if (levelText != null)
            levelText.text = currLv.ToString();
        else
            Debug.LogError("ShopSystem: levelText가 할당되지 않음");

        // Price
        int price = (currLv == 0)
            ? selectedUnit.OpenPrice
            : selectedUnit.LevelupPrice * currLv;
        if (priceText != null)
            priceText.text = price.ToString();
        else
            Debug.LogError("ShopSystem: priceText가 할당되지 않음");

        // 구매 버튼 활성화
        if (buyButton != null)
            buyButton.interactable = currLv < selectedUnit.MaxLevel
                && MoneyManager.Instance != null
                && MoneyManager.Instance.Coins >= price;
    }

    /// <summary>
    /// 구매/레벨업 버튼 클릭
    /// </summary>
    void OnBuyClicked()
    {
        if (selectedUnit == null) return;
        int currLv = selectedUnit.CurrentLevel;
        int price = (currLv == 0)
            ? selectedUnit.OpenPrice
            : selectedUnit.LevelupPrice * currLv;

        if (MoneyManager.Instance == null)
        {
            Debug.LogError("ShopSystem: MoneyManager.Instance가 null");
            return;
        }
        if (!MoneyManager.Instance.TrySpend(price)) return;

        // 레벨업 (스크립터블 오브젝트 CurrentLevel 필드 직접 수정)
        if (selectedUnit.CurrentLevel < selectedUnit.MaxLevel)
        {
            selectedUnit.CurrentLevel++;
            Debug.Log($"ShopSystem: '{selectedUnit.name}' 레벨업: {currLv} → {selectedUnit.CurrentLevel}");
        }
    }

    /// <summary>
    /// 팝업 닫기 및 상점 종료 버튼 클릭
    /// </summary>
    void ClosePopup()
    {
        isPopupOpen = false;
        detailPopup.SetActive(false);
        SceneManager.LoadScene("StageSelect");
    }
}