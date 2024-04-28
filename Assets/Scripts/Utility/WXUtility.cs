using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeChatWASM;

[System.Serializable]
public class OpenDataMessage
{
    // type 用于表明时间类型
    public string type;
    public string shareTicket;
    public int score;
}

public static class WXUtility
{
    public static void UnloadRankScore(int score)
    {
        #if WEIXINMINIGAME && ! UNITY_EDITOR
            OpenDataMessage data = new OpenDataMessage();
            data.type   = "setUserRecord";
            data.shareTicket   = "group1";
            data.score  = score;

            string msg = JsonUtility.ToJson(data);
            WX.GetOpenDataContext().PostMessage(msg);
        #endif
    }

    public static void ShowFriendsRank()
    {
        #if WEIXINMINIGAME && ! UNITY_EDITOR
            OpenDataMessage data = new OpenDataMessage();
            data.type   = "showFriendsRank";

            string msg = JsonUtility.ToJson(data);
            WX.GetOpenDataContext().PostMessage(msg);
        #endif
    }

    public static void ShowGroupRank()
    {
        #if WEIXINMINIGAME && ! UNITY_EDITOR
            OpenDataMessage data = new OpenDataMessage();
            data.type   = "showGroupFriendsRank";
            data.shareTicket   = "group1";

            string msg = JsonUtility.ToJson(data);
            WX.GetOpenDataContext().PostMessage(msg);
        #endif
    }
}
