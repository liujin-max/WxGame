using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeChatWASM;


namespace CB
{
    public class RankWindow : MonoBehaviour
    {
        [SerializeField] private Transform m_Content;
        [SerializeField] private Transform m_OurPivot;

        private HeadItem m_HeadItem = null;

        public void Init(RankDataInfo data_info)
        {
            InitOur();
            InitRanks(data_info.data);
        }

        void InitRanks(RankData[] rankDatas)
        {
            for (int i = 0; i < rankDatas.Length; i++)
            {
                var item = GameFacade.Instance.UIManager.LoadItem("Prefab/UI/Item/RankItem", m_Content).GetComponent<RankItem>();
                item.Init(rankDatas[i]);
            }
        }

        void InitOur()
        {
            if (m_HeadItem == null) {
                m_HeadItem = GameFacade.Instance.UIManager.LoadItem("Prefab/UI/Item/HeadItem", m_OurPivot.Find("HeadPivot")).GetComponent<HeadItem>();
            } 
            m_HeadItem.Init(GameFacade.Instance.User.HeadURL);

            m_OurPivot.Find("Name").GetComponent<Text>().text = GameFacade.Instance.User.Name;
            m_OurPivot.Find("Score").GetComponent<Text>().text = string.Format("{0} 层", GameFacade.Instance.User.Score);
        }

        public void OnMask()
        {
            GameFacade.Instance.UIManager.UnloadWindow(gameObject);
        }
    }
}

