using UnityEngine;

public class GameManager_PetalMeadow : MonoBehaviour
{
    public static GameManager_PetalMeadow Instance;

    [Header("📊 Thống Kê Lợn Trong Game")]
    public int totalBluePigs = 0;
    public int totalPinkPigs = 0;
    public int totalPurplePigs = 0;

    [Header("✅ Lợn Đã Vào Đúng Chuồng")]
    public int bluePigsInPen = 0;
    public int pinkPigsInPen = 0;
    public int purplePigsInPen = 0;

    [Header("🎯 Win Condition")]
    public bool gameWon = false;

    // Event khi có lợn vào chuồng đúng
    public delegate void OnPigEnteredCorrectPen(PigColor color);
    public event OnPigEnteredCorrectPen onPigEnteredCorrectPen;

    // Event khi win
    public delegate void OnGameWin();
    public event OnGameWin onGameWin;

    // ✅ Event khi đếm xong lợn (để UI cập nhật)
    public delegate void OnPigCountUpdated();
    public event OnPigCountUpdated onPigCountUpdated;

    void Awake()
    {
        // ✅ GLOBAL SINGLETON - DontDestroyOnLoad
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("✅ GameManager: Khởi tạo GLOBAL singleton");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ✅ Khởi tạo dữ liệu game cho level mới - GỌI TỪ LEVELMANAGER
    /// </summary>
    public void InitializeForNewLevel()
    {
        // Reset data
        bluePigsInPen = 0;
        pinkPigsInPen = 0;
        purplePigsInPen = 0;
        gameWon = false;

        // Đếm lợn
        CountAllPigs();

        Debug.Log($"📊 GameManager: Khởi tạo level mới - Blue={totalBluePigs}, Pink={totalPinkPigs}, Purple={totalPurplePigs}");

        // ✅ Trigger event để UI cập nhật
        onPigCountUpdated?.Invoke();
    }

    /// <summary>
    /// Đếm tất cả lợn trong scene
    /// </summary>
    void CountAllPigs()
    {
        PigBehavior_PetalMeadow[] allPigs = FindObjectsByType<PigBehavior_PetalMeadow>(FindObjectsSortMode.None);

        totalBluePigs = 0;
        totalPinkPigs = 0;
        totalPurplePigs = 0;

        foreach (var pig in allPigs)
        {
            switch (pig.pigColor)
            {
                case PigColor.Blue:
                    totalBluePigs++;
                    break;
                case PigColor.Pink:
                    totalPinkPigs++;
                    break;
                case PigColor.Purple:
                    totalPurplePigs++;
                    break;
            }
        }

        Debug.Log($"🐷 GameManager: Đếm lợn - Blue={totalBluePigs}, Pink={totalPinkPigs}, Purple={totalPurplePigs}");
    }

    /// <summary>
    /// Gọi khi lợn vào ĐÚNG chuồng
    /// </summary>
    public void RegisterPigInCorrectPen(PigColor color)
    {
        switch (color)
        {
            case PigColor.Blue:
                bluePigsInPen++;

                break;
            case PigColor.Pink:
                pinkPigsInPen++;

                break;
            case PigColor.Purple:
                purplePigsInPen++;

                break;
        }

        // Trigger event để UI update
        onPigEnteredCorrectPen?.Invoke(color);

        // Kiểm tra win
        CheckWinCondition();
    }

    /// <summary>
    /// Gọi khi lợn rời khỏi chuồng đúng (bị cứu ra)
    /// </summary>
    public void UnregisterPigFromCorrectPen(PigColor color)
    {
        switch (color)
        {
            case PigColor.Blue:
                bluePigsInPen = Mathf.Max(0, bluePigsInPen - 1);
                Debug.Log($"🔵 Lợn Xanh rời chuồng: {bluePigsInPen}/{totalBluePigs}");
                break;
            case PigColor.Pink:
                pinkPigsInPen = Mathf.Max(0, pinkPigsInPen - 1);
                Debug.Log($"🩷 Lợn Hồng rời chuồng: {pinkPigsInPen}/{totalPinkPigs}");
                break;
            case PigColor.Purple:
                purplePigsInPen = Mathf.Max(0, purplePigsInPen - 1);
                Debug.Log($"💜 Lợn Tím rời chuồng: {purplePigsInPen}/{totalPurplePigs}");
                break;
        }

        onPigEnteredCorrectPen?.Invoke(color);
        gameWon = false; // Reset win nếu lợn bị lấy ra
    }

    /// <summary>
    /// Kiểm tra điều kiện thắng
    /// </summary>
    void CheckWinCondition()
    {
        if (gameWon) return; // Đã thắng rồi thì không check nữa

        bool allBlueIn = (totalBluePigs == 0 || bluePigsInPen >= totalBluePigs);
        bool allPinkIn = (totalPinkPigs == 0 || pinkPigsInPen >= totalPinkPigs);
        bool allPurpleIn = (totalPurplePigs == 0 || purplePigsInPen >= totalPurplePigs);

        if (allBlueIn && allPinkIn && allPurpleIn)
        {
            gameWon = true;
            Debug.Log("🎉🎉🎉 THẮNG RỒI! Tất cả lợn đã vào đúng chuồng!");
            SoundManager_PetalMeadow.Instance.PlayVFXSound(0); // Sound thắng
            onGameWin?.Invoke();
        }
    }

    /// <summary>
    /// Lấy số lợn còn thiếu
    /// </summary>
    public int GetRemainingPigs(PigColor color)
    {
        switch (color)
        {
            case PigColor.Blue:
                return totalBluePigs - bluePigsInPen;
            case PigColor.Pink:
                return totalPinkPigs - pinkPigsInPen;
            case PigColor.Purple:
                return totalPurplePigs - purplePigsInPen;
            default:
                return 0;
        }
    }
}