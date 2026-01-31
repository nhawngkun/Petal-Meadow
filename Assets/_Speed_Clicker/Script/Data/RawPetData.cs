using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

[System.Serializable]
public class RawPet 
{
	public PetType PetType;
	public PetRarity PetRarity;
	public float bonus;
	public int Rate;
}

public class RawPetData : ScriptableObject 
{
    [TableList]
    public List<RawPet> rawPets = new List<RawPet>();
}
