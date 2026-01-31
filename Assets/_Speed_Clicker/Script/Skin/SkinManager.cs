using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SkinManager : SerializedMonoBehaviour
{
    [SerializeField] private List<Skin> _skins = new();
    [SerializeField] private Dictionary<CharacterType, Skin> _skinDict;

    // =========================
    // Unity Life Cycle
    // =========================

    private void Awake()
    {
        //BuildSkinDictionary();
    }

    [Button]
    public void SetUP()
    {
        CollectSkinsInChildren();
        BuildSkinDictionary();
    }


    // =========================
    // Public API
    // =========================

    public Skin GetSkin(CharacterType characterType)
    {
        if (_skinDict == null)
            BuildSkinDictionary();

        if (_skinDict.TryGetValue(characterType, out var skin))
        {
            ActivateSkin(characterType);
            return skin;
        }
            

        Debug.LogError(
            $"[SkinManager] Skin with CharacterType '{characterType}' not found on {name}",
            this
        );
        return null;
    }
    public bool ActivateSkin(CharacterType characterType)
    {
        if (_skinDict == null)
            BuildSkinDictionary();

        bool found = false;

        foreach (var pair in _skinDict)
        {
            bool isTarget = pair.Key == characterType;
            pair.Value.gameObject.SetActive(isTarget);

            if (isTarget)
                found = true;
        }

        if (!found)
        {
            Debug.LogError(
                $"[SkinManager] Cannot activate skin. CharacterType '{characterType}' not found.",
                this
            );
        }

        return found;
    }
    public bool TryGetSkin(CharacterType characterType, out Skin skin)
    {
        if (_skinDict == null)
            BuildSkinDictionary();

        return _skinDict.TryGetValue(characterType, out skin);
    }

    // =========================
    // Internal Logic
    // =========================

    private void CollectSkinsInChildren()
    {
        _skins.Clear();
        GetComponentsInChildren(true, _skins);
    }

    private void BuildSkinDictionary()
    {
        if (_skinDict == null)
            _skinDict = new Dictionary<CharacterType, Skin>();
        else
            _skinDict.Clear();

        foreach (var skin in _skins)
        {
            if (skin == null)
                continue;

            var type = skin.CharacterType;

            if (_skinDict.ContainsKey(type))
            {
                Debug.LogError(
                    $"[SkinManager] Duplicate CharacterType '{type}' found.\n" +
                    $"Skin: {skin.name} already exists in dictionary.",
                    skin
                );
                continue;
            }

            _skinDict.Add(type, skin);
        }
    }
}