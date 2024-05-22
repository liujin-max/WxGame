using System.Collections;
using System.Collections.Generic;
using PC;
using UnityEngine;




public static class NavigationController
{
    public static void GotoLoading()
    {
        GameFacade.Instance.SoundManager.PlayBGM(SOUND.BGM);

        //加载loading界面
        GameFacade.Instance.ScenePool.LoadSceneAsync("Loading", () => {
            GameFacade.Instance.UIManager.LoadWindow("LoadingWindow", GameFacade.Instance.UIManager.BOTTOM);
        });
    }

    public static void GotoGame()
    {
        GameFacade.Instance.SoundManager.PlayBGM(SOUND.BGM);

        GameFacade.Instance.ScenePool.LoadSceneAsync("Game", () => {
            GameObject.Find("Entity").AddComponent<Field>();
            Field.Instance.Enter();
        });
    }
}


