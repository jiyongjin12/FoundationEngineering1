using UnityEngine;
using System.Collections.Generic;

public class FXManager : MonoBehaviour
{
    public static FXManager Instance { get; private set; }

    // 이펙트들 보관 클래스
    [System.Serializable]
    public class EffectEntry
    {
        public string key;
        public GameObject prefab;     
    }

    [Header("이펙트 목록")]
    public List<EffectEntry> effects = new List<EffectEntry>();

    // 이펙트들 보관
    private Dictionary<string, GameObject> effectDict;

    // 싱글톤
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDictionary();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeDictionary()
    {
        effectDict = new Dictionary<string, GameObject>();
        foreach (var entry in effects)
        {
            if (!effectDict.ContainsKey(entry.key))
            {
                effectDict.Add(entry.key, entry.prefab);
            }
        }
    }

    // 부모 객체 자식으로 생성
    public void PlayEffect(string key, Transform parent, float destroyAfter = 2f, Vector3 localOffset = default)
    {
        if (effectDict == null || !effectDict.ContainsKey(key))
        {
            Debug.LogWarning($"EffectManager: 키 '{key}'에 해당하는 이펙트가 없습니다.");
            return;
        }

        GameObject prefab = effectDict[key];
        GameObject instance = Instantiate(prefab, parent);
        instance.transform.localPosition = localOffset;
        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;

        if (destroyAfter > 0)
        {
            Destroy(instance, destroyAfter);
        }
    }

    // 로컬좌표값으로 생성은 필요하면 따로 제작바람
}
