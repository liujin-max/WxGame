using System;
using System.Collections;
using System.Collections.Generic;
using CB;
using Unity.VisualScripting;
using UnityEngine;

namespace CB
{
    /// <summary>
    /// 蛮力弹珠
    /// </summary>
    /// 
    public class BallManLi : Ball
    {
        private int m_Power;
        private int m_Count;



        public override void UpgradeTo(int level)
        {
            base.UpgradeTo(level);


            m_Power = 4 + m_Level * 2;
            m_Demage.SetBase(m_Power);
        }

        public override void Shoot(Vector3 pos)
        {
            base.Shoot(pos);

            m_Demage.Clear();
            m_Count = 0;
        }


        public override string GetDescription()
        {
            var str = string.Format("初始拥有<size=32><#43A600>{0}</color></size>点伤害，每次击中宝石后伤害降低1点", m_Demage.ToNumber());
            
            return str;
        }

        //碰撞逻辑
        public override void OnCollisionEnter2D(Collision2D collision)
        {
            this.TriggerEnter(collision);
            this.OnHitElement(collision);
            this.OnHitObstable(collision);

            Obstacle obt = collision.transform.GetComponent<Obstacle>();
            if (obt != null) {
                if (m_Demage.ToNumber() > 1) {
                    m_Count ++;
                    m_Demage.PutADD(this, -m_Count);
                }
            }
        }
    }
}