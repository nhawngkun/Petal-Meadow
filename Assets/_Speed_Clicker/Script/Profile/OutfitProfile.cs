using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

 public class OutfitProfiles
    {
        public List<CharacterType> skinUnlocked = new List<CharacterType>();
        public CharacterType skinEquip = CharacterType.Default;
        public Dictionary<CharacterType, int> skinInfoRewards;

        public int choosedSkin = 0;

        public void Save()
        {
            PlayerPrefs.SetInt(PlayerPrefKey.SKIN_EQUIP, (int)skinEquip);
            PlayerPrefs.SetInt(PlayerPrefKey.CHOOSED_SKIN, choosedSkin);
            SaveSkins();
            SaveDictionaryRewards();
        }
        public void Load()
        {
            skinEquip = (CharacterType)PlayerPrefs.GetInt(PlayerPrefKey.SKIN_EQUIP, 0);
            choosedSkin = PlayerPrefs.GetInt(PlayerPrefKey.CHOOSED_SKIN, 0);
            LoadSkins();
            LoadDictionaryRewards();
        }

        #region ListSkins
        void SaveSkins()
        {
            string skinUnlockedData = string.Join(",", skinUnlocked.Select(p => ((byte)p).ToString()));
            PlayerPrefs.SetString(PlayerPrefKey.SKIN_UNLOCKED, skinUnlockedData);
        }
        void LoadSkins()
        {
            skinUnlocked.Clear();
            string skinUnlockedData = PlayerPrefs.GetString(PlayerPrefKey.SKIN_UNLOCKED, "");
            if (!string.IsNullOrEmpty(skinUnlockedData))
            {
                string[] skinValues = skinUnlockedData.Split(',');
                foreach (string value in skinValues)
                {
                    if (byte.TryParse(value, out byte skinByte))
                    {
                        skinUnlocked.Add((CharacterType)skinByte);
                    }
                    else if (System.Enum.TryParse<CharacterType>(value, out var skinEnum))
                    {
                        skinUnlocked.Add(skinEnum);
                    }
                }
            }

            if (!skinUnlocked.Contains(CharacterType.Default))
                skinUnlocked.Add(CharacterType.Default);
            if (!skinUnlocked.Contains(CharacterType.GirlDefault))
                skinUnlocked.Add(CharacterType.GirlDefault);
        }
        #endregion
        #region Dictionary Rewards
        public void SaveDictionaryRewards()
        {
            string skinInfoRewardsData = string.Join(";", skinInfoRewards.Select(p => $"{(byte)p.Key},{p.Value}"));
            PlayerPrefs.SetString(PlayerPrefKey.SKIN_INFO_REWARDS, skinInfoRewardsData);
        }
        public void LoadDictionaryRewards()
        {
            //Outfit Info Rewards
            skinInfoRewards = new Dictionary<CharacterType, int>();
            string skinInfoRewardsData = PlayerPrefs.GetString(PlayerPrefKey.SKIN_INFO_REWARDS, "");
            if (!string.IsNullOrEmpty(skinInfoRewardsData))
            {
                string[] skinValues = skinInfoRewardsData.Split(';');
                foreach (string value in skinValues)
                {
                    string[] parts = value.Split(',');
                    if (parts.Length == 2 && byte.TryParse(parts[0], out byte skinByte) && int.TryParse(parts[1], out int reward))
                    {
                        skinInfoRewards[(CharacterType)skinByte] = reward;
                    }
                }
            }
            var characterTypes = System.Enum.GetValues(typeof(CharacterType));
            foreach (CharacterType type in characterTypes)
            {
                if (type != CharacterType.Default && type != CharacterType.GirlDefault && !skinInfoRewards.ContainsKey(type))
                {
                    skinInfoRewards[type] = 0;
                }
            }
        }
        #endregion
    }