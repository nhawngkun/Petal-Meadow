using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvas_PetalMeadow : MonoBehaviour
{
    [SerializeField] bool isDestroyOnClose = false;
    private CanvasGroup canvasGroup;

    protected virtual void Awake()
    {
        InitializeCanvasGroup();
    }

    /// <summary>
    /// ✅ Đảm bảo CanvasGroup luôn tồn tại
    /// </summary>
    void InitializeCanvasGroup()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            Debug.Log($"🔧 {gameObject.name}: Đã thêm CanvasGroup");
        }
    }

    public virtual void Setup()
    {
        // ✅ Đảm bảo CanvasGroup tồn tại khi Setup
        if (canvasGroup == null)
        {
            InitializeCanvasGroup();
        }
    }

    public virtual void Open()
    {
        // ✅ Kiểm tra lại CanvasGroup trước khi dùng
        if (canvasGroup == null)
        {
            InitializeCanvasGroup();
        }

        if (canvasGroup == null)
        {
            Debug.LogError($"❌ {gameObject.name}: Không thể lấy CanvasGroup!");
            return;
        }

        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        Debug.Log($"✅ {gameObject.name}: Open() - alpha={canvasGroup.alpha}");
    }

    public virtual void Close(float time)
    {
        Invoke(nameof(CloseDirectly), time);
    }

    public virtual void CloseDirectly()
    {
        if (canvasGroup == null)
        {
            InitializeCanvasGroup();
        }

        if (isDestroyOnClose)
        {
            Destroy(gameObject);
        }
        else
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }
    }
}