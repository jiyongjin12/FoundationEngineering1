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
    [Tooltip("�������� �Ǹ��� UnitData ��� (Inspector���� ����)")]
    public List<UnitData> unitDataList;

    [Tooltip("Unit ��ư ������ (��ư �ڽĿ� TMP_Text�� �־�� ��)")]
    public Button unitButtonPrefab;
    [Tooltip("��ư���� ��ġ�� �θ� Transform (GridLayout ����)")]
    public Transform buttonParent;

    [Header("Popup UI Elements")]
    public GameObject detailPopup;
    public TMP_Text nameText;
    public TMP_Text hpText;
    public TMP_Text damageText;
    public TMP_Text levelText;
    public TMP_Text priceText;
    public Button buyButton;
    public Button closeButton;

    private UnitData selectedUnit;
    private bool isPopupOpen = false;

    void Start()
    {
        // ��ư ���� ����
        if (unitDataList == null || unitButtonPrefab == null || buttonParent == null)
        {
            Debug.LogError("ShopSystem: �ʼ� ������ �����Ǿ����ϴ�! unitDataList, unitButtonPrefab, buttonParent�� Ȯ���ϼ���.");
            return;
        }

        for (int i = 0; i < unitDataList.Count; i++)
        {
            int idx = i;
            var ud = unitDataList[i];
            var btn = Instantiate(unitButtonPrefab, buttonParent);
            btn.name = ud.name;

            // ��ư �� ����
            var label = btn.GetComponentInChildren<TMP_Text>();
            if (label != null)
                label.text = ud.name;
            else
                Debug.LogWarning($"ShopSystem: ��ư �����տ� TMP_Text ������Ʈ�� �����ϴ�. ���� '{ud.name}'");

            btn.onClick.AddListener(() => OnUnitButtonClicked(idx));
        }

        // �˾� �� ��ư �ʱ�ȭ
        if (detailPopup == null) Debug.LogError("ShopSystem: detailPopup�� �Ҵ���� ����");
        else detailPopup.SetActive(false);

        if (buyButton == null) Debug.LogError("ShopSystem: buyButton�� �Ҵ���� ����");
        else buyButton.onClick.AddListener(OnBuyClicked);

        if (closeButton == null) Debug.LogError("ShopSystem: closeButton�� �Ҵ���� ����");
        else closeButton.onClick.AddListener(ClosePopup);
    }

    void Update()
    {
        // �˾� ���������� �ǽð� ����
        if (isPopupOpen && selectedUnit != null)
            RefreshPopup();
    }

    void OnUnitButtonClicked(int index)
    {
        if (index < 0 || index >= unitDataList.Count)
        {
            Debug.LogError($"ShopSystem: �߸��� ��ư �ε��� {index}");
            return;
        }
        selectedUnit = unitDataList[index];
        isPopupOpen = true;
        detailPopup.SetActive(true);
        RefreshPopup();
    }

    void RefreshPopup()
    {
        if (selectedUnit == null)
        {
            Debug.LogError("ShopSystem: RefreshPopup ȣ��Ǿ����� selectedUnit�� null");
            return;
        }

        // HP
        if (hpText != null)
            hpText.text = selectedUnit.Hp.ToString();

        // Damage
        if (damageText != null && selectedUnit.UnitDefaultSkill != null)
            damageText.text = selectedUnit.UnitDefaultSkill.Damage.ToString();
        else
            damageText.text = "0";

        // Name
        if (nameText != null)
            nameText.text = selectedUnit.name.ToString();

        // Level
        int currLv = selectedUnit.CurrentLevel;
        if (levelText != null)
            levelText.text = currLv.ToString();
        else
            Debug.LogError("ShopSystem: levelText�� �Ҵ���� ����");

        // Price
        int price = (currLv == 0)
            ? selectedUnit.OpenPrice
            : selectedUnit.LevelupPrice * currLv;
        if (priceText != null)
            priceText.text = price.ToString();
        else
            Debug.LogError("ShopSystem: priceText�� �Ҵ���� ����");

        // ���� ��ư Ȱ��ȭ
        if (buyButton != null)
            buyButton.interactable = currLv < selectedUnit.MaxLevel
                && MoneyManager.Instance != null
                && MoneyManager.Instance.Coins >= price;
    }

    void OnBuyClicked()
    {
        if (selectedUnit == null) return;
        int currLv = selectedUnit.CurrentLevel;
        int price = (currLv == 0)
            ? selectedUnit.OpenPrice
            : selectedUnit.LevelupPrice * currLv;

        if (MoneyManager.Instance == null)
        {
            Debug.LogError("ShopSystem: MoneyManager.Instance�� null");
            return;
        }
        if (!MoneyManager.Instance.TrySpend(price)) return;

        // ������ (��ũ���ͺ� ������Ʈ CurrentLevel �ʵ� ���� ����)
        if (selectedUnit.CurrentLevel < selectedUnit.MaxLevel)
        {
            selectedUnit.CurrentLevel++;
            Debug.Log($"ShopSystem: '{selectedUnit.name}' ������: {currLv} �� {selectedUnit.CurrentLevel}");
        }
    }

    void ClosePopup()
    {
        isPopupOpen = false;
        detailPopup.SetActive(false);
        SceneManager.LoadScene("StageSelect");
    }
}