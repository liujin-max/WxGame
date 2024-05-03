using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Platform
{
    private static Platform m_Platform;
    public static Platform Instance {
        get {
            if (m_Platform == null)
            {
                #if UNITY_EDITOR
                    m_Platform = new EditorPlatform();
                #elif WEIXINMINIGAME
                    m_Platform = new WXPlatform();
                #else
                    m_Platform = new EditorPlatform();
                #endif 
            }

            return m_Platform;
        }
    }

    public abstract void INIT(Action callback);
    public abstract GameUserData LOGIN(GameUserData userData, Action<GameUserData> callback);
    public abstract void SYNC(GameUserData userData);
    public abstract void UPLOAD(GameUserData userData);

    //设备振动
    public abstract void VIBRATE(string level);
}
