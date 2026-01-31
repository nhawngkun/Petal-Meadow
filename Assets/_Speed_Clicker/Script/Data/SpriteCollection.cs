using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sprite Collection", menuName = "GameData/Sprite Collection", order = 3)]
public class SpriteCollection : SerializedScriptableObject
{
    public Dictionary<PetType, Sprite> pets;
    public Dictionary<CharacterType, Sprite> outfits;
    public Dictionary<PetRarity, Sprite> gacchaBox;
    public Dictionary<PetRarity, Sprite> listBox;
    public Dictionary<ButtonType, Sprite> button;
}
