using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;
using System.Runtime.ConstrainedExecution;

[Serializable]
public class StagePage
{
    public int PageIndex;                // 0-based page index
    public List<StageGroup> StageGroups; // up to 10 per page
}

public class StageSelectionManager : MonoBehaviour
{
    [Header("Stage Group Database")]
    public SheetStageDatabase stageGroupDatabase;

    [Header("Page Settings")]
    public int itemsPerPage = 10;
    public float panelWidth = 800f;
    public float animationDuration = 0.5f;
    public Ease animationEase = Ease.OutCubic;

    [Header("UI Containers")]
    public RectTransform[] pageContainers = new RectTransform[3]; // left(0), center(1), right(2)

    [Header("Navigation Buttons")]
    public Button prevButton;
    public Button nextButton;

    public List<StagePage> pages;
    public int currentPage = 0;
    private Vector2[] initialPositions = new Vector2[3];
    private bool isAnimating = false;

    void Awake()
    {
        if (stageGroupDatabase == null)
        {
            Debug.LogError("Stage Group Database is not assigned");
            return;
        }
        BuildPages();
        for (int i = 0; i < 3; i++)
            if (pageContainers[i] != null)
                initialPositions[i] = pageContainers[i].anchoredPosition;
    }

    void Start()
    {
        UpdateButtons();
        UpdatePageViews();
        prevButton.onClick.AddListener(OnPrevClicked);
        nextButton.onClick.AddListener(OnNextClicked);
    }

    void BuildPages()
    {
        pages = new List<StagePage>();
        var groups = stageGroupDatabase.stages;
        int pageCount = Mathf.CeilToInt((float)groups.Count / itemsPerPage);
        for (int p = 0; p < pageCount; p++)
        {
            pages.Add(new StagePage
            {
                PageIndex = p,
                StageGroups = groups.Skip(p * itemsPerPage).Take(itemsPerPage).ToList()
            });
        }
    }

    void OnNextClicked()
    {
        if (isAnimating || currentPage >= pages.Count - 1) return;
        isAnimating = true;
        ToggleNavButtons(false);
        for (int i = 0; i < 3; i++)
        {
            float targetX = initialPositions[i].x - panelWidth;
            pageContainers[i].DOAnchorPosX(targetX, animationDuration).SetEase(animationEase);
        }
        DOVirtual.DelayedCall(animationDuration, () =>
        {
            currentPage++;
            ResetPanels();
            UpdateButtons();
            ToggleNavButtons(true);
            isAnimating = false;
        });
    }

    void OnPrevClicked()
    {
        if (isAnimating || currentPage <= 0) return;
        isAnimating = true;
        ToggleNavButtons(false);
        for (int i = 0; i < 3; i++)
        {
            float targetX = initialPositions[i].x + panelWidth;
            pageContainers[i].DOAnchorPosX(targetX, animationDuration).SetEase(animationEase);
        }
        DOVirtual.DelayedCall(animationDuration, () =>
        {
            currentPage--;
            ResetPanels();
            UpdateButtons();
            ToggleNavButtons(true);
            isAnimating = false;
        });
    }

    void ResetPanels()
    {
        for (int i = 0; i < 3; i++)
            if (pageContainers[i] != null)
                pageContainers[i].anchoredPosition = initialPositions[i];
        UpdatePageViews();
    }

    void UpdatePageViews()
    {
        if (pages == null || pages.Count == 0) return;

        int count = pages.Count;

        // 좌우 페이지 설정 (순환 구조로 인덱스 변경)
        int prevIndex = currentPage - 1 < 0 ? pages.Count - 1 : currentPage - 1;
        int nextIndex = currentPage + 1 >= pages.Count ? 0 : currentPage + 1;

        int[] pageIndices = new int[] { prevIndex, currentPage, nextIndex };

        for (int i = 0; i < 3; i++)
        {
            PopulateContainer(pageContainers[i], pages[pageIndices[i]]);
        }
    }

    void PopulateContainer(RectTransform container, StagePage page)
    {
        var buttons = container.GetComponentsInChildren<Button>(true)
        .Where(b => b.transform.parent == container)
        .OrderBy(b => b.transform.GetSiblingIndex())
        .ToArray();

        for (int i = 0; i < buttons.Length; i++)
        {
            var btn = buttons[i];
            var tmp = btn.GetComponentInChildren<TMPro.TMP_Text>(true);

            if (i < page.StageGroups.Count)
            {
                btn.interactable = true;

                var group = page.StageGroups[i];

                // 여기서 PageIndex를 사용해야 정확함
                tmp.text = string.Format("{0}-{1}", page.PageIndex + 1, i + 1);

                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    Debug.Log($"[StageSelection] Selected Wave -> Series:{group.StageSeries}, Waves:{group.Waves.Count}");
                    StageLoadManager.Instance.SelectAndLoad(group, "Stage0");
                });
            }
            else
            {
                tmp.text = "";
                btn.interactable = false;
            }
        }
    }

    void UpdateButtons()
    {
        prevButton.gameObject.SetActive(currentPage > 0);
        nextButton.gameObject.SetActive(currentPage < pages.Count - 1);
    }

    void ToggleNavButtons(bool active)
    {
        prevButton.gameObject.SetActive(active && currentPage > 0);
        nextButton.gameObject.SetActive(active && currentPage < pages.Count - 1);
    }

}
