using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

[System.Serializable]
public class RawOutfit 
{
	public CharacterType CharacterType;
	public UnlockType UnlockType;
	public int Price;
}

public class RawOutfitData : ScriptableObject 
{
    [TableList]
    public List<RawOutfit> rawOutfits = new List<RawOutfit>();
}
