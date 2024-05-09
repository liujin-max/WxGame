using System;
using CB;

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

    //设备振动
    public abstract void VIBRATE(string level);

}
