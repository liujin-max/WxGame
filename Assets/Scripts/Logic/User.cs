using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;


//账号数据
[System.Serializable]
public class GameUserData
{
    public string Name = "unkown";
    public string HeadUrl;
    public int Score = 0;   //通关记录
    public List<int> AchiveRecords = new List<int>();
}

//排行榜数据
[System.Serializable]
public class RankData
{
    public string Name;
    public string Head;
    public int Score;

}

[System.Serializable]
public class RankDataInfo
{
    public RankData[] data;
}


//负责管理账号的各种数据
public class User : MonoBehaviour
{
    //用户数据
    [SerializeField] private GameUserData m_Data = new GameUserData();

    public string Name{ get{ return m_Data.Name;}}
    public int Score{ get{ return m_Data.Score;}}
    public string HeadURL{ get{ return m_Data.HeadUrl;}}

    private bool m_scoreUpdate = false; //记录变动标记
    public bool IsScoreUpdate {get { return m_scoreUpdate;}}

    private bool m_userUpdate = false;  //账号数据变动标记
    public bool IsDirty {get { return m_userUpdate || m_scoreUpdate;}}
    
    //从本地存档里同步数据
    public void Sync()
    {
        //同步账号基础数据(头像、名字)
        m_Data = Platform.Instance.LOGIN(m_Data, (m_Data)=>{
            //同步账号游玩数据(记录、成就等)
            Platform.Instance.SYNC(m_Data);
        });
    }

    //在失败或成功时统一调用
    public void Save()
    {
        Platform.Instance.UPLOAD(m_Data);

        //重置标记
        m_scoreUpdate   = false;
        m_userUpdate    = false;
    }




    public void SetScore(int value)
    {
        if (value <= m_Data.Score ) return;

        m_scoreUpdate   = true;
        m_userUpdate    = true;

        m_Data.Score = value;
    }

    public void SetAchievement(int id)
    {
        if (!m_Data.AchiveRecords.Contains(id)) {
            m_userUpdate    = true;
            m_Data.AchiveRecords.Add(id);
        }
    }

    public bool IsNewScore(int score)
    {
        if (m_Data.Score == 0) return false;

        return score > m_Data.Score;
    }
}
