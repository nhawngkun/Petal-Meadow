using UnityEngine;
using UnityEngine.UI;

public class UILoss_PetalMeadow : UICanvas_PetalMeadow
{
    [Header("🎮 Navigation Buttons")]
    public Button homeButton;
    public Button resetButton;

    [Header("📊 Stats")]
    public Text messageText;

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

        // Hiển thị thông báo
        if (messageText != null)
        {
            if (GameManager_PetalMeadow.Instance != null)
            {
                int remainingBlue = GameManager_PetalMeadow.Instance.GetRemainingPigs(PigColor.Blue);
                int remainingPink = GameManager_PetalMeadow.Instance.GetRemainingPigs(PigColor.Pink);
                int remainingPurple = GameManager_PetalMeadow.Instance.GetRemainingPigs(PigColor.Purple);

                messageText.text = $"Còn thiếu:\nBlue: {remainingBlue}\nPink: {remainingPink}\nPurple: {remainingPurple}";
            }
        }
    }

    void OnHomeButton()
    {
        Debug.Log("🏠 Về Home từ Loss");
        LevelManager.Instance.GoToHome();
    }

    void OnResetButton()
    {
        Debug.Log("🔄 Reset Level từ Loss");
        LevelManager.Instance.RestartCurrentLevel();
        GameManager_PetalMeadow.Instance.InitializeForNewLevel();

    }
}