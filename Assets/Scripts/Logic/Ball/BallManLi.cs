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
        private int m_Power = 5;
        private int m_Count;



        public override void UpgradeTo(int level)
        {
            base.UpgradeTo(level);

            Demage.PutADD(this, m_Power);
        }

        public override void Shoot(Vector3 pos)
        {
            base.Shoot(pos);

            Demage.PutADD(this, m_Power);
            m_Count = 0;
        }


        public override string GetDescription()
        {
            var str = string.Format("初始额外拥有<size=32><#43A600>{0}</color></size>点伤害，每次击中宝石后伤害降低1点", Demage.ToNumber());
            
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
                if (m_Power > m_Count) {
                    m_Count ++;
                    Demage.PutADD(this, Mathf.Max(0, m_Power - m_Count));
                }
            }
        }
    }
}