using System;
using CB;
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
    public abstract void SYNC(BaseData baseData, GameUserData userData);
    public abstract void UPLOAD(GameUserData userData);
    //拉取排行榜
    public abstract void PULLRANK();
    //分享
    public abstract void SHARE();

    //激励广告
    public abstract void REWARD_VIDEOAD(string ad_id, Action callback = null);
    public abstract void BANNER_VIDEOAD(string ad_id, bool is_show, int top = 780);
    public abstract void INTER_VIDEOAD(string ad_id);

    //设备振动
    public abstract void VIBRATE(string level);
    //适配UI
    public abstract void ADAPTATION(RectTransform rectTransform);

}
