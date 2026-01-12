using UnityEngine;
using System.Collections;

/// <summary>
/// Quản lý outline của lợn theo trạng thái:
/// - Không thuần phục: Không có outline
/// - Đã thuần phục: Outline màu trắng
/// - Vào đúng chuồng: Outline màu xanh lá
/// - Vào nhầm chuồng: Outline màu đỏ
/// - Mất thuần phục: Nhấp nháy rồi tắt
/// </summary>
[RequireComponent(typeof(PigBehavior_PetalMeadow))]
public class PigOutlineManager_PetalMeadow : MonoBehaviour
{
    [Header("🎨 Màu Outline")]
    public Color normalTamedColor = Color.white;      // Màu khi thuần phục bình thường
    public Color correctPenColor = Color.green;       // Màu khi vào đúng chuồng
    public Color wrongPenColor = Color.red;           // Màu khi vào nhầm chuồng

    [Header("⚙️ Cài Đặt Outline")]
    public float outlineWidth = 3f;
    public Outline.Mode outlineMode = Outline.Mode.OutlineVisible;

    [Header("✨ Hiệu Ứng Mất Thuần Phục")]
    public float blinkDuration = 1.5f;      // Tổng thời gian nhấp nháy
    public float blinkSpeed = 0.15f;        // Tốc độ nhấp nháy (càng nhỏ càng nhanh)
    public int blinkCount = 5;              // Số lần nhấp nháy

    [Header("🐛 Debug")]
    public bool showDebugLogs = true;

    private PigBehavior_PetalMeadow pigBehavior;
    private Outline outline;
    private bool wasInPen = false;
    private bool wasCorrectPen = false;
    private bool wasTamed = false;
    private bool isBlinking = false;

    void Awake()
    {
        pigBehavior = GetComponent<PigBehavior_PetalMeadow>();

        if (pigBehavior == null)
        {
            Debug.LogError($"❌ {gameObject.name}: Không tìm thấy PigBehavior_PetalMeadow!");
            enabled = false;
            return;
        }

        // ✅ Tìm hoặc thêm component Outline
        SetupOutline();
    }

    void Start()
    {
        // ✅ Delay 1 frame để đảm bảo PigBehavior đã khởi tạo xong
        StartCoroutine(InitializeOutline());
    }

    IEnumerator InitializeOutline()
    {
        yield return null; // Đợi 1 frame

        if (outline != null)
        {
            // ✅ TẮT outline lúc đầu (lợn chưa thuần phục)
            outline.enabled = false;

            if (showDebugLogs)
                Debug.Log($"[Outline] {gameObject.name}: Khởi tạo - outline TẮT (chưa thuần phục)");
        }
    }

    void SetupOutline()
    {
        // Tìm Outline trong chính object này
        outline = GetComponent<Outline>();

        // Nếu không có, tìm trong children
        if (outline == null)
        {
            outline = GetComponentInChildren<Outline>();
        }

        // Nếu vẫn không có, tạo mới
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();

            if (showDebugLogs)
                Debug.Log($"[Outline] {gameObject.name}: Đã thêm component Outline");
        }

        // ✅ Cấu hình outline
        outline.OutlineMode = outlineMode;
        outline.OutlineWidth = outlineWidth;
        outline.OutlineColor = normalTamedColor;

        if (showDebugLogs)
            Debug.Log($"[Outline] {gameObject.name}: Setup complete - Mode: {outlineMode}, Width: {outlineWidth}");
    }

    void Update()
    {
        if (pigBehavior == null || outline == null || isBlinking) return;

        UpdateOutlineState();
    }

    /// <summary>
    /// Cập nhật trạng thái outline dựa trên trạng thái của lợn
    /// </summary>
    void UpdateOutlineState()
    {
        bool isTamed = pigBehavior.IsTamed;
        bool isInPen = pigBehavior.IsInPen;
        bool isCorrectPen = pigBehavior.IsCorrectPen;

        // ⚡ Kiểm tra mất thuần phục (từ thuần phục → không thuần phục)
        if (wasTamed && !isTamed)
        {
            StartCoroutine(BlinkAndDisable());
            wasTamed = false;
            return;
        }

        // ❌ Chưa thuần phục → TẮT outline
        if (!isTamed)
        {
            if (outline.enabled)
            {
                outline.enabled = false;

                if (showDebugLogs)
                    Debug.Log($"[Outline] {gameObject.name}: TẮT outline (chưa thuần phục)");
            }
            wasTamed = false;
            return;
        }

        // ✅ Đã thuần phục → BẬT outline
        if (!outline.enabled)
        {
            outline.enabled = true;

            if (showDebugLogs)
                Debug.Log($"[Outline] {gameObject.name}: BẬT outline (đã thuần phục)");
        }

        // 🏠 Kiểm tra trạng thái trong chuồng
        if (isInPen)
        {
            // Vào đúng chuồng → Màu xanh lá
            if (isCorrectPen)
            {
                if (!wasInPen || !wasCorrectPen)
                {
                    outline.OutlineColor = correctPenColor;

                    if (showDebugLogs)
                        Debug.Log($"[Outline] {gameObject.name}: Đổi sang màu XANH LÁ (đúng chuồng)");

                    wasCorrectPen = true;
                }
            }
            // Vào nhầm chuồng → Màu đỏ
            else
            {
                if (!wasInPen || wasCorrectPen)
                {
                    outline.OutlineColor = wrongPenColor;

                    if (showDebugLogs)
                        Debug.Log($"[Outline] {gameObject.name}: Đổi sang màu ĐỎ (sai chuồng)");

                    wasCorrectPen = false;
                }
            }
            wasInPen = true;
        }
        // 🚶 Ngoài chuồng → Màu trắng (thuần phục bình thường)
        else
        {
            if (wasInPen || !wasTamed)
            {
                outline.OutlineColor = normalTamedColor;

                if (showDebugLogs)
                    Debug.Log($"[Outline] {gameObject.name}: Đổi sang màu TRẮNG (thuần phục bình thường)");

                wasInPen = false;
                wasCorrectPen = false;
            }
        }

        wasTamed = true;
    }

    /// <summary>
    /// ✨ Hiệu ứng nhấp nháy khi mất thuần phục
    /// </summary>
    IEnumerator BlinkAndDisable()
    {
        isBlinking = true;

        if (showDebugLogs)
            Debug.Log($"[Outline] {gameObject.name}: Bắt đầu nhấp nháy - mất thuần phục!");

        // Lưu màu hiện tại
        Color currentColor = outline.OutlineColor;

        // Nhấp nháy
        for (int i = 0; i < blinkCount; i++)
        {
            // Tắt
            outline.enabled = false;
            yield return new WaitForSeconds(blinkSpeed);

            // Bật
            outline.enabled = true;
            outline.OutlineColor = currentColor;
            yield return new WaitForSeconds(blinkSpeed);
        }

        // Tắt hoàn toàn sau khi nhấp nháy xong
        outline.enabled = false;

        isBlinking = false;
        wasInPen = false;
        wasCorrectPen = false;

        if (showDebugLogs)
            Debug.Log($"[Outline] {gameObject.name}: Kết thúc nhấp nháy - outline đã TẮT");
    }

    /// <summary>
    /// 🎨 Đổi màu outline thủ công (optional)
    /// </summary>
    public void SetOutlineColor(Color color)
    {
        if (outline != null)
        {
            outline.OutlineColor = color;
        }
    }

    /// <summary>
    /// ⚙️ Đổi độ rộng outline (optional)
    /// </summary>
    public void SetOutlineWidth(float width)
    {
        if (outline != null)
        {
            outline.OutlineWidth = width;
        }
    }

    /// <summary>
    /// 🔄 Reset outline về trạng thái ban đầu
    /// </summary>
    public void ResetOutline()
    {
        StopAllCoroutines(); // Dừng hiệu ứng nhấp nháy nếu đang chạy

        if (outline != null)
        {
            outline.enabled = false;
            wasInPen = false;
            wasCorrectPen = false;
            wasTamed = false;
            isBlinking = false;
        }
    }

    /// <summary>
    /// 🔧 Force bật outline để test
    /// </summary>
    [ContextMenu("Test - Enable Outline")]
    public void TestEnableOutline()
    {
        if (outline != null)
        {
            outline.enabled = true;
            outline.OutlineColor = Color.yellow;
            outline.OutlineWidth = outlineWidth;
            Debug.Log($"[Outline] {gameObject.name}: TEST - Bật outline màu vàng");
        }
    }

    /// <summary>
    /// 🔧 Force tắt outline để test
    /// </summary>
    [ContextMenu("Test - Disable Outline")]
    public void TestDisableOutline()
    {
        if (outline != null)
        {
            outline.enabled = false;
            Debug.Log($"[Outline] {gameObject.name}: TEST - Tắt outline");
        }
    }

    void OnDestroy()
    {
        // Cleanup
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        // Tự động cập nhật màu trong Editor khi thay đổi
        if (Application.isPlaying && outline != null && pigBehavior != null && !isBlinking)
        {
            UpdateOutlineState();
        }
    }
#endif
}