using System;
using System.Collections;
using System.Collections.Generic;
using CB;
using UnityEngine;

public class EditorPlatform : Platform
{
    public override void INIT(Action callback)
    {
        Application.targetFrameRate = _C.DEFAULT_FRAME;

        if (callback != null) {
            callback.Invoke();
        }
    }

    public override GameUserData LOGIN(GameUserData userData, Action<GameUserData> callback)
    {
        string json = PlayerPrefs.GetString(SystemManager.KEY_USER);

        if (string.IsNullOrEmpty(json)) {
            callback.Invoke(userData);
            return userData;
        }
        
        userData    = JsonUtility.FromJson<GameUserData>(json);

        callback.Invoke(userData);

        return userData;
    }

    //不做别的处理了
    //编辑器模式下 存档数据在Login的时候直接都从本地获取到了
    public override void SYNC(BaseData baseData, GameUserData userData)
    {
        GameFacade.Instance.User.SyncRecords(userData);

        EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHUSER));
    }

    public override void UPLOAD(GameUserData userData)
    {
        string json = JsonUtility.ToJson(userData);
        PlayerPrefs.SetString(SystemManager.KEY_USER, json);
        PlayerPrefs.Save();
    }

    public override void PULLRANK()
    {
        GameFacade.Instance.FlyTip("获取排行榜失败");

        //测试代码
        // string mp = @"{""data"":[{""OpenID"":""oEvu464ef9tMCx4c_e9As9a4tpDo"",""Head"":""https://thirdwx.qlogo.cn/mmopen/vi_32/4ep13af4pan2BUzOL1T5ia5M5DWhsae3Kib8c5IOUL8QHuBHwNsVyQxKjoiajCERV7JILyex4nmMExb2dfXMu0dkfgBITZuIOUrkWkKkdfmicFI/132"",""Name"":""imnotfunny。"",""Score"":11},{""OpenID"":""oEvu4658xDAZMbtJV5TRmX_5iazg"",""Head"":""https://thirdwx.qlogo.cn/mmopen/vi_32/AoXC8DwxrsiczH6ibyfL7Y8lMuialNPNo0tjS9N4sB6hl96wATT6k7FDW8OdtcEH5dhGHMw05ia75pRo5HEIynXhnVPnHku7cnyTGNfic674ZaAI/132"",""Name"":""沐浴露"",""Score"":9}]}";
        // RankDataInfo data_info = JsonUtility.FromJson<RankDataInfo>(mp);
        // //呼出排行榜
        // var obj = GameFacade.Instance.UIManager.LoadWindow("Prefab/UI/RankWindow", GameFacade.Instance.UIManager.BOARD);
        // var window = obj.GetComponent<RankWindow>();
        // window.Init(data_info);

    }

    public override void SHARE()
    {
        EventManager.SendEvent(new GameEvent(EVENT.ONSHAREGAME));
    }

    public override void VIBRATE(string level)
    {
        if (!GameFacade.Instance.SystemManager.VibrateFlag) return;

    }
}
