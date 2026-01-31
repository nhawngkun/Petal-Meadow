using NabaGame.UI;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PetPanel : BaseUI
{
    public List<PetSlot> petSlot;
    public int slotID;
    public Button EquipedBtn;
    public Button DeleteBtn;
    public Button EquipBest;
    public Image BGPreview;
    //public RawImage pvImage;
    public Button CloseBtn;
    public Image SelectBG;
    public TextMeshProUGUI select;
    public TextMeshProUGUI EquipedCount;
    public TextMeshProUGUI Quantity;

    public override void OnInAnimationStart()
    {
        base.OnInAnimationStart();
        SetInSlotInfor();
        OnSelect(petSlot[0]);
    }

    public void SetInfor()
    {
        EquipedBtn.onClick.AddListener(Equip);
        DeleteBtn.onClick.AddListener(Delete);
        CloseBtn.onClick.AddListener(Hide);
        OnSelect(petSlot[0]);
    }


    public void SetInSlotInfor() 
    {
        for (int i = 0; i < petSlot.Count; i++)
        {
            if(i < GameController.Instance.PetManager.PetList.Count) 
            {
                petSlot[i].gameObject.SetActive(true);
                petSlot[i].SetInfor(GameController.Instance.PetManager.PetList[i]);
             
            }
            else 
            {
                petSlot[i].gameObject.SetActive(false);
            }
        }
        EquipedCount.text = $"{GameController.Instance.PetManager.PetEquip.Count}/3";
        Quantity.text = $"{GameController.Instance.PetManager.PetList.Count}/20";
    }


    public void OnSelect(PetSlot slot) 
    {
        slotID = slot.ID;
        foreach (var pet in petSlot) 
        {
            if (pet.ID == slotID) 
            {
                pet.bounder.gameObject.SetActive(true);
            }
            else 
            {
                pet.bounder.gameObject.SetActive(false);
            }
        }
        if (slot.isEquiped) 
        {
            select.text = "UnSelect";
            SelectBG.sprite = GameManager.Instance.spriteCollection.button[ButtonType.Red];
        }
        else 
        {
            select.text = "Select";
            SelectBG.sprite = GameManager.Instance.spriteCollection.button[ButtonType.Green];
        }


    }    

    public void Delete() 
    {
        GameController.Instance.PetManager.RemovePet(slotID);
        OnSelect(petSlot[0]);
    }
    public void Equip() 
    {
        GameController.Instance.PetManager.ToggleEquip(slotID);
        foreach (var pet in petSlot) 
        {
            if(pet.ID == slotID) 
            {
                OnSelect(pet);
            }
        }
    }
}
