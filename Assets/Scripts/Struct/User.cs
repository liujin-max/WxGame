using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//账号数据
[System.Serializable]
public class GameUserData
{
    public int Score = 0;   //通关记录
}


//负责管理账号的各种数据
public class User
{
    private  const string KEY_SCORE = "KEY_SCORE";


    private GameUserData m_Data = new GameUserData();

    public int Score{ get{ return m_Data.Score;}}
    

    public void Init()
    {
        #if WEIXINMINIGAME && !UNITY_EDITOR
            //从云数据库加载存档
            WXUtility.Cloud_GetUserData(m_Data);
        #else
            m_Data.Score = PlayerPrefs.GetInt(KEY_SCORE, 0);
            Debug.Log("加载：" + m_Data.Score);
        #endif 
    }

    public void SetScore(int value)
    {
        if (value <= m_Data.Score ) return;

        m_Data.Score = value;

        Save();
    }

    public void Save()
    {
        int value = m_Data.Score;
        #if WEIXINMINIGAME && !UNITY_EDITOR
            //存储个人数据
            WXUtility.Cloud_SetUserData(m_Data);
            //上传排行榜
            WXUtility.UnloadRankScore(value);
        #else
            PlayerPrefs.SetInt(KEY_SCORE, value);
        #endif 
    }

    public bool IsNewScore(int score)
    {
        if (m_Data.Score == 0) return false;

        return score > m_Data.Score;
    }
}
