using Cysharp.Threading.Tasks;
using DG.Tweening;
using Kit.Common;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameState GameState;
    [SerializeField] bool isCustom = true;
    [SerializeField] int levelIndex = 1;

    private void Start()
    {
        //TTPCore.Setup();
        GameState = GameState.Playing;
        Application.targetFrameRate = 60;
        Input.multiTouchEnabled = false;
        DOTween.SetTweensCapacity(1000, 500);
        StartLevel();
    }

    public void ShowWin()
    {
        if (GameState != GameState.Playing) return;
        GameState = GameState.Win;
        //AudioManager.instance.PlayWinSfx();
        //GameUI.instance.Get<UIWin>().Show();
    }

    public void ShowLose()
    {
        if (GameState != GameState.Playing) return;
        GameState = GameState.Lose;
        //AudioManager.instance.PlayLoseSfx();
        //ClikMissionFail();
        //GameUI.instance.Get<UILose>().Show();
    }

    public async void Retry()
    {
        //Debug.Log("Retry");
        await UniTask.DelayFrame(0);
        //UIIngame uIIngame = GameUI.instance.Get<UIIngame>();
        //uIIngame.SetLevelText(User.data.CurrentLevel + 1);
        GameState = GameState.Playing;

        LevelController.instance.OnReset();
        LevelController.instance.SetUp(User.data.currentLevel);
    }

    public async void NextLevel()
    {
        Debug.Log("Next Level");
        User.data.currentLevel++;
        if (User.data.currentLevel >= LevelController.instance.levelConfig.GetTotalLevel())
            User.data.currentLevel = 0;
        User.Save();
        //UIIngame uIIngame = GameUI.instance.Get<UIIngame>();
        //uIIngame.SetLevelText(User.data.currentLevel + 1);
        await UniTask.DelayFrame(0);

        GameState = GameState.Playing;

        //LevelController.instance.OnReset();
        //LevelController.instance.SetUp(User.data.currentLevel);
        if (User.data.currentLevel > 1) return;
    }

    void StartLevel()
    {
        if (isCustom)
        {
            CustomLevel();
            return;
        }

        //UIIngame uIIngame = GameUI.instance.Get<UIIngame>();
        //uIIngame.Show();
        //uIIngame.SetLevelText(User.data.currentLevel + 1);

        LevelController.instance.SetUp(User.data.currentLevel);
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void CustomLevel()
    {
        User.data.currentLevel = levelIndex - 1;

        LevelController.instance.OnReset();
        LevelController.instance.SetUp(levelIndex - 1);


        //UIIngame uIIngame = GameUI.instance.Get<UIIngame>();
        //uIIngame.Show();
        //uIIngame.SetLevelText(User.data.currentLevel + 1);
    }

  
}
