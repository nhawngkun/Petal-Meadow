using UnityEngine;
using DG.Tweening;

/// <summary>
/// Quản lý camera: Vị trí ban đầu → Follow Player → Win/Loss về lại vị trí ban đầu
/// ✅ FIX: Tìm lại player mỗi khi load level mới
/// </summary>
public class CameraController_PetalMeadow : MonoBehaviour
{
    public static CameraController_PetalMeadow Instance;

    [Header("🎯 Target")]
    public Transform player;

    [Header("📍 Vị Trí Ban Đầu")]
    private Vector3 startPosition;
    private Quaternion startRotation;

    [Header("🎮 Follow Player Settings")]
    public Vector3 followOffset = new Vector3(0, 10, -8);
    public Vector3 followRotation = new Vector3(45, 0, 0);
    public float followSmoothSpeed = 5f;
    public bool lookAtPlayer = false;

    [Header("⏱️ Timing")]
    public float delayBeforeFollow = 1f;
    public float transitionDuration = 1.5f;

    [Header("🎬 Animation")]
    public Ease transitionEase = Ease.OutQuad;

    [Header("🎯 Trạng Thái")]
    public CameraState currentState = CameraState.Intro;

    private bool isFollowing = false;
    private Vector3 velocity = Vector3.zero;

    // ✅ EVENTS
    public delegate void OnCameraReturnedToStart();
    public event OnCameraReturnedToStart onCameraReturnedToStart;

    public enum CameraState
    {
        Intro,
        Following,
        ReturningToStart
    }

    void Awake()
    {
        // ✅ TRUE SINGLETON - Camera tồn tại across levels
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("⚠️ Đã có CameraController khác - Destroy duplicate");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("✅ CameraController: TRUE SINGLETON - Sẽ tồn tại across levels");
    }

    void OnDestroy()
    {
        // ✅ Unsubscribe events
        if (GameManager_PetalMeadow.Instance != null)
        {
            GameManager_PetalMeadow.Instance.onGameWin -= OnGameWin;
        }

        // ⚠️ KHÔNG clear Instance vì Camera là true singleton - không bị destroy
    }

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;

        // ✅ XÓA reference player cũ
        player = null;

        // Subscribe events
        if (GameManager_PetalMeadow.Instance != null)
        {
            GameManager_PetalMeadow.Instance.onGameWin += OnGameWin;
        }

        currentState = CameraState.Intro;

        Debug.Log("📷 Camera: Khởi tạo - chờ level được load");
    }

    void LateUpdate()
    {
        if (currentState == CameraState.Following && isFollowing && player != null)
        {
            FollowPlayerSmooth();
        }
    }

    /// <summary>
    /// ✅ Tự động tìm Player trong scene - GỌI MỖI KHI CẦN
    /// </summary>
    void FindPlayer()
    {
        Debug.Log("🔍 Camera: Bắt đầu tìm Player...");

        // ✅ LUÔN xóa reference cũ trước
        player = null;

        // Thử tìm qua Instance trước
        if (CarrotPlayer_PetalMeadow.Instance != null)
        {
            player = CarrotPlayer_PetalMeadow.Instance.transform;
            Debug.Log($"📷 Camera: Tìm thấy Player qua Instance - {player.name}");
            return;
        }

        // Nếu không có Instance, tìm trong scene
        CarrotPlayer_PetalMeadow foundPlayer = FindFirstObjectByType<CarrotPlayer_PetalMeadow>();
        if (foundPlayer != null)
        {
            player = foundPlayer.transform;
            Debug.Log($"📷 Camera: Tìm thấy Player trong scene - {player.name}");
        }
        else
        {
            Debug.LogWarning("📷 Camera: KHÔNG tìm thấy Player trong scene!");
        }
    }

    /// <summary>
    /// ✅ Bắt đầu follow player - Gọi từ LevelManager sau khi load level
    /// </summary>
    public void StartFollowingPlayer()
    {
        Debug.Log("📷 Camera: StartFollowingPlayer được gọi");

        // ✅ LUÔN tìm lại player trước khi follow
        FindPlayer();

        if (player == null)
        {
            Debug.LogError("📷 Camera: KHÔNG tìm thấy Player! Retry sau 0.05s");

            // ✅ Thử tìm lại ngay sau 1 frame (rất nhanh)
            Invoke(nameof(RetryFindPlayer), 0.05f);
            return;
        }

        Debug.Log($"📷 Camera: Bắt đầu di chuyển đến Player: {player.name}");

        currentState = CameraState.Following;

        Vector3 targetPosition = player.position + followOffset;
        Quaternion targetRotation = Quaternion.Euler(followRotation);

        transform.DOMove(targetPosition, transitionDuration)
            .SetEase(transitionEase)
            .OnComplete(() =>
            {
                isFollowing = true;
                Debug.Log("📷 Camera: Đã chuyển sang chế độ Follow Player");
            });

        transform.DORotateQuaternion(targetRotation, transitionDuration)
            .SetEase(transitionEase);
    }

    /// <summary>
    /// ✅ Thử tìm lại player nếu lần đầu không thấy
    /// </summary>
    void RetryFindPlayer()
    {
        Debug.Log("🔄 Camera: Retry tìm Player...");

        FindPlayer();

        if (player != null)
        {
            Debug.Log("✅ Camera: Đã tìm thấy Player sau khi retry!");
            StartFollowingPlayer();
        }
        else
        {
            Debug.LogError("❌ Camera: VẪN không tìm thấy Player sau khi retry!");
        }
    }

    void FollowPlayerSmooth()
    {
        if (player == null)
        {
            Debug.LogWarning("📷 Camera: Player reference bị null trong FollowPlayerSmooth!");
            FindPlayer();
            return;
        }

        Vector3 targetPosition = player.position + followOffset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            1f / followSmoothSpeed
        );

        if (lookAtPlayer)
        {
            Vector3 lookDirection = player.position - transform.position;
            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    targetRotation,
                    followSmoothSpeed * Time.deltaTime
                );
            }
        }
    }

    /// <summary>
    /// 🎉 Khi win - về lại vị trí ban đầu
    /// </summary>
    void OnGameWin()
    {
        ReturnToStartPosition(true);
    }

    /// <summary>
    /// ⏱ Khi loss - về lại vị trí ban đầu
    /// </summary>
    public void OnLoss()
    {
        ReturnToStartPosition(false);
    }

    /// <summary>
    /// 🔄 Về lại vị trí ban đầu và trigger event sau khi hoàn thành
    /// </summary>
    void ReturnToStartPosition(bool isWin)
    {
        isFollowing = false;
        currentState = CameraState.ReturningToStart;

        Debug.Log($"📷 Camera: {(isWin ? "WIN" : "LOSS")}! Quay về vị trí ban đầu");

        transform.DOMove(startPosition, transitionDuration)
            .SetEase(transitionEase);

        transform.DORotateQuaternion(startRotation, transitionDuration)
            .SetEase(transitionEase)
            .OnComplete(() =>
            {
                Debug.Log("📷 Camera: Đã về vị trí ban đầu!");

                // ✅ TRIGGER EVENT - Hiện UI Win/Loss
                onCameraReturnedToStart?.Invoke();

                if (isWin)
                {
                    UIManager_PetalMeadow.Instance.EnableGameplay(false);
                    UIManager_PetalMeadow.Instance.EnableWin(true);
                }
                else
                {
                    UIManager_PetalMeadow.Instance.EnableGameplay(false);
                    UIManager_PetalMeadow.Instance.EnableLoss(true);
                }
            });
    }

    /// <summary>
    /// 🔄 Reset camera về vị trí ban đầu (dùng khi replay/load level mới)
    /// </summary>
    public void ResetToStart()
    {
        Debug.Log("📷 Camera: ResetToStart được gọi");

        // Kill tất cả animations đang chạy
        DOTween.Kill(transform);

        // ✅ RESET NGAY LẬP TỨC - không animation để tránh giật
        transform.position = startPosition;
        transform.rotation = startRotation;
        currentState = CameraState.Intro;
        isFollowing = false;
        velocity = Vector3.zero;

        // ✅ XÓA reference player cũ
        player = null;
        Debug.Log("🗑️ Camera: Đã xóa reference player cũ");

        // ✅ Bắt đầu follow sau delay
        Invoke(nameof(StartFollowingPlayer), delayBeforeFollow);

        Debug.Log($"📷 Camera: Đã reset về ban đầu, sẽ follow player sau {delayBeforeFollow}s");
    }

    public void SetStartPosition(Vector3 position, Quaternion rotation)
    {
        startPosition = position;
        startRotation = rotation;
        Debug.Log($"📷 Camera: Đã set vị trí ban đầu mới - {position}");
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (Application.isPlaying)
        {
            Gizmos.DrawWireSphere(startPosition, 0.5f);
            Gizmos.DrawLine(startPosition, startPosition + startRotation * Vector3.forward * 2f);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, 0.5f);
            Gizmos.DrawLine(transform.position, transform.position + transform.rotation * Vector3.forward * 2f);
        }

        if (player != null)
        {
            Vector3 followPos = player.position + followOffset;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(followPos, 0.5f);

            if (Application.isPlaying)
            {
                Gizmos.DrawLine(transform.position, followPos);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Vector3 previewPos = player.position + followOffset;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(previewPos, 0.3f);
            Gizmos.DrawLine(player.position, previewPos);

            Quaternion previewRot = Quaternion.Euler(followRotation);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(previewPos, previewRot * Vector3.forward * 3f);
        }
    }
}