using Mirror;
using Mirror.Discovery;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    // 버튼 클릭 시 호출되는 함수


    public void StartHost()
    {
        // 씬 전환은 SceneFlowManager에게 위임
        SceneFlowManager.Instance.LoadScene(SceneType.HostConnect);
    }

    public void StartClient()
    {
        SceneFlowManager.Instance.LoadScene(SceneType.ClientConnect);
    }

    public void EndGame()
    {
        Application.Quit();
    }
}
