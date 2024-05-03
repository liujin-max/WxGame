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
    public override void SYNC(GameUserData userData)
    {
        //同步成就完成记录
        userData.AchiveRecords.ForEach(id => {
            var ach = GameFacade.Instance.DataCenter.GetAchievement(id);
            if (ach != null) {
                ach.Sync();
            }
        });
    }

    public override void UPLOAD(GameUserData userData)
    {
        string json = JsonUtility.ToJson(userData);
        PlayerPrefs.SetString(SystemManager.KEY_USER, json);
        PlayerPrefs.Save();
    }

    public override void VIBRATE(string level)
    {

    }
}
