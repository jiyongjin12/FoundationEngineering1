using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSpawnSystem : MonoBehaviour
{
    [Header("Custom Units (Inspector ����, ���� 10)")]
    public UnitData[] customUnits = new UnitData[10];

    [Header("Spawn Buttons UI (Inspector���� 10�� �Ҵ�)")]
    public List<UnitSpawnButtonUI> spawnButtons;

    [Header("���� ���� ��ġ")]
    public Transform unitSpawnPos;

    private Button[] _buttons;
    private List<UnitData> unlocked => UnitStatusManager.Instance.unlockedUnits;

    void Start()
    {
        // �ʼ� �ʵ� �Ҵ� üũ
        if (customUnits == null || spawnButtons == null || unitSpawnPos == null)
        {
            Debug.LogError("UnitSpawnSystem: customUnits, spawnButtons, unitSpawnPos �� �Ҵ���� ���� �׸��� �ֽ��ϴ�.");
            enabled = false;
            return;
        }

        int count = Mathf.Min(customUnits.Length, spawnButtons.Count);
        _buttons = new Button[count];

        // ��ư ������Ʈ ���� �� Ŭ�� ������ ���
        for (int i = 0; i < count; i++)
        {
            var ui = spawnButtons[i];
            if (ui == null)
            {
                Debug.LogError($"UnitSpawnSystem: spawnButtons[{i}]�� null�Դϴ�.");
                continue;
            }
            var btnComp = ui.GetComponent<Button>();
            if (btnComp == null)
            {
                Debug.LogError($"UnitSpawnSystem: spawnButtons[{i}]�� Button ������Ʈ�� �����ϴ�.");
                continue;
            }
            _buttons[i] = btnComp;
            int idx = i; // Ŭ���� ���� ����
            btnComp.onClick.AddListener(() => OnSpawnButton(idx));
        }

        // ù ��ư ���� ����
        RefreshButtons();
    }

    void Update()
    {
        // Ű���� �Է� (1~9, 0Ű)�ε� ����
        for (int i = 0; i < spawnButtons.Count; i++)
        {
            // 0�� ��ư -> Alpha1, 1�� -> Alpha2, ..., 8�� -> Alpha9, 9�� -> Alpha0
            KeyCode code = (i < 9)
                ? KeyCode.Alpha1 + i
                : KeyCode.Alpha0;
            if (Input.GetKeyDown(code))
                OnSpawnButton(i);
        }
    }

    //void Update()
    //{
    //    // Ű���� �Է� (1~0Ű)�ε� ����
    //    for (int i = 0; i < spawnButtons.Count; i++)
    //    {
    //        var ui = spawnButtons[i];
    //        if (ui == null) continue;
    //        KeyCode code = KeyCode.Alpha0 + ui.UseKeyCode;
    //        if (Input.GetKeyDown(code))
    //            OnSpawnButton(i);
    //    }
    //}

    void RefreshButtons()
    {
        for (int i = 0; i < _buttons.Length; i++)
        {
            var ud = customUnits[i];
            _buttons[i].interactable = (ud != null && unlocked.Contains(ud));
        }
    }

    void OnSpawnButton(int index)
    {
        if (index < 0 || index >= customUnits.Length) return;
        var ud = customUnits[index];
        if (ud == null || !unlocked.Contains(ud)) return;

        // ���� �ν��Ͻ�ȭ
        var go = Instantiate(ud.UnitBody, unitSpawnPos.position, Quaternion.identity);
        if (go.TryGetComponent<Unit>(out var comp))
            comp.unitData = ud;

        // ��ư ��ٿ� ����
        StartCoroutine(CooldownRoutine(index, ud.CoolDownTime));
    }

    IEnumerator CooldownRoutine(int idx, float duration)
    {
        var ui = spawnButtons[idx];
        var btn = _buttons[idx];
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
