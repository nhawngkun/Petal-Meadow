using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CookedPetData", menuName = "GameData/CookedPetData", order = 4)]
public class CookedPetData : SerializedScriptableObject
{
    public RawPetData rawPetData;
    [TableList]
    public List<AnimalSpawnInfor> Pet;

    public Dictionary<PetType,float> PetBonus;
    public Dictionary<PetType,PetRarity> PetRarity;
    [Button("Get Data")]
    public void GetData()
    {
        Pet = new List<AnimalSpawnInfor>();

        foreach (var item in rawPetData.rawPets)
        {
            AnimalSpawnInfor animalSpawnInfor = new AnimalSpawnInfor();
            animalSpawnInfor.animalType = item.PetType;
            animalSpawnInfor.rate = item.Rate;
            Pet.Add(animalSpawnInfor);
            PetBonus.Add(item.PetType, item.bonus);
            PetRarity.Add(item.PetType, item.PetRarity);
        }
    }
    public PetType GetRandomResult()
    {
        return  CaculateAnimal(Pet);
    }
    public PetType CaculateAnimal(List<AnimalSpawnInfor> infos)
    {
        int total = 0;
        for (int i = 0; i < infos.Count; i++)
            total += infos[i].rate;

        int rand = Random.Range(0, total);

        int sum = 0;
        for (int i = 0; i < infos.Count; i++)
        {
            sum += infos[i].rate;
            if (rand < sum)
                return infos[i].animalType;
        }

        return infos[0].animalType;
    }

}
[System.Serializable]
public class AnimalSpawnInfor
{
    public PetType animalType;
    public int rate;
}
