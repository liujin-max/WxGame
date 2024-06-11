using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public static class NavigationController
{
    //前往主页
    public static void GotoLogin()
    {
        GameFacade.Instance.SoundManager.PlayBGM(SOUND.BGM);

        //加载loading界面
        GameFacade.Instance.ScenePool.LoadSceneAsync("Login", () => {
            var window = GameFacade.Instance.UIManager.LoadWindow("LoginWindow", UIManager.BOTTOM).GetComponent<LoginWindow>();
            window.Init();
        });
    }

    //前往关卡模式
    public static StageJSON GotoGame()
    {
        //判断体力
        // GameFacade.Instance.DataCenter.User.Level + 1
        int level   = GameFacade.Instance.TestMode == true ? GameFacade.Instance.TestStage : GameFacade.Instance.DataCenter.User.Level + 1;
        var json    = GameFacade.Instance.DataCenter.Level.GetStageJSON(level);

        if (!GameFacade.Instance.DataCenter.Level.IsFoodEnough2Next(json))
        {
            EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, "体力不足"));
            return null;
        }

        //扣除体力
        GameFacade.Instance.DataCenter.User.UpdateFood(-json.Food);

        GameFacade.Instance.EffectManager.Load(EFFECT.SWITCH, Vector3.zero, UIManager.EFFECT.gameObject).GetComponent<SceneSwitch>().Enter(()=>{
            GameFacade.Instance.SoundManager.PlayBGM(SOUND.BGM);

            GameFacade.Instance.ScenePool.LoadSceneAsync("Game", () => {
                Field.Instance.Enter(level);
            });
        });


        return json;
    }

    //前往无尽模式
    public static void GotoEndless()
    {
        GameFacade.Instance.EffectManager.Load(EFFECT.SWITCH, Vector3.zero, UIManager.EFFECT.gameObject).GetComponent<SceneSwitch>().Enter(()=>{
            GameFacade.Instance.SoundManager.PlayBGM(SOUND.BGM);

            GameFacade.Instance.ScenePool.LoadSceneAsync("Game", () => {
                Field.Instance.Enter(999);
            });
        });
    }
}


