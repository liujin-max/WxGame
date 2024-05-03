using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace CB
{
    public class Ball : MonoBehaviour
    {
        private SpriteRenderer c_sprite;

        protected Rigidbody2D c_rigidbody;
        public Rigidbody2D Rigidbody{ get {return c_rigidbody;}}

        protected Collider2D c_collision;
        public Collider2D Collision{ get {return c_collision;}}


        #region 属性
        protected _C.BALLTYPE m_Type;
        public _C.BALLTYPE Type{ get {return m_Type;}}

        public BallData Config;

        public string Name { get {return Config.Name ;}}


        protected int m_HP = 1;
        public int HP 
        { 
            get {return m_HP;}
            set {m_HP = value;}
        }

        protected int m_Level = 1;
        public int Level{ get {return m_Level;}}

        protected bool m_GroundValid = true;   //触发正常回收
        public bool IsGroundValid{ get {return m_GroundValid; }}

        private bool m_DeadFlag = false;
        public AttributeValue Demage = new AttributeValue(1);
        public AttributeValue m_Scale = new AttributeValue(1, false);

        //碰撞半径
        public float ColliderRadius
        {
            get 
            {
                var circle_collider = transform.GetComponent<CircleCollider2D>();
                if (circle_collider != null) {
                    return circle_collider.radius;
                }
                return 0.3f;
            }
        }

        private Vector3 m_LastPos;
        private Vector2 m_LastVelocity;
        public Vector2 LastVelocity{ get {return m_LastVelocity;}}

        [HideInInspector]public bool IsSimulate = false;

        private float m_FlyTime = 0;
        public float FlyTime { get {return m_FlyTime;}}

        public int SortingOrder{set {c_sprite.sortingOrder = value; }}
        //速度
        public Vector2 Velocity
        {
            get {
                return c_rigidbody.velocity; 
            }

            set { 
                c_rigidbody.velocity = value; 
            }
        }

        //准备就位
        public bool IsIdle
        {
            get {return gameObject.layer == (int)_C.LAYER.BALLIDLE;}
        }
        
        public bool IsRecycle
        {
            get {return gameObject.layer == (int)_C.LAYER.BALLRECYCLE;}
        }

        public bool IsActing
        {
            get {return gameObject.layer == (int)_C.LAYER.BALLACTING;}
        }

        //滚动方向
        public float RunningDirection
        {
            get {return transform.localPosition.x - m_LastPos.x;}
        }

        #endregion 属性



        void Awake()
        {
            c_rigidbody     = this.GetComponent<Rigidbody2D>();
            c_sprite        = transform.GetComponent<SpriteRenderer>();
            c_collision     = transform.GetComponent<Collider2D>();
            if (c_collision == null) {
                Debug.LogError(this.m_Type);
            }

            m_LastPos       = transform.localPosition;
            m_LastVelocity  = c_rigidbody.velocity;


            this.SetState((int)_C.LAYER.BALLIDLE);
        }

        public virtual void Init(_C.BALLTYPE type)
        {
            m_Type = type;
            Config = CONFIG.GetBallData(m_Type);

            this.UpgradeTo(1);
        }

        public virtual void Update()
        {
            if (this.IsActing)
            {
                m_FlyTime += Time.deltaTime;

                GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.ONBALLFLY, this));
            }
        }

        public virtual void LateUpdate()
        {
            m_LastPos       = transform.localPosition;
            m_LastVelocity  = c_rigidbody.velocity;
        }

        public void SetState(int state)
        {
            gameObject.layer = state;
        }

        public int GetState()
        {
            return gameObject.layer;
        }

        public virtual void Recyle()
        {
            if (this.IsRecycle == true) return;

            this.SetState((int)_C.LAYER.BALLRECYCLE);

            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.ONBALLRECYCLE, this));
        }

        public bool IsDead()
        {
            return m_DeadFlag || m_HP <= 0;
        }

        public void Dead()
        {
            m_DeadFlag = true;
        }

        public virtual void UpgradeTo(int level)
        {
            m_Level     = level;
            Demage.SetBase(level);
        }

        public void Show(bool flag)
        {
            gameObject.SetActive(flag);
        }

        public bool IsShow()
        {
            return gameObject.activeSelf;
        }

        public void Simulate(bool flag)
        {
            c_rigidbody.simulated = flag;
        }

        //准备
        public virtual void Breech()
        {
            this.SetState((int)_C.LAYER.BALLIDLE);
            this.Velocity = Vector2.zero;
            transform.localPosition = _C.BALL_ORIGIN_POS;
            
            c_rigidbody.bodyType = RigidbodyType2D.Kinematic;
        }

        //发射
        public virtual void Shoot(Vector3 pos)
        {
            //发射前取消小球和炮台的碰撞
            Physics2D.IgnoreCollision(c_collision, GameFacade.Instance.Game.c_borad, true);

            this.SetState((int)_C.LAYER.BALLACTING);
            c_rigidbody.bodyType = RigidbodyType2D.Dynamic;

            //放回原始位置
            c_rigidbody.SetRotation(0);
            c_rigidbody.transform.localPosition = _C.BALL_SHOOT_POS;
            c_rigidbody.velocity = Vector2.zero;

            m_FlyTime = 0;

            Vector2 force = pos - _C.BALL_SHOOT_POS;
            Vector2 normal= Vector3.Normalize(force);
            Vector2 vec     = normal * 700;

            c_rigidbody.AddForce(vec);

            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.ONBALLSHOOT, this, true));
        }

        //碰撞
        public void Crash(Vector2 force)
        {
            this.SetState((int)_C.LAYER.BALLACTING);

            c_rigidbody.velocity = force;

            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.ONBALLSHOOT, this, false));
        }

        public void AddForce(Vector2 force)
        {
            c_rigidbody.AddForce(force);
        }

        protected bool OnHitElement(Collision2D collision)
        {
            //排除宝石
            if (collision.gameObject.GetComponent<Obstacle>() != null) return false;

            Box box = collision.gameObject.GetComponent<Box>();
            if (box != null) {
                GameFacade.Instance.EffectManager.Load(EFFECT.BALLOON, collision.contacts[0].point);

                box.OnHit(this);
                box.OnShake();

                GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.ONBALLHITBOX, this, box, collision));

                return true;
            }

            return false;
        }

        protected bool OnHitObstable(Collision2D collision)
        {
            Obstacle obt = collision.gameObject.GetComponent<Obstacle>();
            if (obt != null) {
                GameFacade.Instance.EffectManager.Load(EFFECT.BALLOON, collision.contacts[0].point);

                GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.ONBALLHITBEFORE, this, obt, collision));

                obt.OnHit(this, (int)Demage.ToNumber());

                //暴击特效
                var rate = this.Demage.ShowRate();
                if (rate > 1) {
                    GameFacade.Instance.EffectManager.FlyRate(collision.contacts[0].point, rate);
                }

                GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.ONBALLHITAFTER, this, obt, collision));

                return true;
            }
            return false;
        }

        protected virtual void TriggerEnter(Collision2D collision)
        {
            //碰撞后取消无视
            Physics2D.IgnoreCollision(c_collision, GameFacade.Instance.Game.c_borad, false);

            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.ONENTERCOLLISION, this, collision));
        }


        //碰撞逻辑
        public virtual void OnCollisionEnter2D(Collision2D collision)
        {
            this.TriggerEnter(collision);
            this.OnHitElement(collision);
            this.OnHitObstable(collision);
        }

        public virtual void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.GetComponent<Box>() != null || collision.gameObject.GetComponent<Obstacle>() != null)
            {
                Vector2 direction = Quaternion.Euler(0, 0, RandomUtility.Random(0, 360)) * Vector2.right;
                this.Crash(direction * 16);
            }  
        }

        public virtual void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.GetComponent<Obstacle>() == null) return;

            //
            if (this.Velocity.magnitude <= 3f) {
                this.Velocity *= 5;

                // Debug.Log("长度：" + this.Velocity.magnitude);
            }
        }


        public virtual string GetDescription()
        {
            return "平平无奇的弹珠";
        }

        public virtual void Dispose()
        {
            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.ONBALLDEAD, this));

            Destroy(gameObject);
        }

    }

}