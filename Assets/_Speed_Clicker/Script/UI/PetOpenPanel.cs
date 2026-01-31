using NabaGame.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetOpenPanel :BaseUI
{

    public GameObject EggPanel;
    public GameObject PetPanel;
    public override void OnInAnimationFinish()
    {
        base.OnInAnimationFinish();
        ShowEgg();
    }


    public void ShowEgg()
    {
        EggPanel.SetActive(true);
        PetPanel.SetActive(false);
        PlayEggAnim();
    }

    public void PlayEggAnim() 
    {
    
    }

    public void ShowPet()
    {
        
    }

}
