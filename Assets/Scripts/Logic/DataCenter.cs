using System.Collections.Generic;
using UnityEngine;
using WeChatWASM;

namespace CB
{
    //全局数据类
    public class DataCenter
    {
        //存储所有的成就类
        public void Init()
        {
            InitAchievements();
        }



        #region 成就
        private List<Achievement> m_Achievements = new List<Achievement>();
        private Dictionary<int, Achievement> m_AchievementsDic = new Dictionary<int, Achievement>();
        public List<Achievement> Achievements {get{return m_Achievements;}}

        void InitAchievements()
        {
            var list_datas = CONFIG.GetAchievementDatas();
            list_datas.ForEach(data => {
                Achievement achievement = Achievement.Create(data);
                m_Achievements.Add(achievement);
                m_AchievementsDic[data.ID]  = achievement;
            });
        }
        
        public Achievement GetAchievement(int id)
        {
            return m_AchievementsDic[id];
        }

        #endregion




    }
}
