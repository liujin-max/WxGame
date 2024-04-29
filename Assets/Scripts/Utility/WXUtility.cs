using System.Collections;
using System.Collections.Generic;
using LitJson;
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

    // public static void ShowGroupRank()
    // {
    //     #if WEIXINMINIGAME && ! UNITY_EDITOR
    //         OpenDataMessage data = new OpenDataMessage();
    //         data.type   = "showGroupFriendsRank";
    //         data.shareTicket   = "group1";

    //         string msg = JsonUtility.ToJson(data);
    //         WX.GetOpenDataContext().PostMessage(msg);
    //     #endif
    // }

    //云开发：上传账号数据
    public static void Cloud_SetUserData(GameUserData gameUserData)
    {
        Debug.Log("====开始存储账号数据====");

        WX.cloud.CallFunction(new CallFunctionParam()
        {
            name = "SetUserData",
            data = JsonUtility.ToJson(gameUserData),
            success = (res) =>
            {
                Debug.Log("====存储账号数据成功====");
            },
            fail = (res) => { Debug.LogError("====存储账号数据失败====");},
            complete = (res) => { Debug.Log("====存储账号数据结束====");}
        });
    }

    //云开发：获取账号数据
    public static void Cloud_GetUserData(GameUserData userData)
    {
        Debug.Log("====开始获取账号数据====");

        WX.cloud.CallFunction(new CallFunctionParam()
        {
            name = "GetUserData",
            //不填data，或填空串时，调用云函数失败，原因不明，大佬评论区教我哈哈
            data = JsonUtility.ToJson(""), //JsonUtility.ToJson(new GameUserData()),
            success = (res) =>
            {
                Debug.Log("====获取账号数据成功====");

                //云数据保存到本地
                var data = JsonMapper.ToObject(res.result);
                if (data.ContainsKey("gamedata"))
                {
                    var gamedata    = data["gamedata"];
                    if (gamedata.ContainsKey("Score")) userData.Score  = (int)gamedata["Score"];
                }
            },
            fail = (res) => { Debug.LogError("====获取账号数据失败====");},
            complete = (res) => { Debug.Log("====获取账号数据结束====");}
        });
    }
}
