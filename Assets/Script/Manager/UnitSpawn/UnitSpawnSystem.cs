using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSpawnSystem : MonoBehaviour
{
    [Header("Custom Units (Inspector 설정, 길이 10)")]
    public UnitData[] customUnits = new UnitData[10];

    [Header("Spawn Buttons UI (Inspector에서 10개 할당)")]
    public List<UnitSpawnButtonUI> spawnButtons;

    [Header("유닛 스폰 위치")]
    public Transform unitSpawnPos;

    private Button[] _buttons;
    private List<UnitData> unlocked => UnitStatusManager.Instance.unlockedUnits;

    void Start()
    {
        // 필수 필드 할당 체크
        if (customUnits == null || spawnButtons == null || unitSpawnPos == null)
        {
            Debug.LogError("UnitSpawnSystem: customUnits, spawnButtons, unitSpawnPos 중 할당되지 않은 항목이 있습니다.");
            enabled = false;
            return;
        }

        int count = Mathf.Min(customUnits.Length, spawnButtons.Count);
        _buttons = new Button[count];

        // 버튼 컴포넌트 연결 및 클릭 리스너 등록
        for (int i = 0; i < count; i++)
        {
            var ui = spawnButtons[i];
            if (ui == null)
            {
                Debug.LogError($"UnitSpawnSystem: spawnButtons[{i}]가 null입니다.");
                continue;
            }
            var btnComp = ui.GetComponent<Button>();
            if (btnComp == null)
            {
                Debug.LogError($"UnitSpawnSystem: spawnButtons[{i}]에 Button 컴포넌트가 없습니다.");
                continue;
            }
            _buttons[i] = btnComp;
            int idx = i; // 클로저 문제 방지
            btnComp.onClick.AddListener(() => OnSpawnButton(idx));
        }

        // 첫 버튼 상태 설정
        RefreshButtons();
    }

    void Update()
    {
        for (int i = 0; i < spawnButtons.Count; i++)
        {
            // 버튼이 아직 준비되지 않았거나, 쿨다운 중(interactable=false)이면 건너뛰기
            if (_buttons == null  i >= _buttons.Length  _buttons[i] == null || !_buttons[i].interactable)
        continue;

        KeyCode key;
        if (i < 9)
        {
            key = KeyCode.Alpha1 + i;   // 0→1, 1→2, ... , 8→9
        }
        else if (i == 9)
        {
            key = KeyCode.Alpha0;      // 9→0
        }
        else
        {
            continue;
        }

        if (Input.GetKeyDown(key))
        {
            OnSpawnButton(i);
        }
    }
}
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

        // 유닛 인스턴스화
        var go = Instantiate(ud.UnitBody, unitSpawnPos.position, Quaternion.identity);
        if (go.TryGetComponent<Unit>(out var comp))
            comp.unitData = ud;

        // 버튼 쿨다운 시작
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
