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
        public int ID { get {return m_Data.ID;}}
        public string Name { get {return m_Data.Name;}}

        private int m_Count;    //拥有数量
        public int Count {get {return m_Count;}}

        private float m_Price;    //单个成本(负责计算)
        public float Price {get {return m_Price;}}   //负责显示

        
        public Package(GoodsData data)
        {
            m_Data = data;
        }

        public void Increase(int price, int count)
        {
            float total = m_Count * m_Price + price * count;

            m_Count += count;
            m_Price = total / (float)m_Count;
        }

        public void Decrease(int price, int count)
        {
            m_Count -= count;
        }
    }
}

