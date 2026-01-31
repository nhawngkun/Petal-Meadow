using NabaGame.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : UIManagerSingleton<UIManager>
{
    public SkinPanel skinPanel;
    public GamePlayPanel gamePlayPanel;
    public PetOpenPanel petOpenPanel;
    public GacchaPetPanel gacchaPetPanel;
    public PetPanel petPanel;
    void OnValidate() 
    {
        skinPanel = GetComponentInChildren<SkinPanel>(true);
        gamePlayPanel = GetComponentInChildren<GamePlayPanel>(true);
        //petOpenPanel = GetComponentInChildren<PetOpenPanel>(true);
        petPanel = GetComponentInChildren<PetPanel>(true);
    }

    public void Start()
    {
        gamePlayPanel.SetInfor();
        skinPanel.SetInfor();
        //gacchaPetPanel.SetInfor();
        petPanel.SetInfor();
    }
   
}
