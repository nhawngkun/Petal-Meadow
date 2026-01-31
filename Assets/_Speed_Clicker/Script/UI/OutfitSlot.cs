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
        var rawOutfit = GameManager.Instance.outfitData.rawOutfits
                .FirstOrDefault(outfit => outfit.CharacterType == characterType);
        UIManager.Instance.skinPanel.CheckRewardOutfitComplete(rawOutfit);

        //GameController.Instance._skinManager.Init();  // skinOnMap
        //if (openCongratsPanel)
        //{
        //    var outfit = GameManager.Instance.PlayerProfile.outfitProfile;
        //    UIManager.Instance.congratPanel.OpenCongratulationPanel(
        //        rawOutfit.CharacterType,
        //        delegate { ClickButtonSlot(false); },
        //        outfit.skinInfoRewards[rawOutfit.CharacterType],
        //        rawOutfit.Price);
        //}
        //else
        //{
        //    UIMainManager.Instance.congratPanel.ReportWatchAdsComplete();
        //}
    }
    public void CheckUnlock()
    {
        //var outfit = GameManager.Instance.PlayerProfile.outfitProfile;
        //isUnlock = outfit.skinUnlocked.Contains(characterType);
        //var rawOutfit = GameManager.Instance.outfitData.rawOutfits
        //    .FirstOrDefault(outfit => outfit.CharacterType == characterType);
        //if (!isUnlock && rawOutfit.UnlockType == UnlockType.Video)
        //{
        //    adsCount.text = GameManager.Instance.PlayerProfile.outfitProfile.skinInfoRewards[characterType]
        //        + "/" + rawOutfit.Price;
        //}
        //ads.gameObject.SetActive(!isUnlock && rawOutfit.UnlockType == UnlockType.Video);
        //SetSibling();

    }
    public void SetSibling()
    {
        //var outfit = GameManager.Instance.PlayerProfile.outfitProfile;
        //var rawOutfit = GameManager.Instance.outfitData.rawOutfits
        //        .FirstOrDefault(outfit => outfit.CharacterType == characterType);
        //isUnlock = outfit.skinUnlocked.Contains(rawOutfit.CharacterType);
        //if (GameManager.Instance.PlayerProfile.outfitProfile.skinEquip == rawOutfit.CharacterType)
        //{
        //    transform.SetAsFirstSibling();
        //}
        //else if (isUnlock)
        //{
        //    transform.SetSiblingIndex(1);
        //}
        //else
        //{
        //    transform.SetAsLastSibling();
        //}
    }
}


