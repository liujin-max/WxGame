using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Money
{

    /// <summary>
    /// 市场：负责生成商品和存放我当前仓库的商品
    /// </summary>
    public class Market 
    {
        private List<Goods> m_Goods = new List<Goods>();
        private List<Package> m_Packages = new List<Package>();


        public void Init()
        {
            InitGoods();

        }

        //生成商品
        void InitGoods()
        {
            m_Goods.Clear();

            GameFacade.Instance.DataCenter.GetGoodsDatas().ForEach(data => {
                Goods goods = new Goods(data);
                m_Goods.Add(goods);
            });
        }
    }
}
