using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;




//基础数据
[System.Serializable]
public class BaseData
{
    public string openId;
}

//账号数据
[System.Serializable]
public class GameUserData
{
    public string Name = "未知";
    public string HeadUrl;
    public int Level = 0;   //通关记录
    public int Coin = 0;    //金币
}




//负责管理账号的各种数据
public class User : MonoBehaviour
{
    //用户数据
    [SerializeField] private BaseData m_Base = new BaseData();
    [SerializeField] private GameUserData m_Data = new GameUserData();

    public string OpenID{ get{ return m_Base.openId;}}
    public string Name{ get{ return m_Data.Name;}}
    public string HeadURL{ get{ return m_Data.HeadUrl;}}
    public int Level{ get{ return m_Data.Level;}}
    public int Coin{ get{ return m_Data.Coin;}}


    private bool m_userUpdate = false;  //账号数据变动标记
    private bool m_scoreUpdate = false; //记录变动标记
    public bool IsDirty {get { return m_userUpdate || m_scoreUpdate;}}
    
    //从本地存档里同步数据
    public void Sync()
    {
        //同步账号基础数据(头像、名字)
        m_Data = Platform.Instance.LOGIN(m_Data, (m_Data)=>{
            //同步账号游玩数据(记录、成就等)
            Platform.Instance.SYNC(m_Base, m_Data);
        });
    }

    public void SyncRecords(GameUserData userData)
    {
        //同步成就完成记录

    }

    //在失败或成功时统一调用
    public void Save()
    {
        Platform.Instance.UPLOAD(m_Data);

        //重置标记
        m_scoreUpdate   = false;
        m_userUpdate    = false;
    }

    public void SetLevel(int value)
    {
        if (value <= m_Data.Level ) return;

        m_Data.Level    = value;


        m_userUpdate    = true;
    }

    public void SetCoin(int value)
    {
        m_Data.Coin = value;

        m_userUpdate    = true;
    }
}


