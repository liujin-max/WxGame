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

        public bool IsIdle { get {return m_Moving != true;}}

        public void Move(Vector3 offset)
        {
            if (this.IsDead()) return;

            m_Moving = true;
            transform.DOLocalMove(transform.localPosition + offset, 0.5f).OnComplete(()=>{
                m_Moving = false;
            });
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

        public virtual void Dispose()
        {
            Destroy(gameObject);
        }
    }
}