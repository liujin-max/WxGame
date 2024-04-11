using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace CB
{
    public static class NavigationController
    {
        public static void GotoLoading()
        {
            GameFacade.Instance.SoundManager.PlayBGM(SOUND.BGM);

            //加载loading界面
            GameFacade.Instance.ScenePool.LoadSceneAsync("Loading", () => {
                GameFacade.Instance.UIManager.LoadWindow("Prefab/UI/LoadingWindow", GameFacade.Instance.UIManager.BOTTOM);
            });
        }
    }
}

