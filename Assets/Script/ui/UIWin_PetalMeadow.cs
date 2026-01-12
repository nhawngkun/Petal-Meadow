using UnityEngine;
using UnityEngine.UI;

public class UIWin_PetalMeadow : UICanvas_PetalMeadow
{
    [Header("🎮 Navigation Buttons")]
    public Button homeButton;
    public Button resetButton;
    public Button nextLevelButton;

    [Header("⭐ Stars & Score")]
    public Text scoreText;
    public GameObject[] stars; // 3 sao

    public override void Setup()
    {
        base.Setup();

        // Setup buttons
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

        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.RemoveAllListeners();
            nextLevelButton.onClick.AddListener(OnNextLevelButton);
        }

        // Unlock level tiếp theo
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.UnlockNextLevel();
        }

        // TODO: Tính số sao dựa trên thời gian
        UpdateStars(3);
    }

    void UpdateStars(int starCount)
    {
        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] != null)
            {
                stars[i].SetActive(i < starCount);
            }
        }
    }

    void OnHomeButton()
    {
        Debug.Log("🏠 Về Home từ Win");
        LevelManager.Instance.GoToHome();
    }

    void OnResetButton()
    {
        Debug.Log("🔄 Reset Level từ Win");
        LevelManager.Instance.RestartCurrentLevel();
    }

    void OnNextLevelButton()
    {
        Debug.Log("➡️ Next Level");
        LevelManager.Instance.LoadNextLevel();
    }
}