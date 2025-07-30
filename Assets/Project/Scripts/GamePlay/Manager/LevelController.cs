using Kit.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : Singleton<LevelController>
{
    [Header("Data")]
    [SerializeField] LevelConfig m_LevelConfig;
    public LevelConfig levelConfig => m_LevelConfig;
    [SerializeField] ColorManager m_ColorManager;
    [SerializeField] PrefabManager m_PrefabManager;

    LevelData levelData;

    [Header("Managers")]

    [SerializeField] InputManager m_InputManager;
    public InputManager inputManager => m_InputManager;

    [SerializeField] ShelfManager m_ShelfManager;
    public ShelfManager shelfManager => m_ShelfManager;

    [SerializeField] SlotManager m_SlotManager;
    public SlotManager slotManager => m_SlotManager;

    [SerializeField] OrderManager m_OrderManager;
    public OrderManager orderManager => m_OrderManager;

    [SerializeField] CameraController m_CameraController;
    public CameraController cameraController => m_CameraController;

    public void SetUp(int levelIndex)
    {
        Debug.Log("SetUp Level: " + levelIndex);    
        levelData = DataConvertor.ConvertTextAssetToLevelData(m_LevelConfig.GetLevel(levelIndex));
        cameraController.SetUp();
        shelfManager.SetUp(levelData.shelfDatas);
        slotManager.SetUp();
        orderManager.SetUp(levelData.orderDatas);

    }

    public void OnReset()
    {

    }
}
