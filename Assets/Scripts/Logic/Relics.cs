using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UnityEngine;


namespace CB
{
    //装备
    public class Relics
    {
        private RelicsData m_Data;
        public int ID { get{return m_Data.ID;}}
        public string Name { get{return m_Data.Name;}}
        public int Price { get{return m_Data.Price;}}

        
        private List<BEffect> m_Effects = new List<BEffect>();


        public Relics(RelicsData data)
        {
            m_Data = data;

            //解析效果器
            string[] str_list = data.Effect.ToString().Split(",");
            foreach (var id in str_list)
            {
                var e = BEffect.Create(Convert.ToInt16(id));
                e.Belong = this;
                m_Effects.Add(e);
            }
        }

        public void Equip()
        {
            m_Effects.ForEach(e => {
                e.Execute();
            });
        }

        public void Unload()
        {
            m_Effects.ForEach(e => {
                e.Cancel();
            });
        }

        public List<BEffect> GetEffects()
        {
            return m_Effects;
        }

        public string GetDescription()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < m_Effects.Count; i++) {
                var e = m_Effects[i];
                sb.Append(e.GetDescription());
            }

            return sb.ToString();
        }

        public string ShowValue()
        {
            var e = m_Effects[0];
            return e.ShowString();
        }
    }
}

