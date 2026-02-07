using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneFlowManager : MonoBehaviour
{
    public static SceneFlowManager Instance;

    [System.Serializable]
    public struct SceneEntry
    {
        public SceneType type;
        public string sceneName;
    }

    [Header("Scene Mapping")]
    [SerializeField] private List<SceneEntry> scenEntries;

    private Dictionary<SceneType, string> sceneMap;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        sceneMap = new Dictionary<SceneType, string>();
        foreach (var entry in scenEntries)
        {
            sceneMap[entry.type] = entry.sceneName;
        }

        Debug.Log("[SceneFlowManager] Initialized");
    }

    void Start()
    {
        // BootstrapScene에서 시작하면
        LoadScene(SceneType.Title);
    }

    public void LoadScene(SceneType type)
    {
        if (!sceneMap.ContainsKey(type))
        {
            Debug.LogError($"SceneType {type} not mapped!");
            return;
        }

        SceneManager.LoadScene(sceneMap[type]);
    }
}
