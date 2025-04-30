using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageButton : MonoBehaviour 
{
    public StageData[] SDataList;

    public Button stageButton;
    public GameObject SpawnButtonPos;

    public void Start()
    {
        foreach (StageData data in SDataList)
        {
            // 버튼 생성 및 부모 설정
            Button btn = Instantiate(stageButton, SpawnButtonPos.transform);

            // 버튼 텍스트 변경
            Text btnText = btn.GetComponentInChildren<Text>();
            if (btnText != null)
                btnText.text = $"Stage {data.stageNum}";

            // 캡처를 위한 지역 변수
            int stageNum = data.stageNum;
            string sceneName = "Stage" + stageNum;

            // 버튼 클릭 시 해당 씬 로드
            btn.onClick.AddListener(() =>
            {
                Debug.Log($"Loading scene: {sceneName}");
                SceneManager.LoadScene(sceneName);
            });
        }
    }
}

