using UnityEngine;
using System.Collections.Generic;

public class FXManager : MonoBehaviour
{
    public static FXManager Instance { get; private set; }

    // ����Ʈ�� ���� Ŭ����
    [System.Serializable]
    public class EffectEntry
    {
        public string key;
        public GameObject prefab;     
    }

    [Header("����Ʈ ���")]
    public List<EffectEntry> effects = new List<EffectEntry>();

    // ����Ʈ�� ����
    private Dictionary<string, GameObject> effectDict;

    // �̱���
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

    // �θ� ��ü �ڽ����� ����
    public void PlayEffect(string key, Transform parent, float destroyAfter = 2f, Vector3 localOffset = default)
    {
        if (effectDict == null || !effectDict.ContainsKey(key))
        {
            Debug.LogWarning($"EffectManager: Ű '{key}'�� �ش��ϴ� ����Ʈ�� �����ϴ�.");
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

    // ������ǥ������ ������ �ʿ��ϸ� ���� ���۹ٶ�
}
