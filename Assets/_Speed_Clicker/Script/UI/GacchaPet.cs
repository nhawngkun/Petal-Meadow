using JetBrains.Annotations;
using NabaGame.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GacchaPetPanel : BaseUI
{
    public Button GacchaBtn;
    public Button RewadBtn; 

    public void SetInfor() 
    {
        GacchaBtn.onClick.AddListener(OnGaccha);
    }
    public void OnGaccha() 
    {
        Hide();

    }
}
