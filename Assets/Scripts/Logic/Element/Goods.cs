using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;


namespace Money
{
    /// <summary>
    /// 商品：市场上正在出售的
    /// </summary>
    public class Goods
    {
        private GoodsData m_Data;
        public int ID { get {return m_Data.ID;}}
        public string Name { get {return m_Data.Name;}}
        public string IncreaseDes { 
            get {
                string result = m_Data.Increase.Replace("#", _C.PRICE_INCREASE_COLOR + m_Data.Name + "</color>");
                return result;
            }
        }
        public string ReductionDes {
            get {
                string result = m_Data.Reduction.Replace("#", _C.PRICE_REDUCE_COLOR + m_Data.Name + "</color>");
                return result;
            }
        }

        private int m_Price;    //当前价格
        public int Price {get {return m_Price;}}
        private _C.GOODSTATE m_STATE;

        private Dictionary<object, int> m_StateWeightPairs = new Dictionary<object, int>();
        private Dictionary<object, int> m_StatePricePairs = new Dictionary<object, int>();

        public Goods(GoodsData data)
        {
            m_Data = data;

            //状态和权重的映射关系
            m_StateWeightPairs.Add(_C.GOODSTATE.UNSOLD, m_Data.Weights[0]);
            m_StateWeightPairs.Add(_C.GOODSTATE.NORMAL, m_Data.Weights[1]);
            m_StateWeightPairs.Add(_C.GOODSTATE.HOT,    m_Data.Weights[2]);

            //状态和价格的映射关系
            m_StatePricePairs.Add(_C.GOODSTATE.UNSOLD,  m_Data.Prices[0]);
            m_StatePricePairs.Add(_C.GOODSTATE.NORMAL,  m_Data.Prices[1]);
            m_StatePricePairs.Add(_C.GOODSTATE.HOT,     m_Data.Prices[2]);

            this.RandomPrice();
        }

        //价格变动
        public void RandomPrice()
        {
            m_STATE     = (_C.GOODSTATE)RandomUtility.PickByWeight(m_StateWeightPairs);

            int price   = m_StatePricePairs[m_STATE];
            m_Price     = RandomUtility.Random((int)Mathf.Ceil(price * 0.8f), (int)Mathf.Ceil(price * 1.2f));
        }

        //热销中
        public bool IsHotSelling()
        {
            return m_STATE == _C.GOODSTATE.HOT;
        }

        //滞销中
        public bool IsUnsold()
        {
            return m_STATE == _C.GOODSTATE.UNSOLD;
        }
    }
}

