using UnityEngine;
using System.Collections.Generic;

public class CarrotPlayer_PetalMeadow : MonoBehaviour
{
    public static CarrotPlayer_PetalMeadow Instance; // Để lợn dễ tìm

    [Header("Di Chuyển")]
    public float moveSpeed = 8f;
    public float rotateSpeed = 15f;
    public VariableJoystick_PetalMeadow joystick; // Kéo joystick vào đây

    [Header("Vùng Thu Phục")]
    public float tamingRadius = 3.5f; // Bán kính vòng tròn quanh Player
    [Tooltip("Kéo một Sprite hình tròn vào đây để làm vòng tròn dưới chân Player (nếu muốn hiện trong game)")]
    public Transform visualRingObject;

    [Header("Điểm Target cho Lợn")]
    [Tooltip("Tạo 1 Empty GameObject con của Player, đặt phía sau lưng Player một chút")]
    public Transform pigFollowTarget; // Điểm mà lợn sẽ đi đến

    [Header("Danh sách lợn đệ tử")]
    public List<PigBehavior_PetalMeadow> myPigs = new List<PigBehavior_PetalMeadow>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Tự tìm joystick nếu chưa gán
        if (joystick == null) joystick = FindFirstObjectByType<VariableJoystick_PetalMeadow>();

        // Chỉnh kích thước vòng tròn hiển thị cho khớp với bán kính logic
        if (visualRingObject != null)
        {
            // Giả sử Sprite gốc kích thước 1x1 đơn vị, ta scale nó lên
            // Nhân 2 vì Radius là bán kính, Scale là đường kính
            visualRingObject.localScale = new Vector3(tamingRadius * 2, tamingRadius * 2, 1);
        }

        // Tự tạo Target nếu chưa có
        if (pigFollowTarget == null)
        {
            GameObject targetObj = new GameObject("PigFollowTarget");
            targetObj.transform.SetParent(transform);
            targetObj.transform.localPosition = new Vector3(0, 0, -2f); // Phía sau Player 2 đơn vị
            pigFollowTarget = targetObj.transform;
            Debug.Log("Đã tự động tạo PigFollowTarget phía sau Player");
        }

        // ✅ Đảm bảo Player spawn trong bounds ngay từ đầu
        if (PenBoundsManager_PetalMeadow.Instance != null)
        {
            Vector3 clampedPos = PenBoundsManager_PetalMeadow.Instance.ClampPlayerPosition(transform.position);
            transform.position = clampedPos;
            Debug.Log("[Player] Đã điều chỉnh vị trí spawn vào trong vùng cho phép");
        }
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        if (joystick == null) return;

        Vector3 direction = new Vector3(joystick.Horizontal, 0, joystick.Vertical);

        if (direction.magnitude >= 0.1f)
        {
            // Tính toán vị trí mới
            Vector3 newPosition = transform.position + direction * moveSpeed * Time.deltaTime;

            // ✅ PLAYER: Có thể đi tự do cả Movement Areas + Pen Areas
            if (PenBoundsManager_PetalMeadow.Instance != null)
            {
                newPosition = PenBoundsManager_PetalMeadow.Instance.ClampPlayerPosition(newPosition);
            }

            // Di chuyển Player
            transform.position = newPosition;

            // Xoay người
            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRot, rotateSpeed * Time.deltaTime);
        }
    }

    public void AddPig(PigBehavior_PetalMeadow pig)
    {
        if (!myPigs.Contains(pig))
        {
            myPigs.Add(pig);
            // Lợn tự tìm Target trong code của nó rồi, không cần gán ở đây nữa
        }
    }

    // Vẽ vòng tròn đỏ trong Scene để debug
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, tamingRadius);

        // Vẽ điểm Target màu xanh lá
        if (pigFollowTarget != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(pigFollowTarget.position, 0.3f);
        }
    }
}