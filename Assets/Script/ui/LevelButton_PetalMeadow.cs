using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gắn vào mỗi Button Level trong UIChooseLevel
/// </summary>
public class LevelButton_PetalMeadow : MonoBehaviour
{
    [Header("⚙️ Setup")]
    [Tooltip("ID của level này (1, 2, 3...)")]
    public int levelID = 1;

    [Header("🎨 UI Components")]
    public Image unlockImage;
    public Image lockImage;
    public TextMeshProUGUI levelText;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClick);
        }
    }

    void Start()
    {
        UpdateButtonState();
    }

    /// <summary>
    /// Cập nhật trạng thái unlock/lock
    /// </summary>
    public void UpdateButtonState()
    {
        if (LevelManager.Instance == null)
        {
            Debug.LogWarning("⚠️ LevelManager chưa có trong scene!");
            return;
        }

        bool isUnlocked = LevelManager.Instance.IsLevelUnlocked(levelID);

        // Hiển thị unlock/lock image
        if (unlockImage != null)
        {
            unlockImage.gameObject.SetActive(isUnlocked);
        }

        if (lockImage != null)
        {
            lockImage.gameObject.SetActive(!isUnlocked);
        }

        // Set text
        if (levelText != null)
        {
            levelText.text = levelID.ToString();
        }

        // Set button interactable
        if (button != null)
        {
            button.interactable = isUnlocked;
        }

        Debug.Log($"🎮 Level {levelID}: {(isUnlocked ? "Unlocked ✅" : "Locked 🔒")}");
    }

    /// <summary>
    /// Khi click vào button
    /// </summary>
    void OnButtonClick()
    {
        if (LevelManager.Instance == null)
        {
            Debug.LogError("❌ LevelManager không tồn tại!");
            return;
        }

        Debug.Log($"🎯 Chọn Level {levelID}");

        LevelManager.Instance.currentLevelID = levelID;

        // ✅ ĐÓNG Level Panel trước
        UIManager_PetalMeadow.Instance.EnableLevelPanel(false);

        // ✅ Kiểm tra đã xem tutorial lần đầu chưa (GLOBAL - không phải per-level)
        if (LevelManager.Instance.HasSeenTutorialOnce())
        {
            // ✅ Đã xem tutorial → Load level ngay
            LevelManager.Instance.LoadLevel(levelID);
        }
        else
        {
            // ✅ Lần đầu tiên → Hiện tutorial
            UITutorial_PetalMeadow tutorial = UIManager_PetalMeadow.Instance.GetUI<UITutorial_PetalMeadow>();
            if (tutorial != null)
            {
                tutorial.OpenFromLevelSelect(); // ✅ Đánh dấu mở từ Level Select
            }

            UIManager_PetalMeadow.Instance.EnableTutorial(true);
        }
    }

    /// <summary>
    /// Gọi từ Editor hoặc code khác để test
    /// </summary>
    [ContextMenu("Test Update State")]
    public void TestUpdateState()
    {
        UpdateButtonState();
    }
}