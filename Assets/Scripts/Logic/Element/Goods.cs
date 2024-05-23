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

        private int m_Price;    //当前价格

        public Goods(GoodsData data)
        {
            m_Data = data;

            m_Price= data.Price_Normal;
        }
    }
}

