using UnityEngine;
using UnityEngine.InputSystem; // 새 인풋 시스템
using Mirror;

public class PlayerPing : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputHandler inputHandler; // 같은 오브젝트에 있으면 자동으로 GetComponent
    [SerializeField] private Camera playerCamera;             // 비우면 자동으로 Camera.main
    [SerializeField] private GameObject pingPrefab;           // 다이아몬드 프리팹 (NetworkIdentity 필수)
    [SerializeField] private PingWheelUI pingWheelUI;         // 원형 UI

    [Header("Settings")]
    [SerializeField] private float minDragDistance = 30f;     // 몇 픽셀 이상 드래그해야 핑 확정

    private bool isPinging;
    private Vector2 startScreenPos;
    private Vector3 worldPingPos;
    private PingType currentType;

    private void Awake()
    {
        if (!inputHandler)
            inputHandler = GetComponent<PlayerInputHandler>();

        if (pingWheelUI == null)
        {
            // 비활성화된 오브젝트까지 포함해서 PingWheelUI 찾기
            pingWheelUI = Object.FindFirstObjectByType<PingWheelUI>(
                FindObjectsInactive.Include
            );
        }
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        if (playerCamera == null)
            playerCamera = Camera.main;

        HandlePingInput();
    }

    private void HandlePingInput()
    {
        if (!inputHandler.isCtrlPressed)
        {
            if (isPinging)
                CancelPing();
            return;
        }

        // Ctrl + 우클릭 눌렀을 때
        if (inputHandler.rightClickJustPressed)
        {
            isPinging = true;
            startScreenPos = inputHandler.mouseScreenPosition;
            worldPingPos = ScreenToWorld(startScreenPos);
            currentType = PingType.DefendMf;

            pingWheelUI?.Open(startScreenPos);
        }

        if (!isPinging) return;

        Vector2 currentPos = inputHandler.mouseScreenPosition;
        Vector2 dir = currentPos - startScreenPos;

        if (dir.magnitude >= minDragDistance)
        {
            currentType = GetPingTypeFromDirection(dir);
            pingWheelUI?.SetSelection(currentType);
        }
        else
        {
            pingWheelUI?.ClearSelection();
        }

        // 우클릭 뗐을 때 확정
        if (inputHandler.rightClickJustReleased)
        {
            if (dir.magnitude >= minDragDistance)
                CmdSpawnPing(worldPingPos, currentType);

            CancelPing();
        }
    }

    private void CancelPing()
    {
        isPinging = false;
        if (pingWheelUI != null)
            pingWheelUI.Close();
    }

    private Vector3 ScreenToWorld(Vector2 screenPos)
    {
        if (playerCamera == null) return Vector3.zero;

        Vector3 sp = new Vector3(screenPos.x, screenPos.y, -playerCamera.transform.position.z);
        Vector3 world = playerCamera.ScreenToWorldPoint(sp);
        world.z = 0f; // 2D면 0으로 고정
        return world;
    }

    private PingType GetPingTypeFromDirection(Vector2 dir)
    {
        if (dir.sqrMagnitude < 0.0001f)
            return PingType.DefendMf; // 기본값(위)

        Vector2 n = dir.normalized;

        float up = Vector2.Dot(n, Vector2.up);    // (0, 1)
        float right = Vector2.Dot(n, Vector2.right); // (1, 0)
        float down = Vector2.Dot(n, Vector2.down);  // (0,-1)
        float left = Vector2.Dot(n, Vector2.left);  // (-1,0)

        float max = Mathf.Max(up, right, down, left);

        // PingType 매핑: 위=DefendMf, 오른쪽=OnMyWay, 아래=AssistMe, 왼쪽=Missing
        if (Mathf.Approximately(max, up)) return PingType.DefendMf;
        if (Mathf.Approximately(max, right)) return PingType.OnMyWay;
        if (Mathf.Approximately(max, down)) return PingType.AssistMe;
        return PingType.Missing;
    }

    // -------- Mirror 통신 ----------

    [Command]
    private void CmdSpawnPing(Vector3 position, PingType type)
    {
        GameObject obj = Instantiate(pingPrefab, position, Quaternion.identity);

        PingMarker marker = obj.GetComponent<PingMarker>();
        if (marker != null)
        {
            marker.Type = type;
        }

        NetworkServer.Spawn(obj);
    }


}
