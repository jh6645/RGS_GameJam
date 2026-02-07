using UnityEngine;
using UnityEngine.UI;

public class TitleUIManager : MonoBehaviour
{

    [Header("Roots")]
    [SerializeField] private Transform menuRoot;

    [Header("Prefabs")]
    [SerializeField] private GameObject titleMenuPrefab;

    [Header("TitleManager")]
    [SerializeField] private TitleManager titleManager;


    private GameObject currentMenu;

    private void Awake()
    {
        if (titleManager == null)
            titleManager = FindObjectOfType<TitleManager>();
    }

    void Start()
    {
        ShowTitleMenu();
    }

    public void ShowTitleMenu()
    {

        if (currentMenu != null)
            return;


        currentMenu = Instantiate(titleMenuPrefab, menuRoot);


        // Prefab 내부 버튼 이벤트 연결
        ConnectButtons();
    }

    private void ConnectButtons()
    {
        if (titleManager == null)
            Debug.LogError("titleManager가 null입니다!");


        // 버튼 찾기
        Button hostButton = currentMenu.transform.Find("HostBtn").GetComponent<Button>();
        if (hostButton == null)
            Debug.LogError("HostBtn이 null입니다!");

        Button clientButton = currentMenu.transform.Find("ClientBtn").GetComponent<Button>();
        //Button configButton = currentMenu.transform.Find("ConfigBtn").GetComponent<Button>();
        Button quitButton = currentMenu.transform.Find("EndBtn").GetComponent<Button>();

        // 버튼 클릭 → TitleManager 함수 호출
        hostButton.onClick.RemoveAllListeners();
        hostButton.onClick.AddListener(titleManager.StartHost);
        clientButton.onClick.RemoveAllListeners();
        clientButton.onClick.AddListener(titleManager.StartClient);
        //configButton.onClick.AddListener(titleManager.);
        quitButton.onClick.RemoveAllListeners();
        quitButton.onClick.AddListener(titleManager.EndGame);
    }
}
