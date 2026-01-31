using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PetSlot : MonoBehaviour
{
    public PetType type;
    public PetRarity rarity;
    public int ID;
    public bool isEquiped = false;
    public TextMeshProUGUI Bonus;
    public Image Icon;
    public Button btn;
    public Image Bg;
    public GameObject Tick;
    public GameObject bounder;
    public void SetInfor(PetInBag pet) 
    {
        type = pet.Type;
        ID = pet.ID;
        isEquiped = pet.isEquiped;
        Bonus.text = GameManager.Instance.cookedPetData.PetBonus
                    .TryGetValue(pet.Type, out float bonus)
                    ? bonus.ToString()
                    : "0";
        Icon.sprite = GameManager.Instance.spriteCollection.pets[type];
        rarity = GameManager.Instance.cookedPetData.PetRarity[type];
        Bg.sprite = GameManager.Instance.spriteCollection.listBox[rarity];
        Tick.SetActive(isEquiped);


    }

    public void OnClick() 
    {
        UIManager.Instance.petPanel.OnSelect(this);
    }
}
