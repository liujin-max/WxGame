using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeChatWASM;


namespace CB
{
    public class RankWindow : MonoBehaviour
    {
        [SerializeField] private Transform c_TopPivot;
        [SerializeField] private Transform c_Content;
        [SerializeField] private Transform c_OurPivot;


        public void Init(RankDataInfo data_info)
        {
            InitOur();
            InitTop(data_info.data);
            InitRanks(data_info.data);
        }

        void InitTop(RankData[] rankDatas)
        {
            for (int i = 0; i < 3; i++)
            {
                var top     = c_TopPivot.Find((i + 1).ToString());

                var head    = GameFacade.Instance.UIManager.LoadItem("Prefab/UI/Item/HeadItem", top.Find("HeadPivot")).GetComponent<HeadItem>();
                var name    = top.Find("Name").GetComponent<Text>();
                var score   = top.Find("Score").GetComponent<Text>();

                c_OurPivot.Find("Score").GetComponent<Text>().text = string.Format("{0} 层", GameFacade.Instance.User.Score);

                if (rankDatas.Length > i) {
                    RankData rankData = rankDatas[i];

                    head.Init(rankData.Head);
                    name.text   = rankData.Name;
                    score.text  = string.Format("{0} 层", rankData.Score);;
                } else {
                    name.text   = "虚位以待";
                    score.text  = string.Empty;
                }
            }
        }

        void InitRanks(RankData[] rankDatas)
        {
            if (rankDatas.Length <= 3) return;

            for (int i = 3; i < rankDatas.Length; i++)
            {
                var item = GameFacade.Instance.UIManager.LoadItem("Prefab/UI/Item/RankItem", c_Content).GetComponent<RankItem>();
                item.Init(rankDatas[i]);
            }
        }

        void InitOur()
        {
            var item = GameFacade.Instance.UIManager.LoadItem("Prefab/UI/Item/HeadItem", c_OurPivot.Find("HeadPivot")).GetComponent<HeadItem>();
            item.Init(GameFacade.Instance.User.HeadURL);

            int order = Rank.Instance.GetMyRankOrder();
            if (order == _C.DEFAULT_RANK) {
                c_OurPivot.Find("-/Order").GetComponent<Text>().text = "未上榜";
            } else {
                c_OurPivot.Find("-/Order").GetComponent<Text>().text = order.ToString();
            }

            c_OurPivot.Find("Name").GetComponent<Text>().text = GameFacade.Instance.User.Name;
            c_OurPivot.Find("Score").GetComponent<Text>().text = string.Format("{0} 层", GameFacade.Instance.User.Score);
        }

        public void OnMask()
        {
            GameFacade.Instance.UIManager.UnloadWindow(gameObject);
        }
    }
}

