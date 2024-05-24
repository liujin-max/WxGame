using System.Collections;
using System.Collections.Generic;
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

        private int m_Price;    //当前价格
        public int Price {get {return m_Price;}}

        public Goods(GoodsData data)
        {
            m_Data = data;

            m_Price= data.Price_Normal;
        }

        public void RandomPrice()
        {
            m_Price = RandomUtility.Random(m_Data.Price_Min, m_Data.Price_Max);
        }
    }
}

