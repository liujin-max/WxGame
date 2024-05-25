using System;
using System.Collections;
using System.Collections.Generic;
using CB;
using LitJson;
using UnityEngine;

#if WEIXINMINIGAME  //&& !UNITY_EDITOR
using WeChatWASM;


public class WXPlatform : Platform
{
    private Dictionary<string, WXBannerAd> m_BannerADPairs = new Dictionary<string, WXBannerAd>();

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
    public override void SYNC(BaseData baseData, GameUserData userData)
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

                    //读取存档数据
                    userData.Score  = tempData.Score;
                    //读取成就数据
                    userData.AchiveRecords  = tempData.AchiveRecords;

                    //基础数据
                    BaseData userInfo   = JsonUtility.FromJson<BaseData>(JsonMapper.ToJson(data["data"]["userInfo"]));
                    baseData.openId     = userInfo.openId;
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
        if (GameFacade.Instance.User.IsDirty == true)
        {
            //上传账号数据
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

    //拉取排行榜
    public override void PULLRANK()
    {
        Debug.Log("====开始清空排行数据====");
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
        EventManager.SendEvent(new GameEvent(EVENT.ONSHAREGAME));
        
        WX.ShareAppMessage(new ShareAppMessageOption()
        {
            title       = "进入弹珠世界！",
            imageUrlId  = "ha6CwV+bTPeClHQlbkrDqw==",
            imageUrl    = "https://mmocgame.qpic.cn/wechatgame/3aU0WbWrP4x4zsIvWz14yOrRianJOLpSLPiaMVnv60FOBhFYabc7zNIjYI2z9FuicAn/0",
        });
    }

    void LoadAD(WXRewardedVideoAd ad, Action callback)
    {
        ad.Load((WXTextResponse reponse)=>{
            callback();
        }, (WXADErrorResponse error_reponse)=>{
            LoadAD(ad, callback);
        });
    }

    //激励广告
    public override void REWARD_VIDEOAD(string ad_id, Action callback)
    {
        WXCreateRewardedVideoAdParam param = new WXCreateRewardedVideoAdParam();
        param.adUnitId = ad_id;

        WXRewardedVideoAd ad = WX.CreateRewardedVideoAd(param);
        this.LoadAD(ad, ()=>{
            ad.Show((WXTextResponse reponse)=>{
                
            }, (WXTextResponse error_reponse)=>{
                GameFacade.Instance.FlyTip("广告展示失败：" + error_reponse.errCode);
            });
        });

        ad.OnClose((WXRewardedVideoAdOnCloseResponse reponse)=>{
            // Debug.Log("是否完成观看：" + reponse.isEnded);
            if (reponse != null && reponse.isEnded == true) {
                if (callback != null) 
                    callback();
            }
        });
    }

    //Banner广告
    public override void BANNER_VIDEOAD(string ad_id, bool is_show, int top = 780)
    {
        WXBannerAd ad;
        if (m_BannerADPairs.TryGetValue(ad_id, out ad)) {
            
        } else {
            WXCreateBannerAdParam param = new WXCreateBannerAdParam();
            param.adUnitId      = ad_id;
            param.adIntervals   = 30;
            param.style.left    = 60;
            param.style.top     = top;
            param.style.width   = 300;
            param.style.height  = 200;

            ad = WX.CreateBannerAd(param);
            m_BannerADPairs.Add(ad_id, ad);
        }

        if (is_show == true) {
            ad.Show((WXTextResponse reponse)=>{

            }, (WXTextResponse reponse)=>{
                GameFacade.Instance.FlyTip("广告展示失败：" + reponse.errCode);
            });
        } else {
            ad.Hide();
        }
    }

    //插屏广告
    public override void INTER_VIDEOAD(string ad_id)
    {
        WXCreateInterstitialAdParam param = new WXCreateInterstitialAdParam();
        param.adUnitId = ad_id;

        WXInterstitialAd ad = WX.CreateInterstitialAd(param);
        ad.Show((WXTextResponse reponse)=>{
            // Debug.Log("Show 成功：" + reponse.errCode);
        }, (WXTextResponse reponse)=>{
            // Debug.Log("Show 失败：" + reponse.errCode);
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

    //适配UI
    public override void ADAPTATION(RectTransform rectTransform)
    {
        var info = WX.GetSystemInfoSync();
        float py = (float)info.safeArea.top / (float)info.windowHeight;

        // Debug.Log("safeArea : " + info.safeArea.top);
        // Debug.Log("windowHeight : " + info.windowHeight);
        // Rootrect初始时设置其Anchor，使其与父节点一样大，也就是屏幕的大小
        // 调整屏幕移到刘海屏下面, 
        float rate = (float)info.safeArea.top / (float)info.windowHeight;
        rectTransform.anchorMin = new Vector2(0,  rate);

        rectTransform.anchorMax = new Vector2(1, 1 - rate);

        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }
}

#endif