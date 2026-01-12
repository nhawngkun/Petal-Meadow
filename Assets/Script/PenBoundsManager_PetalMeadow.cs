using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Quản lý 2 loại vùng: Movement Areas (lợn lang thang) và Pen Areas (chuồng - chỉ lợn thuần phục vào được)
/// </summary>
public class PenBoundsManager_PetalMeadow : MonoBehaviour
{
    public static PenBoundsManager_PetalMeadow Instance;

    [Header("⭐ VÙNG DI CHUYỂN (Movement Areas)")]
    [Tooltip("Kéo các GameObject vùng di chuyển vào đây - Lợn hoang có thể lang thang ở đây")]
    public List<GameObject> movementAreas = new List<GameObject>();

    [Header("🏠 VÙNG CHUỒNG (Pen Areas)")]
    [Tooltip("Kéo các GameObject chuồng vào đây - CHỈ lợn thuần phục mới vào được")]
    public List<GameObject> penAreas = new List<GameObject>();

    [Header("🎨 MÀU CHUỒNG")]
    [Tooltip("Chọn màu cho từng chuồng - theo thứ tự trong danh sách Pen Areas")]
    public List<PigColor> penColors = new List<PigColor>();

    [Header("🎨 Debug")]
    public bool showDebugGizmos = true;
    public Color movementAreaColor = Color.green;  // Màu vùng di chuyển
    public Color penAreaColor = Color.yellow;      // Màu chuồng

    private List<Bounds> movementBounds = new List<Bounds>();
    private List<Bounds> penBounds = new List<Bounds>();

    void Awake()
    {
        // ✅ KHÔNG dùng singleton pattern - mỗi level có PenBoundsManager riêng
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("⚠️ Đã có PenBoundsManager khác - Destroy instance cũ");
            Destroy(Instance.gameObject);
        }
        
        Instance = this;
        Debug.Log("✅ PenBoundsManager: Awake - Instance đã được set");
    }

    void Start()
    {
        SetupAllBounds();
    }

    void OnDestroy()
    {
        // ✅ Clear Instance khi bị destroy
        if (Instance == this)
        {
            Instance = null;
            Debug.Log("🔄 PenBoundsManager: Đã clear Instance");
        }
    }

    /// <summary>
    /// Thiết lập tất cả các vùng
    /// </summary>
    void SetupAllBounds()
    {
        movementBounds.Clear();
        penBounds.Clear();

        // Setup Movement Areas
        foreach (var areaObj in movementAreas)
        {
            if (areaObj == null) continue;

            Collider col = areaObj.GetComponent<Collider>();
            if (col != null)
            {
                movementBounds.Add(col.bounds);
                Debug.Log($"[BoundsManager] ✅ Movement Area: {areaObj.name}");
            }
            else
            {
                Debug.LogWarning($"[BoundsManager] ⚠️ {areaObj.name} không có Collider!");
            }
        }

        // Setup Pen Areas
        foreach (var penObj in penAreas)
        {
            if (penObj == null) continue;

            Collider col = penObj.GetComponent<Collider>();
            if (col != null)
            {
                penBounds.Add(col.bounds);
                Debug.Log($"[BoundsManager] 🏠 Pen Area: {penObj.name}");
            }
            else
            {
                Debug.LogWarning($"[BoundsManager] ⚠️ {penObj.name} không có Collider!");
            }
        }

        Debug.Log($"[BoundsManager] Hoàn tất: {movementBounds.Count} Movement Areas + {penBounds.Count} Pen Areas");

        // ✅ Kiểm tra màu chuồng
        if (penColors.Count != penBounds.Count)
        {
            Debug.LogWarning($"[BoundsManager] ⚠️ Số màu chuồng ({penColors.Count}) không khớp với số chuồng ({penBounds.Count})!");
        }

        if (movementBounds.Count == 0)
        {
            Debug.LogError("[BoundsManager] ❌ KHÔNG CÓ Movement Area nào! Lợn sẽ không biết đi đâu!");
        }
    }

    /// <summary>
    /// Lấy điểm ngẫu nhiên trong vùng di chuyển (cho lợn hoang lang thang)
    /// </summary>
    public Vector3 GetRandomPointInMovementArea(float currentY = 0f)
    {
        if (movementBounds.Count == 0)
        {
            Debug.LogWarning("[BoundsManager] Không có Movement Area! Trả về Vector3.zero");
            return Vector3.zero;
        }

        Bounds randomBound = movementBounds[Random.Range(0, movementBounds.Count)];

        Vector3 randomPoint = new Vector3(
            Random.Range(randomBound.min.x, randomBound.max.x),
            currentY,
            Random.Range(randomBound.min.z, randomBound.max.z)
        );

        return randomPoint;
    }

    /// <summary>
    /// Lấy điểm ngẫu nhiên trong chuồng (cho lợn thuần phục)
    /// </summary>
    public Vector3 GetRandomPointInPenArea(float currentY = 0f)
    {
        if (penBounds.Count == 0)
        {
            Debug.LogWarning("[BoundsManager] Không có Pen Area! Trả về Vector3.zero");
            return Vector3.zero;
        }

        Bounds randomBound = penBounds[Random.Range(0, penBounds.Count)];

        Vector3 randomPoint = new Vector3(
            Random.Range(randomBound.min.x, randomBound.max.x),
            currentY,
            Random.Range(randomBound.min.z, randomBound.max.z)
        );

        return randomPoint;
    }

    /// <summary>
    /// Giới hạn vị trí lợn HOANG (chỉ trong Movement Areas)
    /// </summary>
    public Vector3 ClampToMovementArea(Vector3 position)
    {
        if (movementBounds.Count == 0)
            return position;

        float originalY = position.y;

        foreach (var bound in movementBounds)
        {
            if (bound.Contains(position))
            {
                position.y = originalY;
                return position;
            }
        }

        Bounds closestBound = movementBounds[0];
        float minDist = Vector3.Distance(position, closestBound.ClosestPoint(position));

        foreach (var bound in movementBounds)
        {
            float dist = Vector3.Distance(position, bound.ClosestPoint(position));
            if (dist < minDist)
            {
                minDist = dist;
                closestBound = bound;
            }
        }

        Vector3 clampedPos = closestBound.ClosestPoint(position);
        clampedPos.y = originalY;
        return clampedPos;
    }

    /// <summary>
    /// Giới hạn vị trí lợn THUẦN PHỤC (có thể vào cả Movement + Pen Areas)
    /// </summary>
    public Vector3 ClampToAllAreas(Vector3 position)
    {
        List<Bounds> allBounds = new List<Bounds>();
        allBounds.AddRange(movementBounds);
        allBounds.AddRange(penBounds);

        if (allBounds.Count == 0)
            return position;

        float originalY = position.y;

        foreach (var bound in allBounds)
        {
            if (bound.Contains(position))
            {
                position.y = originalY;
                return position;
            }
        }

        Bounds closestBound = allBounds[0];
        float minDist = Vector3.Distance(position, closestBound.ClosestPoint(position));

        foreach (var bound in allBounds)
        {
            float dist = Vector3.Distance(position, bound.ClosestPoint(position));
            if (dist < minDist)
            {
                minDist = dist;
                closestBound = bound;
            }
        }

        Vector3 clampedPos = closestBound.ClosestPoint(position);
        clampedPos.y = originalY;
        return clampedPos;
    }

    /// <summary>
    /// Giới hạn Player (có thể đi tự do cả 2 vùng)
    /// </summary>
    public Vector3 ClampPlayerPosition(Vector3 position)
    {
        return ClampToAllAreas(position);
    }

    /// <summary>
    /// ✅ Lấy index của chuồng mà vị trí đang nằm trong (-1 nếu không nằm trong chuồng nào)
    /// </summary>
    public int GetPenIndexAtPosition(Vector3 position)
    {
        for (int i = 0; i < penBounds.Count; i++)
        {
            if (penBounds[i].Contains(position))
                return i;
        }
        return -1;
    }

    /// <summary>
    /// ✅ Lấy màu của chuồng cụ thể
    /// </summary>
    public PigColor GetPenColor(int penIndex)
    {
        if (penIndex < 0 || penIndex >= penColors.Count)
        {
            Debug.LogWarning($"[BoundsManager] Pen index {penIndex} không có màu! Trả về Blue");
            return PigColor.Blue;
        }

        return penColors[penIndex];
    }

    /// <summary>
    /// ✅ Lấy tâm của chuồng cụ thể
    /// </summary>
    public Vector3 GetPenCenter(int penIndex)
    {
        if (penIndex < 0 || penIndex >= penBounds.Count)
        {
            Debug.LogWarning($"[BoundsManager] Pen index {penIndex} không hợp lệ!");
            return Vector3.zero;
        }

        return penBounds[penIndex].center;
    }

    /// <summary>
    /// ✅ Lấy điểm ngẫu nhiên trong chuồng cụ thể
    /// </summary>
    public Vector3 GetRandomPointInSpecificPen(int penIndex, float currentY = 0f)
    {
        if (penIndex < 0 || penIndex >= penBounds.Count)
        {
            Debug.LogWarning($"[BoundsManager] Pen index {penIndex} không hợp lệ!");
            return Vector3.zero;
        }

        Bounds targetBound = penBounds[penIndex];

        Vector3 randomPoint = new Vector3(
            Random.Range(targetBound.min.x, targetBound.max.x),
            currentY,
            Random.Range(targetBound.min.z, targetBound.max.z)
        );

        return randomPoint;
    }

    /// <summary>
    /// ✅ Giới hạn vị trí trong chuồng cụ thể
    /// </summary>
    public Vector3 ClampToSpecificPen(Vector3 position, int penIndex)
    {
        if (penIndex < 0 || penIndex >= penBounds.Count)
            return position;

        float originalY = position.y;
        Bounds targetBound = penBounds[penIndex];

        if (targetBound.Contains(position))
        {
            position.y = originalY;
            return position;
        }

        Vector3 clampedPos = targetBound.ClosestPoint(position);
        clampedPos.y = originalY;
        return clampedPos;
    }

    /// <summary>
    /// Kiểm tra xem vị trí có nằm trong Pen Area không
    /// </summary>
    public bool IsInsidePenArea(Vector3 position)
    {
        foreach (var bound in penBounds)
        {
            if (bound.Contains(position))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Kiểm tra xem vị trí có nằm trong Movement Area không
    /// </summary>
    public bool IsInsideMovementArea(Vector3 position)
    {
        foreach (var bound in movementBounds)
        {
            if (bound.Contains(position))
                return true;
        }
        return false;
    }

    void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;

        if (Application.isPlaying)
        {
            Gizmos.color = movementAreaColor;
            foreach (var bound in movementBounds)
            {
                Gizmos.DrawWireCube(bound.center, bound.size);
            }

            Gizmos.color = penAreaColor;
            foreach (var bound in penBounds)
            {
                Gizmos.DrawWireCube(bound.center, bound.size);
            }
        }
        else
        {
            Gizmos.color = movementAreaColor;
            foreach (var areaObj in movementAreas)
            {
                if (areaObj == null) continue;
                Collider col = areaObj.GetComponent<Collider>();
                if (col != null)
                {
                    Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);

#if UNITY_EDITOR
                    UnityEditor.Handles.Label(col.bounds.center, "🚶 Movement");
#endif
                }
            }

            Gizmos.color = penAreaColor;
            foreach (var penObj in penAreas)
            {
                if (penObj == null) continue;
                Collider col = penObj.GetComponent<Collider>();
                if (col != null)
                {
                    Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);

#if UNITY_EDITOR
                    UnityEditor.Handles.Label(col.bounds.center, "🏠 Pen");
#endif
                }
            }
        }
    }
}