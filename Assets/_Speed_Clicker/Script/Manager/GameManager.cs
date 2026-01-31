using NabaGame.Core.Runtime.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public ObjectPool objectPool;
    public RawOutfitData outfitData;
    public SpriteCollection spriteCollection;
    public CookedPetData cookedPetData;
    public PlayerProfiles PlayerProfile;
    public override void Init()
    {
        PlayerProfile = new PlayerProfiles();
    }

    private void Start()
    {
      
    }

    private void OnApplicationPause()
    {
        PlayerProfile.Save();
    }
    private void OnApplicationQuit()
    {
        PlayerProfile.Save();
    }
}
