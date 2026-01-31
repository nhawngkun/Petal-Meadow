using NabaGame.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : UIManagerSingleton<UIManager>
{
    public SkinPanel skinPanel;
    public GamePlayPanel gamePlayPanel;
    void OnValidate() 
    {
        skinPanel = GetComponentInChildren<SkinPanel>(true);
        gamePlayPanel = GetComponentInChildren<GamePlayPanel>(true);

    }

    public void Start()
    {
        gamePlayPanel.SetInfor();
        skinPanel.SetInfor();
    }
   
}
