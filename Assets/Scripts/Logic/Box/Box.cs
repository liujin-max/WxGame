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

        //碰撞半径
        public float ColliderRadius {
            get  {
                return 0.3f;
            }
        }

        protected bool m_Shaking = false;
        protected bool m_Moving = false;
        protected float m_MoveSPD = 2.0f;
        protected Vector3 m_ToPos;

        protected Shield m_Shield;


        public void Move(Vector3 to_pos, float speed = 2.0f)
        {
            if (this.IsDead()) return;

            m_Moving    = true;
            m_ToPos     = to_pos;
            m_MoveSPD   = speed;
        }

        public void Stop()
        {
            m_Moving    = false;
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

            if (m_Moving == true)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, m_ToPos, m_MoveSPD * Time.deltaTime);

                if (Vector3.Distance(transform.localPosition, m_ToPos) <= 0.05f) {
                    m_Moving = false;
                }

                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _C.OBSTACLE_OFFSET * 0.8f);
                if (colliders.Length > 1) {
                    m_Moving = false;
                }

            }
        }

        public virtual void OnCollisionEnter2D(Collision2D collision)
        {

        }

        public virtual void DoScale()
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
                m_Shield.DoDead();
            }
            m_Shield = null;
        }

        public bool HasShield()
        {
            return m_Shield != null;
        }

        public virtual void DoDead()
        {
            Destroy(gameObject);
        }

        public virtual void Dispose()
        {
            Destroy(gameObject);
        }
    }
}