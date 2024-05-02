using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//账号数据
[System.Serializable]
public class GameUserData
{
    public int Score = 0;   //通关记录
    public List<int> AchiveRecords = new List<int>();
}


//负责管理账号的各种数据
public class User : MonoBehaviour
{
    private  const string KEY_USER  = "KEY_USER";

    //用户数据
    [SerializeField] private GameUserData m_Data = new GameUserData();

    public int Score{ get{ return m_Data.Score;}}
    
    //从本地存档里同步数据
    public void Sync()
    {
        if (GameFacade.Instance.Reboot == true) {
            //重置本地存档
            return;
        }

        #if WEIXINMINIGAME && !UNITY_EDITOR
            //从云数据库加载存档
            WXUtility.Cloud_GetUserData(m_Data);
        #else
            string json = PlayerPrefs.GetString(KEY_USER);
            m_Data = JsonUtility.FromJson<GameUserData>(json);
        #endif 

        //同步成就完成记录
        m_Data.AchiveRecords.ForEach(id => {
            var ach = GameFacade.Instance.DataCenter.GetAchievement(id);
            if (ach != null) {
                ach.Sync();
            }
        });
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

    //在失败或成功时统一调用
    public void Save()
    {
        int value = m_Data.Score;
        #if WEIXINMINIGAME && !UNITY_EDITOR
            //存储分数
            WXUtility.Cloud_SetUserData(m_Data);
            //上传排行榜
            WXUtility.UnloadRankScore(value);
        #else
            string json = JsonUtility.ToJson(m_Data);
            PlayerPrefs.SetString(KEY_USER, json);
            PlayerPrefs.Save();
        #endif 
    }

    public bool IsNewScore(int score)
    {
        if (m_Data.Score == 0) return false;

        return score > m_Data.Score;
    }
}
