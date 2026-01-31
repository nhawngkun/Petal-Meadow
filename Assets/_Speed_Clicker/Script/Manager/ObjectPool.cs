using NabaGame.Core.Runtime.Pool;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Object Pool", menuName = "GameData/Object Pool", order = 1)]
public class ObjectPool : ScriptableObject
{
    [FoldoutGroup("UISlot")] public OutfitSlot _SlotOutfits;
    public List<Pet> pets;
    #region Pool Slot
    public Dictionary<CharacterType, OutfitSlot> slotOutfitPool = new();
    #endregion
    public OutfitSlot GetSlotOutfit(CharacterType type, Transform parent)
    {
        if (slotOutfitPool.TryGetValue(type, out var slot))
        {
            if (!slot.gameObject.activeInHierarchy)
                slot.gameObject.SetActive(true);
            return slot;
        }

        var newSlot = Instantiate(_SlotOutfits, parent);
        slotOutfitPool[type] = newSlot;
        newSlot.gameObject.SetActive(true);
        return newSlot;
    }


    public Pet InstantiatePet(PetType type, Vector3 position)
    {
        return FastPoolManager.GetPool(pets[(int)type])
            .FastInstantiate<Pet>(position, Quaternion.identity);
    }

    public void DestroyPet(PetType type, GameObject destroyedGo)
    {
        FastPoolManager.GetPool(pets[(int)type]).FastDestroy(destroyedGo);
    }
}
