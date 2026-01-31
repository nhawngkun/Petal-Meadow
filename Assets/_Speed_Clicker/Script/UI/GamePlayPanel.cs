using NabaGame.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayPanel :BaseUI
{
    public Button Avatar;
    public Button Pet;
    public TextMeshProUGUI Speed;
    public void SetInfor() 
    {
        Avatar.onClick.AddListener(UIManager.Instance.skinPanel.Show);
        Pet.onClick.AddListener(UIManager.Instance.petPanel.Show);
        UpdateText();
    }

    public void UpdateText() 
    {
        Speed.text = GameController.Instance.PlayerController.bonusSpeed.ToString();
    }

}
