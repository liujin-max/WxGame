using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;

namespace CB
{
    public class Obstacle : Box
    {
        private Rigidbody2D c_rigidbody;
        protected Collider2D c_collision;
        public Collider2D Collision{ get {return c_collision;}}


        public TextMeshPro c_CountText;
        private Transform c_Frame;
        private ImageGray m_ImageGray;

        public int Order;
        private bool m_ForceDead = false;
        private HashSet<int> m_ChangeRecords = new HashSet<int>();  //宝石转换记录

        private float m_HPShow = 0;


        public AttributeValue m_Scale = new AttributeValue(1f, false);
        private Tweener m_ScaleTweener;

        void Awake()
        {
            c_rigidbody = transform.GetComponent<Rigidbody2D>();
            c_Frame     = transform.Find("Frame").transform;

            c_collision = transform.GetComponent<Collider2D>();
            if (c_collision == null) {
                Debug.LogError(this.Order);
            }

            m_ImageGray = c_Frame.GetComponent<ImageGray>();


            this.Show(true);
        }

        void Start()
        {
            c_CountText.transform.eulerAngles = Vector3.zero;
        }

        public void Init(int order, int hp)
        {
            Order   = order;
            m_HP    = hp;

            m_ForceDead = false;
            m_HPShow    = hp;

            gameObject.layer = (int)_C.LAYER.DEFAULT;
            c_rigidbody.Sleep();
            c_CountText.gameObject.SetActive(true);
            m_ImageGray.TurnSpriteGray(false);

            var scale = m_Scale.ToNumber();
            transform.localScale = new Vector3(scale, scale, scale);

            Flush();
        }

        void LateUpdate()
        {
            c_CountText.transform.eulerAngles = Vector3.zero;

            if ((int)m_HPShow != m_HP)
            {
                float offset= m_HP - m_HPShow;
                float speed = Math.Min(offset / 0.2f, -20.0f);
                m_HPShow    += speed * Time.deltaTime;

                if (m_HPShow <= m_HP)
                {
                    m_HPShow = m_HP;
                }
                Flush();
            }
        }

        void Flush()
        {
            c_CountText.text = ((int)m_HPShow).ToString();
        }

        public void Show(bool flag)
        {
            c_Frame.gameObject.SetActive(flag);
        }

        public bool IsShow()
        {
            return c_Frame.gameObject.activeSelf;
        }

        public void ForceDead()
        {
            m_ForceDead = true;
            this.Dead();
        }

        public void CopyChanges(Obstacle obt)
        {
            foreach (var order in obt.m_ChangeRecords) {
                m_ChangeRecords.Add(order);
            }
        }

        public void AddChange(int order) {
            if (m_ChangeRecords.Contains(order) != true) {
                m_ChangeRecords.Add(order);
            }
        }

        public bool IsChangeFull()
        {
            return m_ChangeRecords.Count >= 4;
        }

        public void JudgeScale()
        {
            if (m_ScaleTweener != null) {
                m_ScaleTweener.Kill();
            }

            m_ScaleTweener = transform.DOScale(m_Scale.ToNumber(), 0.2f);
        }


        //碰撞
        public override void Crash(Vector2 force)
        {
            c_rigidbody.velocity = force;
        }

        public override void OnHit(Ball ball, int demage = 1)
        {
            if (m_Shield != null) {
                m_Shield.OnHit(ball, demage);
                return;
            }

            m_HP -= demage;

            Flush();
            Show(true);
            OnShake();

            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.ONOBSTACLEHIT, demage, ball));
        }


        public override void OnShake()
        {
            if (this.IsDead() == true) return;

            if (m_Shaking == true) {
                return;
            }

            m_Shaking = true;
            c_Frame.DOShakePosition(0.25f, 0.1f).OnComplete(() => {
                m_Shaking = false;
                c_Frame.localPosition = Vector3.zero;
            });
        }

        public override void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.transform.GetComponent<Ground>() != null)
            {
                c_rigidbody.Sleep();
                c_rigidbody.bodyType = RigidbodyType2D.Kinematic;
                GameFacade.Instance.Game.RecycleObstacle(this);
            }
        }

        public override void Dispose()
        {
            m_ChangeRecords.Clear();
            m_Scale.Clear();

            this.RemoveShield();
            
            gameObject.layer = (int)_C.LAYER.OBSTACLERECY;

            c_CountText.gameObject.SetActive(false);
            m_ImageGray.TurnSpriteGray(true);

            if (m_ForceDead == true) {
                GameFacade.Instance.Game.RecycleObstacle(this);
            } else {
                transform.DOShakePosition(0.2f, 0.2f).OnComplete(()=>{
                    c_rigidbody.WakeUp();
                    c_rigidbody.bodyType = RigidbodyType2D.Dynamic;
                    c_rigidbody.velocity = Vector2.down * 2;
                });
            }
        }
    }
}