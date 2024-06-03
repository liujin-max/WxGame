using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public static class NavigationController
{
    public static void GotoLoading()
    {
        GameFacade.Instance.SoundManager.PlayBGM(SOUND.BGM);

        //加载loading界面
        GameFacade.Instance.ScenePool.LoadSceneAsync("Loading", () => {
            GameFacade.Instance.UIManager.LoadWindow("LoadingWindow", UIManager.BOTTOM);
        });
    }

    public static void GotoGame()
    {
        GameFacade.Instance.SoundManager.PlayBGM(SOUND.BGM);

        GameFacade.Instance.ScenePool.LoadSceneAsync("Game", () => {
            Field.Instance.Enter(1);
        });
    }
}


