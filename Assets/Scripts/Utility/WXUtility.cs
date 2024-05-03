using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using WeChatWASM;


public class OpenDataMessage
{
    // type 用于表明时间类型
    public string type;
    public string shareTicket;
    public int score;
}




public static class WXUtility
{
    //上传排行榜数据
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

    //显示排行榜
    public static void ShowFriendsRank()
    {
        #if WEIXINMINIGAME && ! UNITY_EDITOR
            OpenDataMessage data = new OpenDataMessage();
            data.type   = "showFriendsRank";

            string msg = JsonUtility.ToJson(data);
            WX.GetOpenDataContext().PostMessage(msg);
        #endif
    }
}
