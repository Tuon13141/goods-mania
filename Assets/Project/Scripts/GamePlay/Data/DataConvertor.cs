using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataConvertor
{
    public static LevelData ConvertTextAssetToLevelData(TextAsset textAsset)
    {
        if (textAsset == null)
        {
            Debug.LogError("TextAsset is null.");
            return null;
        }

        LevelData levelData = JsonUtility.FromJson<LevelData>(textAsset.text);
        return levelData;
    }
}
