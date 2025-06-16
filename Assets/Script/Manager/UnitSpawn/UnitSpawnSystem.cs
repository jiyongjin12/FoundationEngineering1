using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSpawnSystem : MonoBehaviour
{
    [Header("Custom Units (���� 10)")]
    public UnitData[] customUnits = new UnitData[10];

    [Header("Spawn Buttons UI (10��)")]
    public List<UnitSpawnButtonUI> spawnButtons;

    [Header("���� ���� ��ġ")]
    public Transform unitSpawnPos;

    private Button[] _buttons;

    void Start()
    {
        // �ʼ� �ʵ� �Ҵ� üũ
        if (customUnits == null || spawnButtons == null || unitSpawnPos == null)
        {
            Debug.LogError("UnitSpawnSystem: �ʼ� �׸��� �Ҵ���� �ʾҽ��ϴ�.");
            enabled = false;
            return;
        }

        int count = Mathf.Min(customUnits.Length, spawnButtons.Count);
        _buttons = new Button[count];

        // ��ư ����
        for (int i = 0; i < count; i++)
        {
            var ui = spawnButtons[i];
            if (ui == null)
            {
                Debug.LogError($"UnitSpawnSystem: spawnButtons[{i}]�� null�Դϴ�.");
                continue;
            }

            var btn = ui.GetComponent<Button>();
            if (btn == null)
            {
                Debug.LogError($"UnitSpawnSystem: spawnButtons[{i}]�� Button ������Ʈ�� �����ϴ�.");
                continue;
            }

            _buttons[i] = btn;
            int idx = i;
            btn.onClick.AddListener(() => OnSpawnButton(idx));
        }
    }

    void Update()
    {
        // Ű���� �Է� (1~0Ű)�� ����
        if (Input.GetKeyDown(KeyCode.Alpha1)) OnSpawnButton(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) OnSpawnButton(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) OnSpawnButton(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) OnSpawnButton(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) OnSpawnButton(4);
        if (Input.GetKeyDown(KeyCode.Alpha6)) OnSpawnButton(5);
        if (Input.GetKeyDown(KeyCode.Alpha7)) OnSpawnButton(6);
        if (Input.GetKeyDown(KeyCode.Alpha8)) OnSpawnButton(7);
        if (Input.GetKeyDown(KeyCode.Alpha9)) OnSpawnButton(8);
        if (Input.GetKeyDown(KeyCode.Alpha0)) OnSpawnButton(9);
    }

    void OnSpawnButton(int index)
    {
        if (index < 0 || index >= customUnits.Length) return;

        var ud = customUnits[index];
        if (ud == null)
        {
            Debug.LogWarning($"UnitSpawnSystem: customUnits[{index}]�� null�Դϴ�.");
            return;
        }

        Debug.Log($"�� ���� ��ȯ: {ud.name} (index: {index})");

        var go = Instantiate(ud.UnitBody, unitSpawnPos.position, Quaternion.identity);
        if (go.TryGetComponent<Unit>(out var comp))
            comp.unitData = ud;

        StartCoroutine(CooldownRoutine(index, ud.CoolDownTime));
    }

    IEnumerator CooldownRoutine(int index, float duration)
    {
        var ui = spawnButtons[index];
        var btn = _buttons[index];
        btn.interactable = false;

        float timer = duration;
        while (timer > 0f)
        {
            ui.UnitSpawnCoolDownSlide.fillAmount = timer / duration;
            ui.UnitSpawnCoolDownText.text = $"{Mathf.CeilToInt(timer)}s";
            timer -= Time.deltaTime;
            yield return null;
        }

        ui.UnitSpawnCoolDownSlide.fillAmount = 0f;
        ui.UnitSpawnCoolDownText.text = "";
        btn.interactable = true;
    }
}
