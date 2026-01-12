using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIGameplay_PetalMeadow : UICanvas_PetalMeadow
{
    [Header("🕹️ Joystick")]
    public VariableJoystick_PetalMeadow joystick;

    [Header("🎮 Navigation Buttons")]
    public Button homeButton;
    public Button resetButton;

    [Header("⚙️ Settings Panel")]
    public Button settingsButton;
    public GameObject settingsPanel; // Panel chứa 3 nút
    public Button settingsHomeButton;
    public Button toggleMusicButton;
    public Button toggleSoundButton;

    [Header("🎵 Toggle Button Sprites")]
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;

    [Header("⏱️ Timer")]
    public Text timerText;
    public float gameTimeLimit = 60f;
    private float currentTime;
    private bool isGameActive = true;

    [Header("⚠️ Cảnh Báo Thời Gian")]
    public float warningTimeThreshold = 10f;
    public Color normalTimeColor = Color.white;
    public Color warningTimeColor = Color.red;

    [Header("🐷 UI Đếm Lợn - Blue")]
    public Image bluePigIcon;
    public Text bluePigCountText;
    public GameObject bluePigPanel;
    public GameObject blueCheckIcon;

    [Header("🐷 UI Đếm Lợn - Pink")]
    public Image pinkPigIcon;
    public Text pinkPigCountText;
    public GameObject pinkPigPanel;
    public GameObject pinkCheckIcon;

    [Header("🐷 UI Đếm Lợn - Purple")]
    public Image purplePigIcon;
    public Text purplePigCountText;
    public GameObject purplePigPanel;
    public GameObject purpleCheckIcon;

    [Header("🎬 Hiệu ứng Hoàn Thành")]
    public float shakeStrength = 20f;
    public float shakeDuration = 0.5f;
    public float checkIconScale = 1.2f;

    private bool isSubscribed = false;
    private bool isSettingsPanelOpen = false;

    void Awake()
    {
        Debug.Log("🎮 UIGameplay: Awake()");
        SetupButtons();

        // Ẩn settings panel ban đầu
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            isSettingsPanelOpen = false;
        }
    }

    void OnDestroy()
    {
        Debug.Log("🎮 UIGameplay: OnDestroy()");
        UnsubscribeEvents();
    }

    void SetupButtons()
    {
        if (homeButton != null)
        {
            homeButton.onClick.RemoveAllListeners();
            homeButton.onClick.AddListener(OnHomeButton);
        }

        if (resetButton != null)
        {
            resetButton.onClick.RemoveAllListeners();
            resetButton.onClick.AddListener(OnResetButton);
        }

        // ⚙️ Setup Settings Button
        if (settingsButton != null)
        {
            settingsButton.onClick.RemoveAllListeners();
            settingsButton.onClick.AddListener(OnSettingsButton);
        }

        // 🏠 Settings Home Button
        if (settingsHomeButton != null)
        {
            settingsHomeButton.onClick.RemoveAllListeners();
            settingsHomeButton.onClick.AddListener(OnSettingsHomeButton);
        }

        // 🎵 Toggle Music Button
        if (toggleMusicButton != null)
        {
            toggleMusicButton.onClick.RemoveAllListeners();
            toggleMusicButton.onClick.AddListener(OnToggleMusicButton);
            UpdateMusicButtonSprite();
        }

        // 🔊 Toggle Sound Button
        if (toggleSoundButton != null)
        {
            toggleSoundButton.onClick.RemoveAllListeners();
            toggleSoundButton.onClick.AddListener(OnToggleSoundButton);
            UpdateSoundButtonSprite();
        }
    }

    void SubscribeEvents()
    {
        if (isSubscribed) return;

        if (GameManager_PetalMeadow.Instance != null)
        {
            GameManager_PetalMeadow.Instance.onPigEnteredCorrectPen += UpdatePigCountUI;
            GameManager_PetalMeadow.Instance.onGameWin += OnGameWin;
            GameManager_PetalMeadow.Instance.onPigCountUpdated += UpdateAllPigCountUI;

            isSubscribed = true;
            Debug.Log("✅ UIGameplay: Đã subscribe events");
        }
        else
        {
            Debug.LogWarning("⚠️ UIGameplay: GameManager chưa tồn tại khi subscribe events");
        }
    }

    void UnsubscribeEvents()
    {
        if (!isSubscribed) return;

        if (GameManager_PetalMeadow.Instance != null)
        {
            GameManager_PetalMeadow.Instance.onPigEnteredCorrectPen -= UpdatePigCountUI;
            GameManager_PetalMeadow.Instance.onGameWin -= OnGameWin;
            GameManager_PetalMeadow.Instance.onPigCountUpdated -= UpdateAllPigCountUI;
        }

        isSubscribed = false;
        Debug.Log("🔄 UIGameplay: Đã unsubscribe events");
    }

    #region Settings Panel

    void OnSettingsButton()
    {
        Debug.Log("⚙️ Settings button clicked!");

        if (settingsPanel == null)
        {
            Debug.LogError("❌ Settings Panel chưa được gán!");
            return;
        }

        isSettingsPanelOpen = !isSettingsPanelOpen;
        settingsPanel.SetActive(isSettingsPanelOpen);

        if (isSettingsPanelOpen)
        {
            Debug.Log("✅ Mở Settings Panel");
            // Optional: Animation hiển thị
            settingsPanel.transform.localScale = Vector3.zero;
            settingsPanel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        }
        else
        {
            Debug.Log("❌ Đóng Settings Panel");
        }
    }

    void OnSettingsHomeButton()
    {
        Debug.Log("🏠 Settings Home Button clicked!");

        // Đóng settings panel trước
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            isSettingsPanelOpen = false;
        }

        // Về Home
        LevelManager.Instance.GoToHome();
    }

    void OnToggleMusicButton()
    {
        if (SoundManager_PetalMeadow.Instance == null)
        {
            Debug.LogWarning("⚠️ SoundManager không tồn tại!");
            return;
        }

        // Toggle music volume
        float currentVolume = SoundManager_PetalMeadow.Instance.GetMusicVolume();

        if (currentVolume > 0)
        {
            // Tắt nhạc
            SoundManager_PetalMeadow.Instance.SetMusicVolume(0f);
            Debug.Log("🔇 Đã tắt Background Music");
        }
        else
        {
            // Bật nhạc (volume mặc định 0.3f)
            SoundManager_PetalMeadow.Instance.SetMusicVolume(0.3f);
            Debug.Log("🔊 Đã bật Background Music");
        }

        UpdateMusicButtonSprite();
    }

    void OnToggleSoundButton()
    {
        if (SoundManager_PetalMeadow.Instance == null)
        {
            Debug.LogWarning("⚠️ SoundManager không tồn tại!");
            return;
        }

        // Toggle sound volume
        float currentVolume = SoundManager_PetalMeadow.Instance.GetSFXVolume();

        if (currentVolume > 0)
        {
            // Tắt sound
            SoundManager_PetalMeadow.Instance.SetSFXVolume(0f);
            Debug.Log("🔇 Đã tắt Sound Effects");
        }
        else
        {
            // Bật sound (volume mặc định 0.5f)
            SoundManager_PetalMeadow.Instance.SetSFXVolume(0.5f);
            Debug.Log("🔊 Đã bật Sound Effects");
        }

        UpdateSoundButtonSprite();
    }

    void UpdateMusicButtonSprite()
    {
        if (toggleMusicButton == null || SoundManager_PetalMeadow.Instance == null) return;

        Image btnImage = toggleMusicButton.GetComponent<Image>();
        if (btnImage == null) return;

        float currentVolume = SoundManager_PetalMeadow.Instance.GetMusicVolume();

        if (currentVolume > 0)
        {
            // Music ON
            btnImage.sprite = musicOnSprite;
        }
        else
        {
            // Music OFF
            btnImage.sprite = musicOffSprite;
        }
    }

    void UpdateSoundButtonSprite()
    {
        if (toggleSoundButton == null || SoundManager_PetalMeadow.Instance == null) return;

        Image btnImage = toggleSoundButton.GetComponent<Image>();
        if (btnImage == null) return;

        float currentVolume = SoundManager_PetalMeadow.Instance.GetSFXVolume();

        if (currentVolume > 0)
        {
            // Sound ON
            btnImage.sprite = soundOnSprite;
        }
        else
        {
            // Sound OFF
            btnImage.sprite = soundOffSprite;
        }
    }

    #endregion

    /// <summary>
    /// ✅ Reset hoàn toàn UI pig panels
    /// </summary>
    void ResetPigPanels()
    {
        Debug.Log("🔄 UIGameplay: ResetPigPanels()");

        // Kill tất cả animations trước
        if (bluePigPanel != null) bluePigPanel.transform.DOKill();
        if (pinkPigPanel != null) pinkPigPanel.transform.DOKill();
        if (purplePigPanel != null) purplePigPanel.transform.DOKill();

        // Reset check icons
        if (blueCheckIcon != null)
        {
            blueCheckIcon.SetActive(false);
            blueCheckIcon.transform.localScale = Vector3.one;
        }
        if (pinkCheckIcon != null)
        {
            pinkCheckIcon.SetActive(false);
            pinkCheckIcon.transform.localScale = Vector3.one;
        }
        if (purpleCheckIcon != null)
        {
            purpleCheckIcon.SetActive(false);
            purpleCheckIcon.transform.localScale = Vector3.one;
        }

        // Hiện lại count texts
        if (bluePigCountText != null) bluePigCountText.gameObject.SetActive(true);
        if (pinkPigCountText != null) pinkPigCountText.gameObject.SetActive(true);
        if (purplePigCountText != null) purplePigCountText.gameObject.SetActive(true);

        // Ẩn tất cả panels ban đầu
        if (bluePigPanel != null) bluePigPanel.SetActive(false);
        if (pinkPigPanel != null) pinkPigPanel.SetActive(false);
        if (purplePigPanel != null) purplePigPanel.SetActive(false);

        Debug.Log("✅ UIGameplay: Đã reset pig panels");
    }

    void InitializeTimer()
    {
        Debug.Log($"⏱️ UIGameplay: InitializeTimer() - {gameTimeLimit}s");

        currentTime = gameTimeLimit;
        isGameActive = true;

        if (timerText != null)
        {
            timerText.color = normalTimeColor;
            timerText.transform.localScale = Vector3.one;
        }

        UpdateTimerUI();
    }

    void Update()
    {
        if (!isGameActive) return;

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            if (currentTime < 0)
            {
                currentTime = 0;
            }

            UpdateTimerUI();

            // Cảnh báo khi sắp hết thời gian
            if (currentTime <= warningTimeThreshold && timerText != null)
            {
                timerText.color = warningTimeColor;

                if (currentTime % 1f < 0.5f)
                {
                    timerText.transform.localScale = Vector3.one * 1.1f;
                }
                else
                {
                    timerText.transform.localScale = Vector3.one;
                }
            }

            // Hết thời gian - THUA
            if (currentTime <= 0 && GameManager_PetalMeadow.Instance != null && !GameManager_PetalMeadow.Instance.gameWon)
            {
                OnTimeUp();
            }
        }
    }

    void UpdateTimerUI()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void OnTimeUp()
    {
        isGameActive = false;

        Debug.Log("⏱ HẾT THỜI GIAN - THUA!");
        SoundManager_PetalMeadow.Instance.PlayVFXSound(2); // Sound thua

        if (timerText != null)
        {
            timerText.transform.DOShakePosition(1f, 30f, 20, 90, false, true);
        }

        if (CameraController_PetalMeadow.Instance != null)
        {
            CameraController_PetalMeadow.Instance.OnLoss();
        }
    }

    void OnGameWin()
    {
        isGameActive = false;

        Debug.Log("🎉 CHIẾN THẮNG!");
        SoundManager_PetalMeadow.Instance.PlayVFXSound(0); 

        if (timerText != null)
        {
            timerText.color = Color.green;
            timerText.transform.DOPunchScale(Vector3.one * 0.3f, 0.5f, 5, 0.5f);
        }
    }

    void UpdatePigCountUI(PigColor color)
    {
        UpdateAllPigCountUI();
        CheckCompletedColor(color);
    }

    void UpdateAllPigCountUI()
    {
        if (GameManager_PetalMeadow.Instance == null)
        {
            
            return;
        }

        // ✅ UPDATE BLUE
        if (GameManager_PetalMeadow.Instance.totalBluePigs > 0)
        {
            if (bluePigPanel != null) bluePigPanel.SetActive(true);
            if (bluePigCountText != null)
            {
                bluePigCountText.gameObject.SetActive(true);
                bluePigCountText.text = $"{GameManager_PetalMeadow.Instance.bluePigsInPen}/{GameManager_PetalMeadow.Instance.totalBluePigs}";
            }
        }
        else
        {
            if (bluePigPanel != null) bluePigPanel.SetActive(false);
        }

        // ✅ UPDATE PINK
        if (GameManager_PetalMeadow.Instance.totalPinkPigs > 0)
        {
            if (pinkPigPanel != null) pinkPigPanel.SetActive(true);
            if (pinkPigCountText != null)
            {
                pinkPigCountText.gameObject.SetActive(true);
                pinkPigCountText.text = $"{GameManager_PetalMeadow.Instance.pinkPigsInPen}/{GameManager_PetalMeadow.Instance.totalPinkPigs}";
            }
        }
        else
        {
            if (pinkPigPanel != null) pinkPigPanel.SetActive(false);
        }

        // ✅ UPDATE PURPLE
        if (GameManager_PetalMeadow.Instance.totalPurplePigs > 0)
        {
            if (purplePigPanel != null) purplePigPanel.SetActive(true);
            if (purplePigCountText != null)
            {
                purplePigCountText.gameObject.SetActive(true);
                purplePigCountText.text = $"{GameManager_PetalMeadow.Instance.purplePigsInPen}/{GameManager_PetalMeadow.Instance.totalPurplePigs}";
            }
        }
        else
        {
            if (purplePigPanel != null) purplePigPanel.SetActive(false);
        }

        Debug.Log($"📊 UIGameplay Updated: Blue={GameManager_PetalMeadow.Instance.bluePigsInPen}/{GameManager_PetalMeadow.Instance.totalBluePigs}, Pink={GameManager_PetalMeadow.Instance.pinkPigsInPen}/{GameManager_PetalMeadow.Instance.totalPinkPigs}, Purple={GameManager_PetalMeadow.Instance.purplePigsInPen}/{GameManager_PetalMeadow.Instance.totalPurplePigs}");
    }

    void CheckCompletedColor(PigColor color)
    {
        if (GameManager_PetalMeadow.Instance == null) return;

        bool isCompleted = false;
        GameObject targetPanel = null;
        Text targetText = null;
        GameObject targetCheckIcon = null;

        switch (color)
        {
            case PigColor.Blue:
                isCompleted = GameManager_PetalMeadow.Instance.bluePigsInPen >= GameManager_PetalMeadow.Instance.totalBluePigs;
                targetPanel = bluePigPanel;
                targetText = bluePigCountText;
                targetCheckIcon = blueCheckIcon;
                break;

            case PigColor.Pink:
                isCompleted = GameManager_PetalMeadow.Instance.pinkPigsInPen >= GameManager_PetalMeadow.Instance.totalPinkPigs;
                targetPanel = pinkPigPanel;
                targetText = pinkPigCountText;
                targetCheckIcon = pinkCheckIcon;
                break;

            case PigColor.Purple:
                isCompleted = GameManager_PetalMeadow.Instance.purplePigsInPen >= GameManager_PetalMeadow.Instance.totalPurplePigs;
                targetPanel = purplePigPanel;
                targetText = purplePigCountText;
                targetCheckIcon = purpleCheckIcon;
                break;
        }

        if (isCompleted && targetPanel != null)
        {
            PlayCompletionEffect(targetPanel, targetText, targetCheckIcon);
        }
    }

    void PlayCompletionEffect(GameObject panel, Text countText, GameObject checkIcon)
    {
        if (panel == null) return;

        panel.transform.DOKill();
        if (checkIcon != null) checkIcon.transform.DOKill();

        Sequence seq = DOTween.Sequence();

        seq.Append(panel.transform.DOShakePosition(shakeDuration, shakeStrength, 10, 90, false, true));

        seq.AppendCallback(() =>
        {
            if (countText != null) countText.gameObject.SetActive(false);
        });

        if (checkIcon != null)
        {
            seq.AppendCallback(() =>
            {
                checkIcon.SetActive(true);
                checkIcon.transform.localScale = Vector3.zero;
            });

            seq.Append(checkIcon.transform.DOScale(checkIconScale, 0.3f).SetEase(Ease.OutBack));
            seq.Append(checkIcon.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 5, 0.5f));
        }
    }

    void OnHomeButton()
    {
        Debug.Log("🏠 Về Home");
        LevelManager.Instance.GoToHome();
    }

    void OnResetButton()
    {
        Debug.Log("🔄 Reset Level");
        LevelManager.Instance.RestartCurrentLevel();
    }

    /// <summary>
    /// ✅ Override Setup - Gọi khi UI được bật
    /// </summary>
    public override void Setup()
    {
        base.Setup();

       

        // 1. Unsubscribe events cũ (nếu có)
        UnsubscribeEvents();

        // 2. Reset pig panels
        ResetPigPanels();

        // 3. Initialize timer
        InitializeTimer();

        // 4. Subscribe events MỚI
        SubscribeEvents();

        // 5. Update UI ngay lập tức
        UpdateAllPigCountUI();

        // 6. Đảm bảo settings panel đóng khi bắt đầu
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            isSettingsPanelOpen = false;
        }

        // 7. Update button sprites
        UpdateMusicButtonSprite();
        UpdateSoundButtonSprite();

        
    }

    /// <summary>
    /// ✅ Override Open - Đảm bảo UI được update khi mở
    /// </summary>
    public override void Open()
    {
       
        base.Open();

        // Update UI sau khi mở
        UpdateAllPigCountUI();
    }
}