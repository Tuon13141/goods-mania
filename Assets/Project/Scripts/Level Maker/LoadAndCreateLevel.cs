using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadAndCreateLevel : MonoBehaviour
{
    [HorizontalLine(1, EColor.Blue)]
    [SerializeField] TextAsset m_levelDataTextAsset;

    [Button("Load Level By Level Asset")]
    public void LoadLevel()
    {
        LevelDataController.instance.LoadLevelFromTextAsset(m_levelDataTextAsset);
    }

    [Button("Save Level")]
    public void SaveLevel()
    {
        LevelNameInputWindow.ShowWindow((levelName) =>
        {
            LevelDataController.instance.CreateLevelDataFile(levelName);
        });
    }
}
