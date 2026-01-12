using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelData", menuName = "PetalMeadow/Level Data")]
public class LevelData : ScriptableObject
{
    [System.Serializable]
    public class LevelInfo
    {
        public int levelID;
        public string levelName;
        public GameObject levelPrefab;
        public float timeLimit = 60f;
    }

    [Header("📋 Danh Sách Level")]
    public List<LevelInfo> levels = new List<LevelInfo>();

    public LevelInfo GetLevel(int id)
    {
        return levels.Find(l => l.levelID == id);
    }
}