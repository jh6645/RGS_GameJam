using UnityEngine;

public enum SceneType
{
    Bootstrap,      // 최초 실행 씬
    Title,          // 타이틀
    HostConnect,    // 방 생성 (Host)
    ClientConnect,  // 방 참가 (Clinet)
    Lobby,          // 로비
    Game,           // 인게임
    Result          // 결과 화면
}
