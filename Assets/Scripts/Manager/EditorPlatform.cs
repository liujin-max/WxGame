using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorPlatform : Platform
{
    public override void Init(Action callback)
    {
        Application.targetFrameRate = _C.DEFAULT_FRAME;

        if (callback != null) {
            callback.Invoke();
        }
    }

    public override GameUserData Login(GameUserData userData, Action<GameUserData> callback)
    {
        if (GameFacade.Instance.Reboot == true) {
            //重置本地存档
            return userData;
        }

        string json = PlayerPrefs.GetString(_C.KEY_USER);
        userData    = JsonUtility.FromJson<GameUserData>(json);

        callback.Invoke(userData);

        return userData;
    }

    //不做别的处理了
    //编辑器模式下 存档数据在Login的时候直接都从本地获取到了
    public override void Sync(GameUserData userData, Action<GameUserData> callback)
    {
        callback.Invoke(userData);
    }

    public override void Upload(GameUserData userData)
    {
        string json = JsonUtility.ToJson(userData);
        PlayerPrefs.SetString(_C.KEY_USER, json);
        PlayerPrefs.Save();
    }
}
