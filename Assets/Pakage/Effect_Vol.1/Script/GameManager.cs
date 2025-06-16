using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject[] Setact;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void setAcc(int num)
    {
        for (int i = 0; i < Setact.Length; i++)
        {
            Setact[i].SetActive(false);
        }

        Setact[num].SetActive(true);

    }

    public void Win()
    {
        UIManager.Instance.Win_UI.gameObject.SetActive(true);
        Time.timeScale = 0f;

        UIManager.Instance.Win_UI.DOFade(1f, 1f)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                // 딜레이 후 씬 전환
                DOVirtual.DelayedCall(2f, () =>
                {
                    Time.timeScale = 1f;
                    StageLoadManager.Instance.LoadSelectedScene("MainMenu");
                }).SetUpdate(true);
            });
        
    }

    public void Lose()
    {
        UIManager.Instance.Lose_UI.gameObject.SetActive(true);
        Time.timeScale = 0f;

        UIManager.Instance.Lose_UI.DOFade(1f, 1f)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                // 딜레이 후 씬 전환
                DOVirtual.DelayedCall(2f, () =>
                {
                    Time.timeScale = 1f;
                    StageLoadManager.Instance.LoadSelectedScene("MainMenu");
                }).SetUpdate(true);
            });
    }
}
