using System.Collections;
using System.Collections.Generic;
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


//负责管理账号的各种数据
public class User : MonoBehaviour
{
    //用户数据
    [SerializeField] private GameUserData m_Data = new GameUserData();

    public int Score{ get{ return m_Data.Score;}}
    public string HeadURL{ 
        get{ return m_Data.HeadUrl;}
    }
    
    //从本地存档里同步数据
    public void Sync()
    {
        //同步账号基础数据(头像、名字)
        m_Data = GameFacade.Instance.Platform.Login(m_Data, (m_Data)=>{
            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHUSER));
        });

        //同步账号游玩数据(记录、成就等)
        GameFacade.Instance.Platform.Sync(m_Data, (m_Data)=>{
            //同步成就完成记录
            m_Data.AchiveRecords.ForEach(id => {
                var ach = GameFacade.Instance.DataCenter.GetAchievement(id);
                if (ach != null) {
                    ach.Sync();
                }
            });
        });
    }

    //在失败或成功时统一调用
    public void Save()
    {
        GameFacade.Instance.Platform.Upload(m_Data);
    }





    public void SetScore(int value)
    {
        if (value <= m_Data.Score ) return;

        m_Data.Score = value;
    }

    public void SetAchievement(int id)
    {
        if (!m_Data.AchiveRecords.Contains(id)) {
            m_Data.AchiveRecords.Add(id);
        }
    }

    public bool IsNewScore(int score)
    {
        if (m_Data.Score == 0) return false;

        return score > m_Data.Score;
    }
}
