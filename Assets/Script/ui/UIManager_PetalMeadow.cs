using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DG.Tweening;

public class UIManager_PetalMeadow : Singleton_PetalMeadow<UIManager_PetalMeadow>
{
    [SerializeField] private List<UICanvas_PetalMeadow> uiCanvases;
    [SerializeField] private CanvasGroup tutorial, howToPlay, home, winPopUp, settingPanel, goButton, Loss, gamplayPanel, levelPanel, shopPanel;
    private float time = 0.5f;

    public Transform _effects;
    private bool isPaused = false;

    public override void Awake()
    {
        base.Awake();
        InitializeUICanvases();
    }

    void Start()
    {
        UIManager_PetalMeadow.Instance.OpenUI<UIhome_PetalMeadow>();
    }

    private UICanvas_PetalMeadow GetUICanvasFromCanvasGroup(CanvasGroup cg)
    {
        if (cg == null) return null;
        return cg.GetComponent<UICanvas_PetalMeadow>();
    }

    public void EnableTutorial(bool enable)
    {
        if (enable)
        {
            UICanvas_PetalMeadow canvas = GetUICanvasFromCanvasGroup(tutorial);
            if (canvas != null) canvas.Setup();
            tutorial.DOFade(1f, time).Play();
        }
        else
        {
            tutorial.DOFade(0f, time).Play();
        }
        tutorial.blocksRaycasts = enable;
        tutorial.interactable = enable;
    }

    public void EnableLoss(bool enable)
    {
        if (enable)
        {
            UICanvas_PetalMeadow canvas = GetUICanvasFromCanvasGroup(Loss);
            if (canvas != null) canvas.Setup();
            Loss.DOFade(1f, time).Play();
        }
        else
        {
            Loss.DOFade(0f, time).Play();
        }
        Loss.blocksRaycasts = enable;
        Loss.interactable = enable;
    }

    public void EnableHowToPlay(bool enable)
    {
        if (enable)
        {
            UICanvas_PetalMeadow canvas = GetUICanvasFromCanvasGroup(howToPlay);
            if (canvas != null) canvas.Setup();
            howToPlay.DOFade(1f, time).Play();
        }
        else
        {
            howToPlay.DOFade(0f, time).Play();
        }
        howToPlay.blocksRaycasts = enable;
        howToPlay.interactable = enable;
    }

    public void EnableHome(bool enable)
    {
        if (enable)
        {
            UICanvas_PetalMeadow canvas = GetUICanvasFromCanvasGroup(home);
            if (canvas != null) canvas.Setup();
            home.DOFade(1f, time).Play();
        }
        else
        {
            home.DOFade(0f, time).Play();
        }
        home.blocksRaycasts = enable;
        home.interactable = enable;
    }

    public void EnableWin(bool enable)
    {
        if (enable)
        {
            UICanvas_PetalMeadow canvas = GetUICanvasFromCanvasGroup(winPopUp);
            if (canvas != null)
            {
                canvas.Setup();
                canvas.Open();
            }
            winPopUp.alpha = 0f;
            winPopUp.DOFade(1f, time).SetDelay(0.5f);
        }
        else
        {
            winPopUp.DOFade(0f, time).Play();
        }
        winPopUp.blocksRaycasts = enable;
        winPopUp.interactable = enable;
    }

    public void EnableSettingPanel(bool enable)
    {
        if (enable)
        {
            UICanvas_PetalMeadow canvas = GetUICanvasFromCanvasGroup(settingPanel);
            if (canvas != null) canvas.Setup();
            settingPanel.DOFade(1f, time).Play();
        }
        else
        {
            settingPanel.DOFade(0f, time).Play();
        }
        settingPanel.blocksRaycasts = enable;
        settingPanel.interactable = enable;
    }

    public void EnableGo(bool enable)
    {
        if (enable) goButton.DOFade(1f, time).Play();
        else goButton.DOFade(0f, time).Play();
        goButton.blocksRaycasts = enable;
    }

    /// <summary>
    /// ✅ FIXED: Đảm bảo CanvasGroup được khởi tạo trước khi sử dụng
    /// </summary>
    public void EnableGameplay(bool enable)
    {
        // ✅ Kiểm tra gamplayPanel có null không
        if (gamplayPanel == null)
        {
            Debug.LogError("❌ gamplayPanel CanvasGroup chưa được gán trong UIManager!");
            return;
        }

        UICanvas_PetalMeadow canvas = GetUICanvasFromCanvasGroup(gamplayPanel);

        if (canvas == null)
        {
            Debug.LogError("❌ Không tìm thấy UICanvas_PetalMeadow component trên gamplayPanel!");
            return;
        }

        if (enable)
        {
            // ✅ Đảm bảo GameObject active trước
            canvas.gameObject.SetActive(true);

            // ✅ Kiểm tra CanvasGroup đã có chưa
            if (gamplayPanel == null)
            {
                gamplayPanel = canvas.gameObject.AddComponent<CanvasGroup>();
                Debug.Log("🔧 Đã thêm CanvasGroup vào UIGameplay");
            }

            // ✅ Setup trước
            Debug.Log("🎮 UIManager: Gọi canvas.Setup()...");
            canvas.Setup();

            // ✅ Set alpha = 0 trước khi fade
            gamplayPanel.alpha = 0f;
            gamplayPanel.blocksRaycasts = true;
            gamplayPanel.interactable = true;

            // ✅ Fade in
            Debug.Log("🎮 UIManager: Đang fade in UIGameplay...");
            gamplayPanel.DOFade(1f, time).OnComplete(() =>
            {
                Debug.Log("✅ UIManager: UIGameplay fade hoàn tất!");
            });
        }
        else
        {
            gamplayPanel.DOFade(0f, time).OnComplete(() =>
            {
                gamplayPanel.blocksRaycasts = false;
                gamplayPanel.interactable = false;
            });
        }
    }

    public void EnableLevelPanel(bool enable)
    {
        if (enable)
        {
            UICanvas_PetalMeadow canvas = GetUICanvasFromCanvasGroup(levelPanel);
            if (canvas != null)
            {
                canvas.Setup();
                canvas.Open();
            }
            levelPanel.DOFade(1f, time).Play();
        }
        else
        {
            levelPanel.DOFade(0f, time).Play();
        }
        levelPanel.blocksRaycasts = enable;
        levelPanel.interactable = enable;
    }

    public void EnableShop(bool enable)
    {
        if (enable)
        {
            UICanvas_PetalMeadow canvas = GetUICanvasFromCanvasGroup(shopPanel);
            if (canvas != null) canvas.Setup();
            shopPanel.DOFade(1f, time).Play();
        }
        else
        {
            shopPanel.DOFade(0f, time).Play();
        }
        shopPanel.blocksRaycasts = enable;
        shopPanel.interactable = enable;
    }

    private void InitializeUICanvases()
    {
        foreach (var canvas in uiCanvases)
        {
            CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();
            }

            canvas.gameObject.SetActive(true);
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public T OpenUI<T>() where T : UICanvas_PetalMeadow
    {
        T canvas = GetUI<T>();
        if (canvas != null)
        {
            canvas.Setup();
            canvas.Open();
        }
        return canvas;
    }

    public void CloseUI<T>(float time) where T : UICanvas_PetalMeadow
    {
        T canvas = GetUI<T>();
        if (canvas != null)
        {
            canvas.Close(time);
        }
    }

    public void CloseUIDirectly<T>() where T : UICanvas_PetalMeadow
    {
        T canvas = GetUI<T>();
        if (canvas != null)
        {
            canvas.CloseDirectly();
        }
    }

    public bool IsUIOpened<T>() where T : UICanvas_PetalMeadow
    {
        T canvas = GetUI<T>();
        if (canvas == null) return false;

        CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
        return canvasGroup != null && canvasGroup.alpha > 0f;
    }

    public T GetUI<T>() where T : UICanvas_PetalMeadow
    {
        return uiCanvases.Find(c => c is T) as T;
    }

    public void CloseAll()
    {
        foreach (var canvas in uiCanvases)
        {
            CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
            if (canvasGroup != null && canvasGroup.alpha > 0f)
            {
                canvas.Close(0);
            }
        }
    }

    public void PauseGame()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}