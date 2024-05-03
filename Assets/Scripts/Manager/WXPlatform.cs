using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using WeChatWASM;

public class WXPlatform : Platform
{
    public override void INIT(Action callback)
    {
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
                //《四、获取用户信息》
                GetUserInfoOption userInfo = new GetUserInfoOption()
                {
                    withCredentials = true,
                    lang = "zh_CN",
                    success = (data) => {
                        userData.Name       = data.userInfo.nickName;
                        userData.HeadUrl    = data.userInfo.avatarUrl;
                        
                        Debug.Log("222 : " + userData.Name);
                        callback.Invoke(userData);
                    }
                };
                WX.GetUserInfo(userInfo);
            }
        };
        WX.GetSetting(info);

        return userData;
    }

    public override void SYNC(GameUserData userData)
    {
        Debug.Log("====开始获取账号数据====");
        
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
                    var gamedata    = data["data"];
   
                    //将Json转换成临时的GameUserData
                    GameUserData tempData = JsonUtility.FromJson<GameUserData>(JsonMapper.ToJson(gamedata));

                    //读取存档数据
                    userData.Score  = tempData.Score;

                    //读取成就数据
                    userData.AchiveRecords  = tempData.AchiveRecords;


                    GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHUSER));
                }
            },
            fail = (res) =>
            {
                Debug.LogError("====获取账号数据失败====");
            },
            complete = (res) =>
            {
                Debug.Log("====获取账号数据结束====");
            }
        });
        
    }

    public override void UPLOAD(GameUserData userData)
    {
        Debug.Log("====开始存储账号数据====");
        string json = JsonUtility.ToJson(userData);
        Debug.Log("Json : " + json);

        //存储账号数据
        WX.cloud.CallFunction(new CallFunctionParam()
        {
            name = "SetUserData",
            data = json,
            success = (res) =>
            {
                Debug.Log("====存储账号数据成功====");
            },
            fail = (res) =>
            {
                Debug.LogError("====存储账号数据失败====" + res.errMsg);
                Debug.Log(res.result);

            },
            complete = (res) =>
            {
                Debug.Log("====存储账号数据结束====");
            }
        });


        //上传排行榜
        // WXUtility.UnloadRankScore(userData.Score);
    }

    //设备振动
    public override void VIBRATE(string level)
    {
        VibrateShortOption op = new VibrateShortOption();
        op.type = level;
        WX.VibrateShort(op);
    }
}
