//using BMH.Ads;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OutfitSlot : MonoBehaviour
{
    public CharacterType characterType;
    public GameObject ads;
    public GameObject cup;
    public TextMeshProUGUI adsCount;
    public Image Icon;
    public Image SelectImage;
    public TextMeshProUGUI select;
    public Button SelectBtn;
    [SerializeField] bool isUnlock;
    private void OnEnable()
    {
        //SelectBtn.onClick.RemoveAllListeners();
        //SelectBtn.onClick.AddListener(delegate { ClickButtonSlot(); });
    }
    public void SetInfoSlot()
    {
        //Icon.sprite = GameManager.Instance.spriteCollection.outfits[characterType];
        //CheckUnlock();
        //SetSibling();
    }

    public void OnSelected()
    {
        //select.text = "Selected";
        //SelectImage.sprite = GameManager.Instance.spriteCollection.button[ButtonType.Green];
    }
    public void UnSelected()
    {
        //select.text = "Select";
        //SelectImage.sprite = GameManager.Instance.spriteCollection.button[ButtonType.Orange];
    }

    public void ClickButtonSlot(bool openCongratsPanel = true)
    {
//        var outfit = GameManager.Instance.PlayerProfile.outfitProfile;
//        if (!isUnlock)
//        {
//#if UNITY_EDITOR
//            OnRewardComplete(openCongratsPanel);
//            return;
//#endif
//            AdManager.Instance.ShowRewardedVideo(() => { OnRewardComplete(openCongratsPanel); }, null,
//                "watch_ads_skin_" + characterType);

//        }
//        else if (outfit.skinEquip == characterType)
//        {
//            return;
//        }
//        else
//        {
//            //AudioManager.Instance.PlaySound(SoundType.buttonClick);
//            GameController.Instance.PlayerController._playerAction.SetSkin(characterType);
//            outfit.skinEquip = characterType;
//            UIManager.Instance.skinPanel.RefreshOutfitSlots();
//            SetSibling();
//        }
    }
    void OnRewardComplete(bool openCongratsPanel = true)
    {
        
    }
    public void CheckUnlock()
    {
        

    }
    public void SetSibling()
    {
        
    }
}


