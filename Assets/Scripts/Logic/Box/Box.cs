using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace CB
{
    public class Box : MonoBehaviour
    {
        protected int m_HP = 1;
        public int HP { get { return m_HP; }}



        protected int m_HPMax = 1;

        //记录受击时的速度向量
        protected Vector2 HitVelocity;

        protected bool m_Shaking = false;
        protected bool m_Moving = false;

        protected Shield m_Shield;

        public bool IsIdle { get {return m_Moving != true;}}

        public void Move(Vector3 offset)
        {
            if (this.IsDead()) return;

            m_Moving = true;
            transform.DOLocalMove(transform.localPosition + offset, 0.5f).OnComplete(()=>{
                m_Moving = false;
            });
        }


        void FixedUpdate()
        {
            if (m_Shield != null)
            {
                if (m_Shield.IsDead())
                {
                    this.RemoveShield();
                }
            }
        }

        public virtual void OnCollisionEnter2D(Collision2D collision)
        {

        }

        public void DoScale()
        {
            var origin_sclae = transform.localScale;
            transform.localScale = Vector3.zero;
            transform.DOScale(origin_sclae * 1.3f , 0.2f).OnComplete(()=> {
                transform.DOScale(origin_sclae, 0.1f);
            });
        }

        public virtual void Dead()
        {
            m_HP = 0;
        }

        public virtual bool IsDead()
        {
            return m_HP <= 0;
        }

        //碰撞
        public virtual void Crash(Vector2 force)
        {

        }

        public virtual void OnHit(Ball ball, int demage = 1)
        {
            m_HP -= demage;
        }

        public virtual void OnShake()
        {
            
        }

        public virtual Shield AddShield(int hp)
        {
            if (m_Shield != null) {
                m_Shield.m_HP = hp;
                return m_Shield;
            }

            var item    = Instantiate(Resources.Load<GameObject>("Prefab/Box/Shield"), Vector3.zero, Quaternion.identity, transform);
            m_Shield    = item.GetComponent<Shield>();
            m_Shield.m_HP = hp;

            return m_Shield;
        }

        public virtual void RemoveShield()
        {
            if (m_Shield != null) {
                m_Shield.Dispose();
            }
            m_Shield = null;
        }

        public bool HasShield()
        {
            return m_Shield != null;
        }

        public virtual void Dispose()
        {
            Destroy(gameObject);
        }
    }
}