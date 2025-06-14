using UnityEngine;
using UnityEngine.SceneManagement;

public class StageLoadManager : MonoBehaviour
{
    public static StageLoadManager Instance { get; private set; }

    public StageGroup SelectedGroup { get; private set; }

    [SerializeField]
    public StageGroup Sele;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SelectAndLoad(StageGroup group, string sceneName)
    {
        SelectedGroup = group;
        Sele = group;
        SceneManager.LoadScene(sceneName);
    }

    public void LoadSelectedScene(string sceneName)
    {
        if (SelectedGroup == null)
        {
            Debug.LogError("StageLoadManager: No wave selected before loading scene.");
            return;
        }
        SceneManager.LoadScene(sceneName);
    }
}
