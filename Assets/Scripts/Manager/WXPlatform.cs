using System;
using System.Collections;
using System.Collections.Generic;
using CB;
using LitJson;
using UnityEngine;
using WeChatWASM;

public class WXPlatform : Platform
{
    public override void INIT(Action callback)
    {
        GameObject.Find("EventSystem").AddComponent<WXTouchInputOverride>();

        WX.InitSDK((code) => {
            //设置帧率
            WX.SetPreferredFramesPerSecond(_C.DEFAULT_FRAME);

            //初始化云开发
            CallFunctionInitParam param = new CallFunctionInitParam();
            param.env = _C.CLOUD_ENV;
            WX.cloud.Init(param);

            //上报启动
            WX.ReportGameStart();

            
            if (callback != null) {
                callback.Invoke();
            }
        });
    }

    public override GameUserData LOGIN(GameUserData userData, Action<GameUserData> callback)
    {
        GetSettingOption info = new GetSettingOption();
        info.complete = (aa) => { /*获取完成*/ };
        info.fail = (aa) => { Debug.Log("获取失败"); };
        info.success = (aa) =>
        {
            if (!aa.authSetting.ContainsKey("scope.userInfo") || !aa.authSetting["scope.userInfo"])
            {
                //《三、调起授权》
                //调用请求获取用户信息
                WXUserInfoButton btn = WX.CreateUserInfoButton(0, 0, Screen.width, Screen.height, "zh_CN", true);
                btn.OnTap((res) => {
                    if (res.errCode == 0) {
                        //用户已允许获取个人信息，返回的data即为用户信息
                        userData.Name       = res.userInfo.nickName;
                        userData.HeadUrl    = res.userInfo.avatarUrl;

                        Debug.Log("111 : " + userData.Name);

                    } else {
                        Debug.Log("用户未允许获取个人信息");
                    }
                    btn.Hide();

                    callback.Invoke(userData);
                });
            }
            else
            {
                EventManager.SendEvent(new GameEvent(EVENT.UI_NETUPDATE, true));
                //《四、获取用户信息》
                GetUserInfoOption userInfo = new GetUserInfoOption()
                {
                    withCredentials = true,
                    lang = "zh_CN",
                    success = (data) => {
                        userData.Name       = data.userInfo.nickName;
                        userData.HeadUrl    = data.userInfo.avatarUrl;
                        
                        Debug.Log("222 : " + userData.Name);
                        EventManager.SendEvent(new GameEvent(EVENT.UI_NETUPDATE, false));
                        callback.Invoke(userData);
                    }
                };
                WX.GetUserInfo(userInfo);
            }
        };
        WX.GetSetting(info);

        return userData;
    }

    //启动同步账号数据
    public override void SYNC(GameUserData userData)
    {
        Debug.Log("====开始获取账号数据====");
        EventManager.SendEvent(new GameEvent(EVENT.UI_NETUPDATE, true));
        //云开发：加载积分数据
        WX.cloud.CallFunction(new CallFunctionParam()
        {
            name = "GetUserData",
            data = JsonUtility.ToJson(""),
            success = (res) =>
            {
                Debug.Log("====获取账号数据成功==== : " + res.result);
                //云数据保存到本地
                var data = JsonMapper.ToObject(res.result);
                if (data.ContainsKey("data"))
                {
                    //将Json转换成临时的GameUserData
                    GameUserData tempData = JsonUtility.FromJson<GameUserData>(JsonMapper.ToJson(data["data"]));
                    //基础数据
                    userData.userInfo = tempData.userInfo;
                    //读取存档数据
                    userData.Score  = tempData.Score;
                    //读取成就数据
                    userData.AchiveRecords  = tempData.AchiveRecords;
                }
            },
            fail = (res) =>
            {
                Debug.LogError("====获取账号数据失败====" + res.errMsg);
            },
            complete = (res) =>
            {
                Debug.Log("====获取账号数据结束====");
                GameFacade.Instance.User.SyncRecords(userData);
                
                EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHUSER));
                EventManager.SendEvent(new GameEvent(EVENT.UI_NETUPDATE, false));
            }
        });
        
        //云开发：加载积分数据
        WX.cloud.CallFunction(new CallFunctionParam()
        {
            name = "GetRank",
            data = JsonUtility.ToJson(""),
            success = (res) =>
            {
                Debug.Log("====获取排行数据成功==== : " + res.result);
                var data = JsonMapper.ToObject(res.result);
                if (data.ContainsKey("data")) {
                    RankDataInfo data_info = JsonUtility.FromJson<RankDataInfo>(res.result);

                    //记录在本地
                    Rank.Instance.UpdateRankList(data_info.data);
                }
            }
        });

    }

    public override void UPLOAD(GameUserData userData)
    {
        //上传账号数据
        {
            if (GameFacade.Instance.User.IsDirty == true)
            {
                Debug.Log("====开始上传账号数据====");
                WX.cloud.CallFunction(new CallFunctionParam()
                {
                    name = "SetUserData",
                    data = JsonUtility.ToJson(userData),
                    success = (res) =>
                    {
                        Debug.Log("====上传账号数据成功====");
                    },
                    fail = (res) =>
                    {
                        Debug.LogError("====上传账号数据失败====" + res.errMsg);
                    }
                });
            }
        } 
    }

    //拉取排行榜
    public override void PULLRANK()
    {
        Debug.Log("====开始获取排行数据====");
        EventManager.SendEvent(new GameEvent(EVENT.UI_NETUPDATE, true));

        //云开发：加载积分数据
        WX.cloud.CallFunction(new CallFunctionParam()
        {
            name = "GetRank",
            data = JsonUtility.ToJson(""),
            success = (res) =>
            {
                Debug.Log("====获取排行数据成功==== : " + res.result);
                var data = JsonMapper.ToObject(res.result);
                if (data.ContainsKey("data")) {
                    RankDataInfo data_info = JsonUtility.FromJson<RankDataInfo>(res.result);

                    //记录在本地
                    Rank.Instance.UpdateRankList(data_info.data);

                    //呼出排行榜
                    var obj = GameFacade.Instance.UIManager.LoadWindow("Prefab/UI/RankWindow", GameFacade.Instance.UIManager.BOARD);
                    var window = obj.GetComponent<RankWindow>();
                    window.Init(data_info);
                }
            },
            fail = (res) =>
            {
                GameFacade.Instance.FlyTip("获取排行榜失败");
            },
            complete = (res) =>
            {
                EventManager.SendEvent(new GameEvent(EVENT.UI_NETUPDATE, false));
            }
        });
    }

    //分享
    public override void SHARE()
    {
        WX.ShareAppMessage(new ShareAppMessageOption()
        {
            title       = "新的记录！！！",
            // imageUrlId  = "",
            // imageUrl    = "https://inews.gtimg.com/newsapp_bt/0/12171811596_909/0",
        });
    }

    //设备振动
    public override void VIBRATE(string level)
    {
        if (!GameFacade.Instance.SystemManager.VibrateFlag) return;
        
        VibrateShortOption op = new VibrateShortOption();
        op.type = level;
        WX.VibrateShort(op);
    }
}
