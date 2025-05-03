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
            Button btn = Instantiate(stageButton, SpawnButtonPos.transform);

            Text btnText = btn.GetComponentInChildren<Text>();
            if (btnText != null)
                btnText.text = $"Stage {data.stageNum}";

            int stageNum = data.stageNum;
            string sceneName = "Stage" + stageNum;

            btn.onClick.AddListener(() =>
            {
                Debug.Log($"Loading scene: {sceneName}");
                SceneManager.LoadScene(sceneName);
            });
        }
    }
}

