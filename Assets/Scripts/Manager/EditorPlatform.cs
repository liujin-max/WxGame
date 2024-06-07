using System;
using System.Collections;
using System.Collections.Generic;
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
        GameFacade.Instance.DataCenter.User.SyncRecords(userData);


    }

    public override void UPLOAD(GameUserData userData)
    {
        string json = JsonUtility.ToJson(userData);
        PlayerPrefs.SetString(SystemManager.KEY_USER, json);
        PlayerPrefs.Save();
    }

    public override void PULLRANK()
    {


    }

    public override void SHARE()
    {

    }

    //激励广告
    public override void REWARDVIDEO()
    {

    }

    public override void VIBRATE(string level)
    {
        if (!GameFacade.Instance.SystemManager.VibrateFlag) return;

    }

    //适配UI
    public override void ADAPTATION(RectTransform rectTransform)
    {

    }
}
