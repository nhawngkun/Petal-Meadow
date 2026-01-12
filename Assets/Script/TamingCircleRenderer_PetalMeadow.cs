using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Vẽ vòng tròn viền đứt quanh Player bằng nhiều LineRenderer
/// </summary>
public class TamingCircleRenderer_PetalMeadow : MonoBehaviour
{
    [Header("Vòng Tròn")]
    public float radius = 3.5f;           // Bán kính (lấy từ Player)
    public int numDashes = 20;            // Số nét đứt
    public float heightOffset = 0.1f;     // Độ cao so với mặt đất

    [Header("Viền Đứt")]
    [Range(0.1f, 0.9f)]
    public float dashRatio = 0.6f;        // Tỷ lệ nét/khoảng trống (0.6 = 60% nét, 40% trống)

    [Header("Màu Sắc & Độ Dày")]
    public Color circleColor = new Color(1f, 1f, 1f, 0.8f);
    public float lineWidth = 0.08f;

    [Header("Animation")]
    public bool rotateCircle = true;
    public float rotationSpeed = 20f;

    [Header("Hiển Thị")]
    public bool alwaysShow = true;
    public float fadeInSpeed = 5f;
    public float fadeOutSpeed = 3f;

    private List<LineRenderer> dashLines = new List<LineRenderer>();
    private Material lineMaterial;
    private float currentRotation = 0f;
    private float targetAlpha = 0f;
    private float currentAlpha = 0f;

    void Start()
    {
        CreateDashedCircle();
        if (alwaysShow)
        {
            currentAlpha = circleColor.a;
            targetAlpha = circleColor.a;
        }
    }

    void CreateDashedCircle()
    {
        // Xóa các line cũ nếu có
        foreach (var line in dashLines)
        {
            if (line != null) Destroy(line.gameObject);
        }
        dashLines.Clear();

        // Tạo material
        lineMaterial = new Material(Shader.Find("Sprites/Default"));

        // Tính góc cho mỗi đơn vị (nét + khoảng trống)
        float anglePerUnit = 360f / numDashes;
        float dashAngle = anglePerUnit * dashRatio;

        // Tạo từng nét đứt
        for (int i = 0; i < numDashes; i++)
        {
            GameObject dashObj = new GameObject($"Dash_{i}");
            dashObj.transform.SetParent(transform);
            dashObj.transform.localPosition = Vector3.zero;
            dashObj.transform.localRotation = Quaternion.identity;

            LineRenderer lr = dashObj.AddComponent<LineRenderer>();

            // Cấu hình LineRenderer
            lr.material = lineMaterial;
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            lr.positionCount = 10; // Số điểm để vẽ cung mượt
            lr.useWorldSpace = false;
            lr.numCornerVertices = 5;
            lr.numCapVertices = 5;
            lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lr.receiveShadows = false;

            // Tính vị trí các điểm của nét này
            float startAngle = i * anglePerUnit;
            float endAngle = startAngle + dashAngle;

            Vector3[] positions = new Vector3[lr.positionCount];
            for (int j = 0; j < lr.positionCount; j++)
            {
                float t = (float)j / (lr.positionCount - 1);
                float angle = Mathf.Lerp(startAngle, endAngle, t);
                float rad = angle * Mathf.Deg2Rad;

                positions[j] = new Vector3(
                    Mathf.Cos(rad) * radius,
                    heightOffset,
                    Mathf.Sin(rad) * radius
                );
            }

            lr.SetPositions(positions);
            lr.startColor = circleColor;
            lr.endColor = circleColor;

            dashLines.Add(lr);
        }

        Debug.Log($"✅ Đã tạo {numDashes} nét đứt cho vòng tròn");
    }

    void Update()
    {
        // ✅ Xoay CHỈ RIÊNG vòng tròn, KHÔNG ảnh hưởng Player
        if (rotateCircle)
        {
            currentRotation += rotationSpeed * Time.deltaTime;

            // Xoay từng nét đứt riêng lẻ
            foreach (var line in dashLines)
            {
                if (line != null)
                {
                    line.transform.localRotation = Quaternion.Euler(0, currentRotation, 0);
                }
            }
        }

        // Fade in/out
        targetAlpha = alwaysShow ? circleColor.a : 0f;

        // Smooth alpha transition
        if (currentAlpha < targetAlpha)
        {
            currentAlpha = Mathf.Min(currentAlpha + fadeInSpeed * Time.deltaTime, targetAlpha);
        }
        else if (currentAlpha > targetAlpha)
        {
            currentAlpha = Mathf.Max(currentAlpha - fadeOutSpeed * Time.deltaTime, targetAlpha);
        }

        // Update màu cho tất cả các nét
        Color newColor = circleColor;
        newColor.a = currentAlpha;

        foreach (var line in dashLines)
        {
            if (line != null)
            {
                line.startColor = newColor;
                line.endColor = newColor;
            }
        }

        if (lineMaterial != null)
        {
            lineMaterial.color = newColor;
        }
    }

    public void UpdateRadius(float newRadius)
    {
        radius = newRadius;
        CreateDashedCircle();
    }

    public void Show()
    {
        alwaysShow = true;
        targetAlpha = circleColor.a;
    }

    public void Hide()
    {
        alwaysShow = false;
        targetAlpha = 0f;
    }

    void OnValidate()
    {
        if (Application.isPlaying && dashLines.Count > 0)
        {
            CreateDashedCircle();
        }
    }

    void OnDestroy()
    {
        foreach (var line in dashLines)
        {
            if (line != null) Destroy(line.gameObject);
        }
        dashLines.Clear();

        if (lineMaterial != null)
        {
            Destroy(lineMaterial);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Preview trong Scene View
        Gizmos.color = new Color(circleColor.r, circleColor.g, circleColor.b, 0.3f);

        float anglePerUnit = 360f / numDashes;
        float dashAngle = anglePerUnit * dashRatio;

        for (int i = 0; i < numDashes; i++)
        {
            float startAngle = i * anglePerUnit;
            float endAngle = startAngle + dashAngle;

            Vector3 prevPoint = Vector3.zero;
            int steps = 10;

            for (int j = 0; j <= steps; j++)
            {
                float t = (float)j / steps;
                float angle = Mathf.Lerp(startAngle, endAngle, t);
                float rad = angle * Mathf.Deg2Rad;

                Vector3 point = transform.position + new Vector3(
                    Mathf.Cos(rad) * radius,
                    heightOffset,
                    Mathf.Sin(rad) * radius
                );

                if (j > 0)
                {
                    Gizmos.DrawLine(prevPoint, point);
                }
                prevPoint = point;
            }
        }
    }
}