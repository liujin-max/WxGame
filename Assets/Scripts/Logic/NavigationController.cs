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

        //代表已经通关了
        if (json == null) 
        {
            EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, "正在加紧制作，敬请期待！"));
            return null;
        }

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
    public static bool GotoEndless()
    {
        if (GameFacade.Instance.DataCenter.User.Level < _C.ENDLESS_UNLOCK_LEVEL)
        {
            EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, string.Format("通过关卡{0}后开放", _C.ENDLESS_UNLOCK_LEVEL)));
            return false;
        }

        GameFacade.Instance.EffectManager.Load(EFFECT.SWITCH, Vector3.zero, UIManager.EFFECT.gameObject).GetComponent<SceneSwitch>().Enter(()=>{
            GameFacade.Instance.SoundManager.PlayBGM(SOUND.BGM);

            GameFacade.Instance.ScenePool.LoadSceneAsync("Game", () => {
                Field.Instance.Enter(10000);
            });
        });
        
        return true;
    }
}


