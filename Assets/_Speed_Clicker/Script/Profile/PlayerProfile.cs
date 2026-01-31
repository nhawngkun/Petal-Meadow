using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProfiles 
{
    public OutfitProfiles outfitProfile;

    public PlayerProfiles()
    {
        outfitProfile = new OutfitProfiles();
        Load();
    }
    public void Save()
    {
        outfitProfile.Save();
        PlayerPrefs.Save();
    }
    private void Load()
    {
        outfitProfile.Load();  
    }
}
