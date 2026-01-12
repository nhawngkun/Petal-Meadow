using UnityEngine;
using UnityEngine.UI;

public class UITutorial_PetalMeadow : UICanvas_PetalMeadow
{
    [Header("🖼️ Tutorial Images - 2 ảnh cố định")]
    public GameObject tutorialPage1; // Kéo GameObject chứa ảnh 1
    public GameObject tutorialPage2; // Kéo GameObject chứa ảnh 2

    [Header("🎮 Navigation")]
    public Button leftButton;
    public Button rightButton;
    public Button closeButton;

    [Header("📄 Page Indicator (optional)")]
    public Text pageText;

    [Header("📖 How To Play Text")]
    public Text howToPlayText;

    private int currentPage = 0;
    private int maxPages = 2;

    // ✅ Flag để biết tutorial được mở từ đâu
    private bool isOpenedFromHowToPlay = false;

    public override void Setup()
    {
        base.Setup();

        // Setup buttons
        if (leftButton != null)
        {
            leftButton.onClick.RemoveAllListeners();
            leftButton.onClick.AddListener(OnPreviousPage);
        }

        if (rightButton != null)
        {
            rightButton.onClick.RemoveAllListeners();
            rightButton.onClick.AddListener(OnNextPage);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(OnClose);
        }

        // ✅ Setup How To Play text
        if (howToPlayText != null)
        {
            howToPlayText.text = 
                "🥕 DI CHUYỂN: Dùng joystick để điều khiển cà rốt\n" +
                "🐷 THU PHỤC: Giữ lợn trong vòng tròn đến khi loading đầy\n" +
                "🏠 DẪN VÀO CHUỒNG: Đưa lợn vào đúng chuồng theo màu (Xanh/Hồng/Tím)\n" +
                "⏱️ CHIẾN THẮNG: Đưa tất cả lợn vào đúng chuồng trước khi hết giờ";
        }

        // Start at page 0
        currentPage = 0;
        UpdateTutorialDisplay();
    }

    /// <summary>
    /// ✅ Mở tutorial từ How To Play button (không load level khi đóng)
    /// </summary>
    public void OpenFromHowToPlay()
    {
        isOpenedFromHowToPlay = true;
        Debug.Log("📖 Tutorial: Mở từ How To Play");
    }

    /// <summary>
    /// ✅ Mở tutorial từ chọn level (load level khi đóng)
    /// </summary>
    public void OpenFromLevelSelect()
    {
        isOpenedFromHowToPlay = false;
        Debug.Log("📖 Tutorial: Mở từ Level Select");
    }

    void UpdateTutorialDisplay()
    {
        // Hiển thị page tương ứng
        if (tutorialPage1 != null)
        {
            tutorialPage1.SetActive(currentPage == 0);
        }

        if (tutorialPage2 != null)
        {
            tutorialPage2.SetActive(currentPage == 1);
        }

        // Update page text
        if (pageText != null)
        {
            pageText.text = $"{currentPage + 1} / {maxPages}";
        }

        // Update button states
        if (leftButton != null)
        {
            leftButton.interactable = currentPage > 0;
        }

        if (rightButton != null)
        {
            rightButton.interactable = currentPage < maxPages - 1;
        }
    }

    void OnPreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdateTutorialDisplay();
        }
    }

    void OnNextPage()
    {
        if (currentPage < maxPages - 1)
        {
            currentPage++;
            UpdateTutorialDisplay();
        }
    }

    void OnClose()
    {
        Debug.Log($"📖 Tutorial: Đóng tutorial - isOpenedFromHowToPlay: {isOpenedFromHowToPlay}");

        // Đóng tutorial trước
        UIManager_PetalMeadow.Instance.EnableTutorial(false);

        // ✅ Kiểm tra xem tutorial được mở từ đâu
        if (isOpenedFromHowToPlay)
        {
            // Nếu mở từ How To Play → Về Home
            Debug.Log("📖 Tutorial: Về Home");
            UIManager_PetalMeadow.Instance.EnableHome(true);
        }
        else
        {
            // Nếu mở từ Level Select → Load level
            Debug.Log("📖 Tutorial: Load level");

            // ✅ Đánh dấu đã xem tutorial lần đầu (GLOBAL)
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.MarkTutorialSeenOnce();
                LevelManager.Instance.LoadLevel(LevelManager.Instance.currentLevelID);
            }
            else
            {
                Debug.LogError("❌ LevelManager không tồn tại!");
            }
        }

        // ✅ Reset flag
        isOpenedFromHowToPlay = false;
    }
}