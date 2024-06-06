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
            GameFacade.Instance.UIManager.LoadWindow("LoginWindow", UIManager.BOTTOM);
        });
    }

    public static void GotoGame()
    {
        GameFacade.Instance.SoundManager.PlayBGM(SOUND.BGM);

        GameFacade.Instance.ScenePool.LoadSceneAsync("Game", () => {
            int stage = GameFacade.Instance.TestMode == true ? GameFacade.Instance.TestStage : 1;
            Field.Instance.Enter(stage);
        });
    }
}


