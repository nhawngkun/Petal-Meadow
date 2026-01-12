using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("📋 Level Data")]
    public LevelData levelData;

    [Header("🎮 Trạng Thái")]
    public int currentLevelID = 1;
    private GameObject currentLevelInstance;

    [Header("📖 Tutorial Global")]
    private bool hasSeenTutorialOnce = false; // ✅ Flag global - chỉ hiển thị 1 lần

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadProgress();
        
        // ✅ Load flag tutorial global từ PlayerPrefs
        hasSeenTutorialOnce = PlayerPrefs.GetInt("HasSeenTutorialOnce", 0) == 1;
        Debug.Log($"📖 Tutorial Global: {(hasSeenTutorialOnce ? "Đã xem lần đầu" : "Chưa xem lần đầu")}");
    }

    /// <summary>
    /// Load level từ ID
    /// </summary>
    public void LoadLevel(int levelID)
    {
        currentLevelID = levelID;

        LevelData.LevelInfo levelInfo = levelData.GetLevel(levelID);
        if (levelInfo == null)
        {
            Debug.LogError($"❌ Không tìm thấy level {levelID}!");
            return;
        }

        Debug.Log($"🔄 Bắt đầu load level {levelID}: {levelInfo.levelName}");

        // ✅ XÓA level cũ trước (nếu có)
        if (currentLevelInstance != null)
        {
            Debug.Log("🗑️ Destroying old level instance...");
            Destroy(currentLevelInstance);
            currentLevelInstance = null;
            ResetLevelSingletons();
        }

        // ✅ ĐÓNG TẤT CẢ UI
        UIManager_PetalMeadow.Instance.CloseAll();
        Debug.Log("✅ Đã đóng tất cả UI");

        // Spawn level mới
        if (levelInfo.levelPrefab != null)
        {
            currentLevelInstance = Instantiate(levelInfo.levelPrefab);
            Debug.Log($"✅ Đã spawn level prefab: {levelInfo.levelName}");
        }
        else
        {
            Debug.LogError($"❌ Level {levelID} không có prefab!");
            return;
        }

        // ✅ SỬ DỤNG INVOKE thay vì Coroutine - Đợi level initialize
        Invoke(nameof(InitializeLevel), 0.1f);
    }

    /// <summary>
    /// ✅ Reset các Singleton TRONG LEVEL (trừ Camera - nó là true singleton)
    /// </summary>
    void ResetLevelSingletons()
    {
        Debug.Log("🔄 Resetting level singletons (trừ Camera)...");

        if (PenBoundsManager_PetalMeadow.Instance != null)
            PenBoundsManager_PetalMeadow.Instance = null;

        // ✅ KHÔNG reset Camera - nó là true singleton tồn tại across levels
        // if (CameraController_PetalMeadow.Instance != null)
        //     CameraController_PetalMeadow.Instance = null;

        if (CarrotPlayer_PetalMeadow.Instance != null)
            CarrotPlayer_PetalMeadow.Instance = null;
    }

    /// <summary>
    /// ✅ Khởi tạo level - GỌI SAU KHI SPAWN (qua Invoke)
    /// </summary>
    void InitializeLevel()
    {
        Debug.Log("🔄 InitializeLevel() được gọi");

        LevelData.LevelInfo levelInfo = levelData.GetLevel(currentLevelID);
        if (levelInfo == null)
        {
            Debug.LogError($"❌ Không tìm thấy level info cho ID {currentLevelID}!");
            return;
        }

        // 1. ✅ Kiểm tra GameManager
        if (GameManager_PetalMeadow.Instance == null)
        {
            Debug.LogError("❌ GameManager không tồn tại!");
            return;
        }

        // 2. ✅ Khởi tạo GameManager (đếm lợn)
        GameManager_PetalMeadow.Instance.InitializeForNewLevel();
        Debug.Log("✅ GameManager đã đếm lợn");

        // 3. ✅ Kiểm tra các components
        Debug.Log($"📊 PenBoundsManager: {(PenBoundsManager_PetalMeadow.Instance != null ? "✅" : "❌")}");
        Debug.Log($"📊 CameraController: {(CameraController_PetalMeadow.Instance != null ? "✅" : "❌")}");
        Debug.Log($"📊 CarrotPlayer: {(CarrotPlayer_PetalMeadow.Instance != null ? "✅" : "❌")}");

        // 4. ✅ Lấy UIGameplay
        UIGameplay_PetalMeadow gameplay = UIManager_PetalMeadow.Instance.GetUI<UIGameplay_PetalMeadow>();

        if (gameplay == null)
        {
            Debug.LogError("❌ KHÔNG TÌM THẤY UIGameplay!");
            Debug.LogError("➡️ Kiểm tra UIManager → uiCanvases → có UIGameplay_PetalMeadow không?");
            return;
        }

        // ✅ Set time limit TRƯỚC KHI Setup
        gameplay.gameTimeLimit = levelInfo.timeLimit;
        Debug.Log($"⏱️ Set time limit: {levelInfo.timeLimit}s");

        // 5. ✅ Reset camera
        if (CameraController_PetalMeadow.Instance != null)
        {
            CameraController_PetalMeadow.Instance.ResetToStart();
            Debug.Log("📷 Camera đã reset");
        }

        // 6. ✅ BẬT UIGameplay - Setup() sẽ được gọi tự động bên trong EnableGameplay()
        Debug.Log("🎮 Đang bật UIGameplay...");
        UIManager_PetalMeadow.Instance.EnableGameplay(true);

        // ✅ Đợi 1 frame để DOTween hoàn tất, sau đó kiểm tra
        Invoke(nameof(VerifyUIGameplayEnabled), 0.6f);

        Debug.Log("✅ Level initialization hoàn tất!");
    }

    /// <summary>
    /// ✅ Kiểm tra UIGameplay đã bật thành công chưa
    /// </summary>
    void VerifyUIGameplayEnabled()
    {
        UIGameplay_PetalMeadow gameplay = UIManager_PetalMeadow.Instance.GetUI<UIGameplay_PetalMeadow>();

        if (gameplay == null)
        {
            Debug.LogError("❌ UIGameplay không tồn tại!");
            return;
        }

        CanvasGroup cg = gameplay.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            Debug.Log($"📊 VERIFY UIGameplay - alpha: {cg.alpha}, interactable: {cg.interactable}, blocksRaycasts: {cg.blocksRaycasts}");

            if (cg.alpha >= 0.9f && cg.interactable && cg.blocksRaycasts)
            {
                Debug.Log("✅✅✅ UIGameplay ĐÃ BẬT THÀNH CÔNG!");
            }
            else
            {
                Debug.LogError("❌❌❌ UIGameplay CHƯA BẬT ĐÚNG!");

                // ✅ Force bật lại
                cg.alpha = 1f;
                cg.interactable = true;
                cg.blocksRaycasts = true;
                gameplay.gameObject.SetActive(true);

                Debug.Log("🔧 Đã FORCE bật UIGameplay!");
            }
        }
    }

    public bool IsLevelUnlocked(int levelID)
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        return levelID <= unlockedLevel;
    }

    public void UnlockNextLevel()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        int nextLevel = currentLevelID + 1;

        if (nextLevel > unlockedLevel)
        {
            PlayerPrefs.SetInt("UnlockedLevel", nextLevel);
            PlayerPrefs.Save();
            Debug.Log($"🔓 Đã mở khóa level {nextLevel}");
        }
    }

    public bool HasSeenTutorial(int levelID)
    {
        return PlayerPrefs.GetInt($"Tutorial_Level_{levelID}", 0) == 1;
    }

    public void MarkTutorialSeen(int levelID)
    {
        PlayerPrefs.SetInt($"Tutorial_Level_{levelID}", 1);
        PlayerPrefs.Save();
        Debug.Log($"✅ Đã đánh dấu tutorial level {levelID} đã xem");
    }

    /// <summary>
    /// ✅ Kiểm tra đã xem tutorial global lần đầu chưa
    /// </summary>
    public bool HasSeenTutorialOnce()
    {
        return hasSeenTutorialOnce;
    }

    /// <summary>
    /// ✅ Đánh dấu đã xem tutorial lần đầu
    /// </summary>
    public void MarkTutorialSeenOnce()
    {
        hasSeenTutorialOnce = true;
        PlayerPrefs.SetInt("HasSeenTutorialOnce", 1);
        PlayerPrefs.Save();
        Debug.Log("📖 Tutorial Global: Đã đánh dấu xem lần đầu");
    }

    void LoadProgress()
    {
        if (!PlayerPrefs.HasKey("UnlockedLevel"))
        {
            PlayerPrefs.SetInt("UnlockedLevel", 1);
            PlayerPrefs.Save();
        }
    }

    public void ResetAllProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("🔄 Đã reset toàn bộ progress!");
    }

    public void RestartCurrentLevel()
    {
        Debug.Log($"🔄 Restarting level {currentLevelID}...");

        UIManager_PetalMeadow.Instance.EnableWin(false);
        UIManager_PetalMeadow.Instance.EnableLoss(false);

        LoadLevel(currentLevelID);
    }

    public void LoadNextLevel()
    {
        int nextLevelID = currentLevelID + 1;
        LevelData.LevelInfo nextLevel = levelData.GetLevel(nextLevelID);

        if (nextLevel != null)
        {
            Debug.Log($"➡️ Loading next level: {nextLevelID}");

            UIManager_PetalMeadow.Instance.EnableWin(false);

            // ✅ Chỉ hiển thị tutorial nếu chưa xem lần đầu
            if (!HasSeenTutorialOnce())
            {
                currentLevelID = nextLevelID;
                UIManager_PetalMeadow.Instance.EnableTutorial(true);
            }
            else
            {
                LoadLevel(nextLevelID);
            }
        }
        else
        {
            Debug.Log("🎉 Đã hoàn thành tất cả level!");
        }
    }

    public void GoToHome()
    {
        Debug.Log("🏠 Going to Home...");

        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
            currentLevelInstance = null;
            ResetLevelSingletons();
        }

        UIManager_PetalMeadow.Instance.CloseAll();
        UIManager_PetalMeadow.Instance.EnableHome(true);
    }
}