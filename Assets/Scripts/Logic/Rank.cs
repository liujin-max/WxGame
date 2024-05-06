using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CB
{
    //排行榜数据
    [System.Serializable]
    public class RankData
    {
        public string OpenID;
        public string Name;
        public string Head;
        public int Score;
        public int Order;

    }

    [System.Serializable]
    public class RankDataInfo
    {
        public RankData[] data;
    }


    public class Rank
    {
        //将上次拉取到的排行榜记录在本地
        private List<RankData> m_RankList = new List<RankData>();


        private static Rank m_instance = null;
        public static Rank Instance
        {
            get {
                if (m_instance == null){
                    m_instance = new Rank();
                }
                return m_instance; 
            }
        }

        //刷新本地排行榜
        public void UpdateRankList(RankData[] rankDatas)
        {
            m_RankList.Clear();

            for (int i = 0; i < rankDatas.Length; i++) {
                var rankData = rankDatas[i];
                rankData.Order = i + 1;
                m_RankList.Add(rankData);
            }

        }

        //获取我的排名
        public int GetMyRankOrder()
        {
            for (int i = 0; i < m_RankList.Count; i++) {
                var rankData = m_RankList[i];
                if (rankData.OpenID == GameFacade.Instance.User.OpenID) {
                    return rankData.Order;
                }
            }

            return 9999999;
        }

        //判断排行是否上升
        public void CheckRankingChanges(ref int origin_order, int score)
        {
            // RankData rankData = new RankData();
            // rankData.OpenID = GameFacade.Instance.User.OpenID;
            // rankData.Name   = GameFacade.Instance.User.Name;
            // rankData.Head   = GameFacade.Instance.User.HeadURL;
            // rankData.Score  = GameFacade.Instance.User.Score;
            // rankData.Order  = 3;

            // EventManager.SendEvent(new GameEvent(EVENT.UI_RANKINGUP, rankData));


            if (m_RankList.Count == 0) return;

            //被超越的人
            RankData final = null;

            //从高到低遍历，取最高的那个，所以可以直接break
            for (int i = 0; i < m_RankList.Count; i++)
            {
                var rankData = m_RankList[i];
                if (rankData.Order < origin_order) {
                    if (score > rankData.Score) {
                        final = rankData;
                        break;
                    }
                }
            }

            if (final != null) {
                origin_order = final.Order;
                //呼出
                EventManager.SendEvent(new GameEvent(EVENT.UI_RANKINGUP, final));
            }
        }
    }

}
