using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;



namespace CB
{
    //基础数据
    [System.Serializable]
    public class BaseInfo
    {
        public string appId;
        public string openId;
    }

    //账号数据
    [System.Serializable]
    public class GameUserData
    {
        public BaseInfo userInfo;

        public string Name = "Unkown";
        public string HeadUrl;
        public int Score = 0;   //通关记录
        public List<int> AchiveRecords = new List<int>();
    }

    //临时关卡存档数据
    [System.Serializable]
    public class ArchiveRecord
    {
        public bool Valid = false;  //是否有效

        public int Order;
        public int Coin;
        public int Glass;
        public List<string> BallRecords;    //弹珠不是唯一的，所以不能用Dictionary
        public List<string> RelicsRecords;
    }


    //负责管理账号的各种数据
    public class User : MonoBehaviour
    {
        //用户数据
        [SerializeField] private GameUserData m_Data = new GameUserData();

        public string OpenID{ get{ return m_Data.userInfo.openId;}}
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

        public void SyncRecords(GameUserData userData)
        {
            //同步成就完成记录
            userData.AchiveRecords.ForEach(id => {
                var ach = GameFacade.Instance.DataCenter.GetAchievement(id);
                if (ach != null) {
                    ach.Sync();
                }
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

            m_Data.Score    = value;

            m_scoreUpdate   = true;
            m_userUpdate    = true;
        }

        public void SetAchievement(int id)
        {
            if (!m_Data.AchiveRecords.Contains(id)) {
                m_Data.AchiveRecords.Add(id);

                m_userUpdate    = true;
            }
        }

        public bool IsNewScore(int score)
        {
            if (m_Data.Score == 0) return false;

            return score > m_Data.Score;
        }

        public ArchiveRecord GetArchiveRecord()
        {
            string json = PlayerPrefs.GetString(SystemManager.KEY_ARCHIVE);

            if (string.IsNullOrEmpty(json) == true) {
                return null;
            }

            Debug.Log("读取关卡存档：" + json);
            ArchiveRecord record = JsonUtility.FromJson<ArchiveRecord>(json);
            if (!record.Valid) {
                return null;
            }

            return record;
        }
    }

}
