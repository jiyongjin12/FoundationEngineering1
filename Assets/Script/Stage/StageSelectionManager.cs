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
        int prev = Mathf.Max(currentPage - 1, 0);
        int next = Mathf.Min(currentPage + 1, count - 1);
        int[] indices = new int[] { prev, currentPage, next };
        for (int i = 0; i < 3; i++)
            PopulateContainer(pageContainers[i], pages[indices[i]]);
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

            // 텍스트를 "currentPage-(i+1)" 로 설정
            tmp.text = $"{currentPage + 1}-{i + 1}";

            // 데이터 유무에 따라 활성/비활성
            if (i < page.StageGroups.Count)
            {
                btn.interactable = true;
                var wave = page.StageGroups[i];
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnStageSelected(wave));
            }
            else
            {
                btn.interactable = false;
                btn.onClick.RemoveAllListeners();
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


    void OnStageSelected(StageGroup group)
    {
        Debug.Log($"Selected Stage Group: {group.StageSeries}");
    }
}
