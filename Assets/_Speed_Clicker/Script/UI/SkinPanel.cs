using NabaGame.UI;
using UnityEngine;

public class SkinPanel : BaseUI
{
    public CharacterType CurrentSelected;
    public Transform contentOutfit;
    
    public void SetInfor() 
    {
        InitOutfitSlots();
    }

    void InitOutfitSlots()
    {
       
    }

    public void RefreshOutfitSlots()
    {
        var outfit = GameManager.Instance.PlayerProfile.outfitProfile;
        var pool = GameManager.Instance.objectPool;
        foreach (OutfitSlot slot in pool.slotOutfitPool.Values)
        {
            slot.CheckUnlock();
            SetFrameChooseOutfit(slot);
        }
    }

    public void SetFrameChooseOutfit(OutfitSlot slot)
    {
        if (slot.characterType == GameManager.Instance.PlayerProfile.outfitProfile.skinEquip)
        {
            slot.OnSelected();
        }
        else
        {
            slot.UnSelected();
        }
    }
    public void CheckRewardOutfitComplete(RawOutfit rawOutfit)
    {
        var outfitProfile = GameManager.Instance.PlayerProfile.outfitProfile;
        outfitProfile.skinInfoRewards[rawOutfit.CharacterType] += 1;
        if (outfitProfile.skinInfoRewards[rawOutfit.CharacterType] >= rawOutfit.Price)
        {
            GameController.Instance.PlayerController._playerAction.SetSkin(rawOutfit.CharacterType);
            GameManager.Instance.PlayerProfile.outfitProfile.skinUnlocked.Add(rawOutfit.CharacterType);
        }
        RefreshOutfitSlots();
        //UIMainManager.Instance.adsBonusRewardPanel.ResetShowInter();
    }
}
