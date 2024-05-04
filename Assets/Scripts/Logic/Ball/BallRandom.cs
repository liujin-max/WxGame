using System.Collections;
using System.Collections.Generic;
using CB;
using UnityEngine;


namespace CB
{
    //变化弹珠
    public class BallRandom : Ball
    {
        private Ball m_RandomBall = null;

        public override void Init(_C.BALLTYPE type)
        {
            base.Init(type);

            EventManager.AddHandler(EVENT.ONENTERGROUND,    OnReponseEnterGround);
            EventManager.AddHandler(EVENT.ONBALLDEAD,       OnReponseBallDead);
        }

        void OnDestroy()
        {
            EventManager.DelHandler(EVENT.ONENTERGROUND,    OnReponseEnterGround);
            EventManager.DelHandler(EVENT.ONBALLDEAD,       OnReponseBallDead);
        }

        public override string GetDescription()
        {
            return string.Format("发射时随机变化成本回合已发射的任意一种弹珠。");
        }

        public override void Shoot(Vector3 pos)
        {
            base.Shoot(pos);

            if (GameFacade.Instance.Game.ShootCache.Count > 0)
            {
                //要排除掉他自己或同类型的
                //还要考虑发射过程中死亡的情况（黑洞弹珠）
                List<Ball> balls = new List<Ball>();
                foreach (var b in GameFacade.Instance.Game.ShootCache) {
                    if (b.Type == this.Type || b == this) continue;

                    balls.Add(b);
                }

                if (balls.Count > 0)
                {
                    this.Show(false);

                    var rand    = RandomUtility.Random(0, balls.Count);
                    var type    = balls[rand].Type;

                    m_RandomBall = GameFacade.Instance.Game.PushBall(transform.localPosition, type);
                    GameFacade.Instance.Game.BreechBall(m_RandomBall);

                    m_RandomBall.Shoot(pos);
                }
            }
        }

        void OnReponseEnterGround(GameEvent gameEvent)
        {
            if (m_RandomBall == null) return;

            var enter_ball = (Ball)gameEvent.GetParam(0);
            if (enter_ball == null) return;

            if (enter_ball == m_RandomBall) {
                if (enter_ball.IsDead() == true)
                {
                    this.Dead();
                }
                else
                {
                    this.HP = enter_ball.HP;
                    this.Recyle();
                    this.Show(true);
                    this.transform.localPosition = enter_ball.transform.localPosition;
                    this.Velocity = enter_ball.Velocity;
                    this.Rigidbody.rotation = enter_ball.Rigidbody.rotation;

                    enter_ball.Dead();
                }

                m_RandomBall = null;
            }
        }

        void OnReponseBallDead(GameEvent gameEvent)
        {
            if (m_RandomBall == null) return;
            var ball = (Ball)gameEvent.GetParam(0);
            if (ball == null) return;

            if (ball == m_RandomBall)
            {
                this.Dead();
            }
        }
    }
}

