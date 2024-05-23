using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Money
{
    /// <summary>
    /// 货品：仓库中拥有的
    /// </summary>
    public class Package
    {
        private GoodsData m_Data;


        private int m_Count;    //拥有数量
        private int m_Price;    //单个成本

        
        public Package(GoodsData data)
        {
            m_Data = data;
        }

        public void Increase(int price, int count)
        {
            int total = m_Count * m_Price + price * count;

            m_Count += count;
            m_Price = (int)MathF.Floor(total / m_Count);
        }

        public void Decrease(int price, int count)
        {
            m_Count -= count;
        }
    }
}

