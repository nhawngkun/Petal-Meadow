using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIChooseLevel_PetalMeadow : UICanvas_PetalMeadow
{
    [Header("🔙 Navigation")]
    public Button backButton;

    [Header("📋 Level Buttons - Đã Setup Sẵn")]
    [Tooltip("Kéo tất cả các LevelButton_PetalMeadow vào đây")]
    public List<LevelButton_PetalMeadow> levelButtons = new List<LevelButton_PetalMeadow>();

    void Start()
    {
        Setup();
    }

    public override void Setup()
    {
        base.Setup();

        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(OnBackButton);
        }

        UpdateAllLevelButtons();
    }

    /// <summary>
    /// Cập nhật trạng thái tất cả buttons
    /// </summary>
    void UpdateAllLevelButtons()
    {
        if (levelButtons == null || levelButtons.Count == 0)
        {
            Debug.LogWarning("⚠️ Chưa gán LevelButtons vào UIChooseLevel!");
            return;
        }

        foreach (var levelBtn in levelButtons)
        {
            if (levelBtn != null)
            {
                levelBtn.UpdateButtonState();
            }
        }

        Debug.Log($"✅ Đã cập nhật {levelButtons.Count} level buttons");
    }

    void OnBackButton()
    {
        UIManager_PetalMeadow.Instance.EnableLevelPanel(false);
        UIManager_PetalMeadow.Instance.EnableHome(true);
    }

    /// <summary>
    /// Tự động tìm tất cả LevelButton trong children (nếu chưa gán)
    /// </summary>
    [ContextMenu("Auto Find Level Buttons")]
    public void AutoFindLevelButtons()
    {
        levelButtons.Clear();
        LevelButton_PetalMeadow[] buttons = GetComponentsInChildren<LevelButton_PetalMeadow>(true);
        levelButtons.AddRange(buttons);
        
        Debug.Log($"🔍 Đã tìm thấy {levelButtons.Count} level buttons");
        
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }
}