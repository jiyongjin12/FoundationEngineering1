using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSpawnSystem : MonoBehaviour
{
    [Header("Custom Units (길이 10)")]
    public UnitData[] customUnits = new UnitData[10];

    [Header("Spawn Buttons UI (10개)")]
    public List<UnitSpawnButtonUI> spawnButtons;

    [Header("유닛 스폰 위치")]
    public Transform unitSpawnPos;

    private Button[] _buttons;

    void Start()
    {
        // 필수 필드 할당 체크
        if (customUnits == null || spawnButtons == null || unitSpawnPos == null)
        {
            Debug.LogError("UnitSpawnSystem: 필수 항목이 할당되지 않았습니다.");
            enabled = false;
            return;
        }

        int count = Mathf.Min(customUnits.Length, spawnButtons.Count);
        _buttons = new Button[count];

        // 버튼 설정
        for (int i = 0; i < count; i++)
        {
            var ui = spawnButtons[i];
            if (ui == null)
            {
                Debug.LogError($"UnitSpawnSystem: spawnButtons[{i}]가 null입니다.");
                continue;
            }

            var btn = ui.GetComponent<Button>();
            if (btn == null)
            {
                Debug.LogError($"UnitSpawnSystem: spawnButtons[{i}]에 Button 컴포넌트가 없습니다.");
                continue;
            }

            _buttons[i] = btn;
            int idx = i;
            btn.onClick.AddListener(() => OnSpawnButton(idx));
        }
    }

    void Update()
    {
        // 키보드 입력 (1~0키)로 스폰
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
            Debug.LogWarning($"UnitSpawnSystem: customUnits[{index}]가 null입니다.");
            return;
        }

        Debug.Log($"▶ 유닛 소환: {ud.name} (index: {index})");

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
