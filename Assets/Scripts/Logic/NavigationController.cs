using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public static class NavigationController
{
    public static void GotoLogin()
    {
        GameFacade.Instance.SoundManager.PlayBGM(SOUND.BGM);

        //加载loading界面
        GameFacade.Instance.ScenePool.LoadSceneAsync("Login", () => {
            var window = GameFacade.Instance.UIManager.LoadWindow("LoginWindow", UIManager.BOTTOM).GetComponent<LoginWindow>();
            window.Init();
        });
    }

    public static void GotoGame()
    {
        //判断体力
        // GameFacade.Instance.DataCenter.User.Level + 1
        int level       = GameFacade.Instance.TestMode == true ? GameFacade.Instance.TestStage : 1;
        if (!GameFacade.Instance.DataCenter.Level.IsFoodEnough2Next(level))
        {

            return;
        }


        GameFacade.Instance.SoundManager.PlayBGM(SOUND.BGM);

        GameFacade.Instance.ScenePool.LoadSceneAsync("Game", () => {
            Field.Instance.Enter(level);

            GameFacade.Instance.DataCenter.User.UpdateFood(-Field.Instance.Stage.Food);
        });
    }
}


