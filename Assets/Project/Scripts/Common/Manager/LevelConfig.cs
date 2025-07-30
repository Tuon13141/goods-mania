using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "ScriptableObjects/LevelConfig", order = 0)]
public class LevelConfig : ScriptableObject
{
    [SerializeField] List<TextAsset> m_LevelDataFiles = new List<TextAsset>();
    [SerializeField] List<LevelData> m_LevelTestDataFiles = new List<LevelData>();

    public TextAsset GetLevel(int index)
    {
        return index >= 0 && index < m_LevelDataFiles.Count ? m_LevelDataFiles[index] : null;
    }

    public LevelData GetLevelTest(int index)
    {
        return index >= 0 && index < m_LevelTestDataFiles.Count ? m_LevelTestDataFiles[index] : null;
    }

    public int GetTotalLevel()
    {
        return m_LevelDataFiles.Count;
    }
}
