using UnityEngine;
using UnityEngine.UI;

public class dfsadf : MonoBehaviour
{
    [Header("Objects to Toggle")]
    public GameObject objectA;  // 활성화/비활성화가 bool값과 동일
    public GameObject objectB;  // bool값과 반대로 활성화

    [Header("UI Button")]
    public Button toggleButton;
    public Button toggleButton2;

    private bool state = false;

    void Start()
    {
        // 초기 상태 설정
        ApplyState();

        // 버튼 클릭 시 ToggleState 호출
        if (toggleButton != null)
            toggleButton.onClick.AddListener(ToggleState);
        if (toggleButton2 != null)
            toggleButton2.onClick.AddListener(ToggleState);
    }

    public void ToggleState()
    {
        state = !state;
        ApplyState();
    }


    private void ApplyState()
    {
        if (objectA != null)
            objectA.SetActive(state);
        if (objectB != null)
            objectB.SetActive(!state);
    }
}
