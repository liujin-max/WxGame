using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Platform
{
    public static Platform Create()
    {
        #if UNITY_EDITOR
            return new EditorPlatform();
        #elif WEIXINMINIGAME
            return new WXPlatform();
        #else
            return new EditorPlatform();
        #endif 
    }

    public abstract void Init(Action callback);
    public abstract GameUserData Login(GameUserData userData, Action<GameUserData> callback);
    public abstract void Sync(GameUserData userData, Action<GameUserData> callback);
    public abstract void Upload(GameUserData userData);
}
