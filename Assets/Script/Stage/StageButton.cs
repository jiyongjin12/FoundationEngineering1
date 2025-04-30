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
            // ��ư ���� �� �θ� ����
            Button btn = Instantiate(stageButton, SpawnButtonPos.transform);

            // ��ư �ؽ�Ʈ ����
            Text btnText = btn.GetComponentInChildren<Text>();
            if (btnText != null)
                btnText.text = $"Stage {data.stageNum}";

            // ĸó�� ���� ���� ����
            int stageNum = data.stageNum;
            string sceneName = "Stage" + stageNum;

            // ��ư Ŭ�� �� �ش� �� �ε�
            btn.onClick.AddListener(() =>
            {
                Debug.Log($"Loading scene: {sceneName}");
                SceneManager.LoadScene(sceneName);
            });
        }
    }
}

