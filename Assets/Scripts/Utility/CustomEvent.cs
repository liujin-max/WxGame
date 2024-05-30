using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//上报事件
[System.Serializable]
public class Event_BuyRelics    //购买道具
{
    public string m_name;
    public int m_coin;
    public int m_glass;
    public int m_stage;

    public int m_relics_id;

    public Event_BuyRelics(int id)
    {
        m_relics_id = id;

        m_name  = GameFacade.Instance.User.Name;
        m_coin  = GameFacade.Instance.Game.Coin;
        m_glass = GameFacade.Instance.Game.Glass;
        m_stage = GameFacade.Instance.Game.Stage;
    }
}

[System.Serializable]
public class Event_ComplexBall    //合成弹珠
{
    public string m_name;
    public int m_coin;
    public int m_glass;
    public int m_stage;

    public int m_ball_id;

    public Event_ComplexBall(int id)
    {
        m_ball_id = id;

        m_name  = GameFacade.Instance.User.Name;
        m_coin  = GameFacade.Instance.Game.Coin;
        m_glass = GameFacade.Instance.Game.Glass;
        m_stage = GameFacade.Instance.Game.Stage;
    }
}

[System.Serializable]
public class Event_LookVideo    //观看广告
{
    public string m_name;
    public int m_coin;
    public int m_glass;
    public int m_stage;

    public string m_ad_id;

    public Event_LookVideo(string ad_id)
    {
        m_ad_id = ad_id;

        m_name  = GameFacade.Instance.User.Name;
        m_coin  = GameFacade.Instance.Game.Coin;
        m_glass = GameFacade.Instance.Game.Glass;
        m_stage = GameFacade.Instance.Game.Stage;
    }
}

public static class CustomEvent
{
    public static string BuyRelics = "e_buyrelics";
    public static string ComplexBall = "e_complexball";

    public static string LookVideo = "e_lookvideo";
}
