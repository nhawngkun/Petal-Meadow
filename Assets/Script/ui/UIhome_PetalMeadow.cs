using UnityEngine;
using UnityEngine.UI;

public class UIhome_PetalMeadow : UICanvas_PetalMeadow
{
    [Header("Buttons")]
    public Button playButton;
    public Button settingsButton;
    public Button howToPlayButton;

    void Start()
    {
        Setup();
    }

    public override void Setup()
    {
        base.Setup();
        SetupButtons();

        Debug.Log("🏠 UIHome: Setup hoàn tất");
    }

    private void SetupButtons()
    {
        if (playButton != null)
        {
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(OnPlay);
            Debug.Log("✅ UIHome: Đã gắn sự kiện cho nút Play");
        }
        else
        {
            Debug.LogError("❌ UIHome: playButton chưa được gán!");
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.RemoveAllListeners();
            settingsButton.onClick.AddListener(OnSettings);
        }

        if (howToPlayButton != null)
        {
            howToPlayButton.onClick.RemoveAllListeners();
            howToPlayButton.onClick.AddListener(OnHowToPlay);
        }
    }

    private void OnPlay()
    {
        if (UIManager_PetalMeadow.Instance == null)
        {
            return;
        }

        UIManager_PetalMeadow.Instance.EnableHome(false);
        UIManager_PetalMeadow.Instance.EnableLevelPanel(true);
    }

    private void OnSettings()
    {
        UIManager_PetalMeadow.Instance.EnableSettingPanel(true);
    }

    private void OnHowToPlay()
    {
        Debug.Log("📖 How To Play button clicked");

        // ✅ Đóng Home
        UIManager_PetalMeadow.Instance.EnableHome(false);

        // ✅ Lấy UITutorial và đánh dấu mở từ How To Play
        UITutorial_PetalMeadow tutorial = UIManager_PetalMeadow.Instance.GetUI<UITutorial_PetalMeadow>();
        if (tutorial != null)
        {
            tutorial.OpenFromHowToPlay();
        }

        // ✅ Mở Tutorial
        UIManager_PetalMeadow.Instance.EnableTutorial(true);
    }
}