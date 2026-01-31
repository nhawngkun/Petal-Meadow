using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PetManager : MonoBehaviour
{
    private const string PET_COUNT_KEY = "PET_COUNT";
    private const string PET_KEY = "PET_";
    public List<PetInBag> PetList = new();
    public int maxPet = 20;
    public int maxEquip = 3;
    public List<Pet> PetEquip;


    // giả sử có data rarity
    [Button] 
    public void GacchaRandom() 
    {
        AddPet(GameManager.Instance.cookedPetData.GetRandomResult());
    }
    public int GetRarity(PetType type)
    {
        if (GameManager.Instance.cookedPetData.PetRarity.TryGetValue(type, out PetRarity rarity))
            return (int)rarity;

        return 0; // fallback
    }

    public void InitEquip() 
    {
        for (int i = 0; i < PetList.Count; i++) 
        {
            if (PetList[i].isEquiped) 
            {
                PickNewTarget();
                Pet equipPet = GameManager.Instance.objectPool.InstantiatePet(PetList[i].Type, _currentTarget);
                equipPet.owner = GameController.Instance.PlayerController.transform;
                equipPet.id = PetList[i].ID;
                equipPet.type = PetList[i].Type;
                PetEquip.Add(equipPet);
            }
        }
    }
    public bool AddPet(PetType type)
    {
        if (PetList.Count >= maxPet)
        {
            Debug.Log("Pet bag full");
            return false;
        }
        int id = GetFreePetId();
        if (id == -1)
        {
            Debug.Log("Pet Bag Full (ID 0–20)");
            return false;
        }
        PetList.Add(new PetInBag
        {
            ID = id,
            Type = type,
            isEquiped = false
        });

        SortPetList();

        return true;
    }
    public bool RemovePet(int petId)
    {
        int index = PetList.FindIndex(p => p.ID == petId);
        if (index < 0) return false;
        PetInBag pet = PetList.Find(p => p.ID == petId);
        if (pet.isEquiped) 
        {
            Pet unEquipPet = PetEquip.FirstOrDefault(b => b.id == petId);
            GameManager.Instance.objectPool.DestroyPet(unEquipPet.type, unEquipPet.gameObject);
            PetEquip.Remove(unEquipPet);
        }
        PetList.RemoveAt(index);
        SortPetList();
        //UIManager.Instance.petPanel.SetInSlotInfor();
        return true;
    }
    public void SavePetBag()
    {
        PlayerPrefs.SetInt(PET_COUNT_KEY, PetList.Count);

        for (int i = 0; i < PetList.Count; i++)
        {
            var pet = PetList[i];

            PlayerPrefs.SetInt($"{PET_KEY}{i}_Type", (int)pet.Type);
            PlayerPrefs.SetInt($"{PET_KEY}{i}_ID", pet.ID);
            PlayerPrefs.SetInt($"{PET_KEY}{i}_Equip", pet.isEquiped ? 1 : 0);
        }

        PlayerPrefs.Save();
    }
    public void LoadPetBag()
    {
        PetList.Clear();
        int count = PlayerPrefs.GetInt(PET_COUNT_KEY, 0);
        for (int i = 0; i < count; i++)
        {
            PetInBag pet = new PetInBag
            {
                Type = (PetType)PlayerPrefs.GetInt($"{PET_KEY}{i}_Type"),
                ID = PlayerPrefs.GetInt($"{PET_KEY}{i}_ID"),
                isEquiped = PlayerPrefs.GetInt($"{PET_KEY}{i}_Equip") == 1
            };

            PetList.Add(pet);
        }
    }
    public int GetFreePetId()
    {
        for (int id = 0; id <= 20; id++)
        {
            bool used = false;

            for (int i = 0; i < PetList.Count; i++)
            {
                if (PetList[i].ID == id)
                {
                    used = true;
                    break;
                }
            }

            if (!used)
                return id;
        }

        return -1; // Full
    }
    public bool ToggleEquip(int petId)
    {
        PetInBag pet = PetList.Find(p => p.ID == petId);
        if (pet == null) return false;

        if (!pet.isEquiped)
        {
            int equipCount = PetList.Count(p => p.isEquiped);
            if (equipCount >= maxEquip)
            {
                Debug.Log("Equip slot full");
                return false;
            }
        }

        pet.isEquiped = !pet.isEquiped;


        if (pet.isEquiped) 
        {
            PickNewTarget();
            Pet equipPet = GameManager.Instance.objectPool.InstantiatePet(pet.Type, _currentTarget);
            equipPet.owner = GameController.Instance.PlayerController.transform;
            equipPet.id = petId;
            equipPet.type = pet.Type;
            PetEquip.Add(equipPet);
        }
        else 
        {
            Pet unEquipPet = PetEquip.FirstOrDefault(b => b.id == petId);
            GameManager.Instance.objectPool.DestroyPet(unEquipPet.type, unEquipPet.gameObject);
            PetEquip.Remove(unEquipPet);
        }

        SortPetList();
        return true;
    }
    public Vector3 _currentTarget;
    public float wanderRadius = 6.0f;
    public float minDistanceToOwner = 3.0f;
    void PickNewTarget()
    {
        for (int i = 0; i < 8; i++)
        {
            Vector2 random = UnityEngine.Random.insideUnitCircle * wanderRadius;

            if (random.magnitude < minDistanceToOwner)
                continue;

            _currentTarget = GameController.Instance.PlayerController.transform.position + new Vector3(random.x, 0f, random.y);
            return;
        }
    }

    void SortPetList()
    {
        PetList.Sort((a, b) =>
        {
            // 1️⃣ Equip trước
            int equipCompare = b.isEquiped.CompareTo(a.isEquiped);
            if (equipCompare != 0)
                return equipCompare;

            // 2️⃣ Rarity cao trước
            int rarityA = GetRarity(a.Type);
            int rarityB = GetRarity(b.Type);
            int rarityCompare = rarityB.CompareTo(rarityA);
            if (rarityCompare != 0)
                return rarityCompare;

            // 3️⃣ ID nhỏ trước (ổn định UI)
            return a.ID.CompareTo(b.ID);
        });
        UIManager.Instance.petPanel.SetInSlotInfor();
        SavePetBag();
    }
}
[Serializable]
public class PetInBag
{
    public PetType Type;
    public int ID;
    public bool isEquiped;
}
