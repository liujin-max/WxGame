using System;
using System.Collections;
using System.Collections.Generic;
using CB;
using Unity.Mathematics;
using UnityEngine;



namespace CB
{
    //瞄准线可以显示反弹的路线
    public class BEffect_AIM : BEffect
    {
        public BEffect_AIM() {}

        public override string GetDescription()
        {
            return "瞄准线可以显示初次反弹的轨迹。";
        }

        public override void Execute()
        {
            GameFacade.Instance.Game.Simulator.FocusCount = 2;
        }

        public override void Cancel()
        {
            GameFacade.Instance.Game.Simulator.FocusCount = 1;
        }
    }


    //弹珠连续击中相同宝石，伤害提升
    public class BEffect_CONTINUE : BEffect
    {
        //int[] => 上次击中的宝石, 连续击中的数量
        private Dictionary<Ball, object[]> m_Records = new Dictionary<Ball, object[]>();
        
        public BEffect_CONTINUE() {}

        public override string GetDescription()
        {
            return "每当弹珠连续击中相同类型的宝石时，伤害提高1倍。(连续击中同一颗宝石时，不纳入计算)";
        }


        public override void OnBallShoot(Ball ball, bool is_real_shoot)
        {
            if (ball.IsSimulate == true) return;

            m_Records[ball] = new object[] {null, 0};
        }

        public override void OnHitBefore(Ball ball, Obstacle obt, Collision2D collision)
        {
            object[] objs;
            if (m_Records.TryGetValue(ball, out objs))
            {
                if (objs[0] == null) //意味着之前没击中过
                {
                    objs[1] = 0;
                }
                else
                {
                    if (obt == (Obstacle)objs[0]){  //连续击中同一个宝石，不算
                        return;
                    }
                    
                    if (((Obstacle)objs[0]).Order == obt.Order)   //连续击中
                    {
                        objs[1] = (int)objs[1] + 1;
                    }
                    else    //中断
                    {
                        objs[1] = 0;
                    }
                }

                objs[0] = obt;

                if((int)objs[1] > 0) {
                    ball.Demage.PutAUL(this, (int)objs[1]);

                    GameFacade.Instance.EffectManager.FlyRate(collision.contacts[0].point, (int)objs[1] + 1);

                    GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));
                }
                
            }
        }

        public override void OnHitAfter(Ball ball, Obstacle obt, Collision2D collision)
        {
            ball.Demage.Pop(this);
        }
    }


    //每拥有2枚碎片, 高级弹珠的伤害就增加1点
    public class BEffect_GROW : BEffect
    {
        public BEffect_GROW() {}
        
        public override string GetDescription()
        {
            return string.Format("每拥有2枚<sprite={0}>,高阶弹珠的伤害增加1点。", (int)_C.SPRITEATLAS.GLASS);
        }

        public override string ShowString()
        {
            return ((int)(GameFacade.Instance.Game.Glass * 0.5f)).ToString();
        }

        public override void Execute()
        {
            GameFacade.Instance.Game.Balls.ForEach(b => {
                if (b.Type != _C.BALLTYPE.NORMAL && b.Type != _C.BALLTYPE.SMALL) {
                    b.Demage.PutADD(this, (int)GameFacade.Instance.Game.Glass * 0.5f);
                }
            });
        }

        public override void Cancel()
        {
            GameFacade.Instance.Game.Balls.ForEach(b => {
                b.Demage.Pop(this);
            });
        }

        public override void OnGlassUpdate(int count)
        {
            GameFacade.Instance.Game.Balls.ForEach(b => {
                if (b.Type != _C.BALLTYPE.NORMAL && b.Type != _C.BALLTYPE.SMALL) {
                    b.Demage.PutADD(this, (int)GameFacade.Instance.Game.Glass * 0.5f);
                }
            });
        }

        public override void OnPushBall(Ball ball)
        {
            if (ball.Type == _C.BALLTYPE.NORMAL || ball.Type == _C.BALLTYPE.SMALL) return;

            ball.Demage.PutADD(this, (int)GameFacade.Instance.Game.Glass * 0.5f);
        }
    }


    //场上每存在一颗三角宝石,所有弹珠的伤害提高#倍
    public class BEffect_SANJIAO : BEffect
    {
        public BEffect_SANJIAO() {}

        public override string GetDescription()
        {
            return string.Format("场上每存在一颗<sprite={0}>,所有弹珠的伤害提高0.2倍。", (int)_C.SPRITEATLAS.SANJIAO);
        }

        public override string ShowString()
        {
            int count = 0;
            GameFacade.Instance.Game.Obstacles.ForEach(o => {
                if (o.Order == 1) {
                   count++; 
                }
            });

            return (count * 0.2f).ToString();
        }

        public override void OnHitBefore(Ball ball, Obstacle obt, Collision2D collision)
        {
            int count = 0;
            GameFacade.Instance.Game.Obstacles.ForEach(o => {
                if (o.Order == 1) {
                   count++; 
                }
            });

            if (count > 0) {
                ball.Demage.PutAUL(this, count * 0.2f);
            }
        }

        public override void OnHitAfter(Ball ball, Obstacle obt, Collision2D collision)
        {
            ball.Demage.Pop(this);
        }
    }


    //结算时,场上每颗圆形宝石可转化成1枚金币
    public class BEffect_COIN : BEffect
    {
        public BEffect_COIN() {}

        public override string GetDescription()
        {
            return string.Format("结算时，场上每颗<sprite={0}>转化成1枚<sprite={1}>。", (int)_C.SPRITEATLAS.YUAN, (int)_C.SPRITEATLAS.COIN);
        }

        public override string ShowString()
        {
            int count = 0;
            GameFacade.Instance.Game.Obstacles.ForEach(o => {
                if (o.Order == 3) {
                   count++; 
                }
            });

            return count.ToString();
        }

        public override void OnPlayEnd()
        {
            int count = 0;
            GameFacade.Instance.Game.Obstacles.ForEach(o => {
                if (o.Order == 3) {
                    count++; 
                   
                    var effect = GameFacade.Instance.EffectManager.Load(EFFECT.FLYCOIN, o.transform.position);
                    effect.GetComponent<FlyCoin>().Fly((count - 1) * 0.1f); 
                }
            });

            if (count > 0) {
                GameFacade.Instance.Game.UpdateCoin(count);

                GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));
            }
        }
    }

    //每次击中方形宝石，弹珠的弹力提高1点
    public class BEffect_FORCE : BEffect
    {
        public BEffect_FORCE() {}

        public override string GetDescription()
        {
            return string.Format("<sprite={0}>的弹力变大。", (int)_C.SPRITEATLAS.FANG);
        }

        public override void OnHitAfter(Ball ball, Obstacle obt, Collision2D collision)
        {
            if (obt.Order == 0)
            {
                ball.Velocity = ball.Velocity.normalized * (ball.Velocity.magnitude + 3);

                GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));
            }
        }
    }

    //升级弹珠花费的<sprite=0>数量-1
    public class BEffect_UPGRADE : BEffect
    {
        public BEffect_UPGRADE() {}

        public override string GetDescription()
        {
            return "合成弹珠花费的<sprite=0>数量有概率-1。";
        }

        public override void OnComplextInit(ComplextEvent evt)
        {
            if (evt.EventType == _C.COMPLEXTEVEMT.NEW) {
                if (RandomUtility.IsHit(50) == true) {
                    if (evt.Cost.ToNumber() > 1) {
                        evt.Cost.PutADD(this, -1);

                        GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));
                    }
                }
            }
        }
    }



    //所有弹珠的基础伤害提高1点
    public class BEffect_ATTACK : BEffect
    {
        public BEffect_ATTACK() {}

        public override string GetDescription()
        {
            return "所有弹珠的基础伤害提高1点。";
        }

        public override void Execute()
        {
            GameFacade.Instance.Game.Balls.ForEach(b => {
                b.Demage.PutADD(this, 1);
            });
        }

        public override void Cancel()
        {
            GameFacade.Instance.Game.Balls.ForEach(b => {
                b.Demage.Pop(this);
            });
        }

        public override void OnPushBall(Ball ball)
        {
            ball.Demage.PutADD(this, 1);
        }
    }


    //击落碎片时有概率额外获得一枚
    public class BEffect_DRAWBALL : BEffect
    {
        public BEffect_DRAWBALL() {}
        public override string GetDescription()
        {
            return "击落<sprite=0>时有概率额外获得一枚。";
        }

        public override void OnHitBox(Ball ball, Box g, Collision2D collision)
        {
            var ghost = g.GetComponent<Ghost>();
            if (ghost == null) return;
            if (ghost.IsDead() == false) return;

            if (RandomUtility.IsHit(40) == true)
            {
                GameFacade.Instance.Game.Glass += 1;

                GameFacade.Instance.EffectManager.Load(EFFECT.FLYGLASS, ball.transform.localPosition);

                GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));
            }
        }
    }

    //之后的第#次碰撞，弹珠造成的伤害提高4倍
    public class BEffect_HITRATE : BEffect
    {
        private int m_Count;
        private const int m_CountMax = 30;
        public BEffect_HITRATE() 
        {
            m_Count = m_CountMax;
        }

        public override string GetDescription()
        {
            return string.Format("之后的第{0}次碰撞，弹珠造成的伤害X4。", m_Count);
        }

        public override string ShowString()
        {
            return m_Count.ToString();
        }

        public override void OnHitBefore(Ball ball, Obstacle obt, Collision2D collision)
        {
            m_Count--;
            if (m_Count == 0) {
                ball.Demage.PutAUL(this, 3);

                GameFacade.Instance.EffectManager.FlyRate(collision.contacts[0].point, 4);

                GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));
            }
        }

        public override void OnHitAfter(Ball ball, Obstacle obt, Collision2D collision)
        {
            if (m_Count == 0) {
                ball.Demage.Pop(this);

                m_Count = m_CountMax;
            }
        }
    }


    //每拥有一枚碎片，普通弹珠的伤害就增加1点
    public class BEffect_BLIZARD : BEffect
    {
        public BEffect_BLIZARD() {}
        
        public override string GetDescription()
        {
            return string.Format("每拥有一枚<sprite={0}>,<sprite={1}>的伤害增加1点。", (int)_C.SPRITEATLAS.GLASS, (int)_C.SPRITEATLAS.BALL);
        }

        public override string ShowString()
        {
            return GameFacade.Instance.Game.Glass.ToString();
        }

        public override void Execute()
        {
            GameFacade.Instance.Game.Balls.ForEach(b => {
                if (b.Type == _C.BALLTYPE.NORMAL) {
                    b.Demage.PutADD(this, GameFacade.Instance.Game.Glass);
                }
            });
        }

        public override void Cancel()
        {
            GameFacade.Instance.Game.Balls.ForEach(b => {
                b.Demage.Pop(this);
            });
        }

        public override void OnGlassUpdate(int count)
        {
            GameFacade.Instance.Game.Balls.ForEach(b => {
                if (b.Type == _C.BALLTYPE.NORMAL) {
                    b.Demage.PutADD(this, GameFacade.Instance.Game.Glass);
                }
            });
        }

        public override void OnPushBall(Ball ball)
        {
            if (ball.Type != _C.BALLTYPE.NORMAL) return;

            ball.Demage.PutADD(this, GameFacade.Instance.Game.Glass);
        }
    }


    //可以额外携带一颗弹珠
    public class BEffect_SLOT : BEffect
    {
        public BEffect_SLOT() {}
        
        public override string GetDescription()
        {
            return "可以额外携带一颗弹珠";
        }

        public override void Execute()
        {
            GameFacade.Instance.Game.SeatCount.PutADD(this, 1);

            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHBALLS));
        }

        public override void Cancel()
        {
            GameFacade.Instance.Game.SeatCount.Pop(this);

            if (GameFacade.Instance.Game.Balls.Count > GameFacade.Instance.Game.SeatCount.ToNumber())
            {
                var ball = GameFacade.Instance.Game.Balls[GameFacade.Instance.Game.Balls.Count - 1];

                ball.Dispose();
                GameFacade.Instance.Game.Balls.Remove(ball);

                GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHBALLS));
            }
        }
    }

    //拥有的弹珠越少，造成的伤害倍率越高(当前：X5)
    public class BEffect_TIECHUI : BEffect
    {
        private int Rate{
            get {
                var count = GameFacade.Instance.Game.Balls.Count;

                return Math.Max((int)(GameFacade.Instance.Game.SeatCount.ToNumber() + 1 - count), 1);
            }
        }

        public BEffect_TIECHUI() {}

        //1个弹珠 5倍
        //2个弹珠 4倍
        //3个弹珠 3倍
        //5个弹珠 1倍

        public override string GetDescription()
        {
            return string.Format("进入下一层时拥有的弹珠越少，弹珠伤害倍率越高(当前：X{0})", Rate);
        }

        public override string ShowString()
        {
            return string.Format("X{0}", Rate);
        }

        
        public override void OnPlayStart()
        {
            GameFacade.Instance.Game.Balls.ForEach(b => {
                b.Demage.PutAUL(this, Rate - 1);
            });
        }

        public override void OnPlayEnd()
        {
            GameFacade.Instance.Game.Balls.ForEach(b => {
                b.Demage.Pop(this);
            });
        }

    }

    //每拥有50金币，弹珠的伤害提高1点(当前：2)
    public class BEffect_WALLET : BEffect
    {
        private int Count{
            get {
                var count = GameFacade.Instance.Game.m_Coin / 50;

                return count;
            }
        }

        public BEffect_WALLET() {}

        public override string GetDescription()
        {
            return string.Format("每拥有50<sprite=1>，弹珠的伤害提高1点(当前：{0})", Count);
        }

        public override string ShowString()
        {
            return Count.ToString();
        }

        public override void Execute()
        {
            GameFacade.Instance.Game.Balls.ForEach(b => {
                b.Demage.PutADD(this, Count);
            });
        }

        public override void Cancel()
        {
            GameFacade.Instance.Game.Balls.ForEach(b => {
                b.Demage.Pop(this);
            });
        }

        public override void OnPushBall(Ball ball)
        {
            GameFacade.Instance.Game.Balls.ForEach(b => {
                b.Demage.PutADD(this, Count);
            });
        }

        public override void OnCoinUpdate(int coin)
        {
            GameFacade.Instance.Game.Balls.ForEach(b => {
                b.Demage.PutADD(this, Count);
            });
        }
    }

    //击中#次菱形宝石后，下一次弹珠撞击造成双倍伤害
    public class BEffect_ZHUSHE : BEffect
    {
        private const int m_CountMax  = 5;
        private int m_Count;

        public BEffect_ZHUSHE() 
        {
            m_Count = m_CountMax;
        }

        public override string GetDescription()
        {
            return string.Format("每击中<sprite=5>{0}次，下一次弹珠撞击造成双倍伤害", m_CountMax);
        }

        public override string ShowString()
        {
            return m_Count.ToString();
        }

        public override void OnHitBefore(Ball ball, Obstacle obt, Collision2D collision)
        {
            if (obt.Order == 2) {
                m_Count--;
                if (m_Count == 0) {
                    GameFacade.Instance.EffectManager.FlyRate(collision.contacts[0].point, 2);

                    GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));
                }
            }
        }

        public override void OnHitAfter(Ball ball, Obstacle obt, Collision2D collision)
        {
            if (m_Count == 0) {
                m_Count = m_CountMax;

                ball.Demage.Pop(this);
            }
        }

        
    }


    //发射的弹珠有小概率会被再次回收。
    public class BEffect_REPEAT : BEffect
    {
        public BEffect_REPEAT() {}

        public override string GetDescription()
        {
            return string.Format("发射的弹珠有小概率会被再次回收。");
        }

        public override void OnBallShoot(Ball ball, bool is_real_shoot)
        {
            if (ball.IsSimulate == true) return;
            if (is_real_shoot == false) return;

            if (RandomUtility.IsHit(15) == true)
            {
                ball.HP += 1;

                GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));
            }
        }
    }


    //弹珠可能发生暴击，造成的伤害X2
    public class BEffect_CRIT : BEffect
    {
        public BEffect_CRIT() {}

        public override string GetDescription()
        {
            return string.Format("弹珠可能发生暴击，造成双倍伤害。");
        }

        public override void OnHitBefore(Ball ball, Obstacle obt, Collision2D collision)
        {
            if (RandomUtility.IsHit(20) == true) {
                ball.Demage.PutMUL(this, 2);

                var pos = collision.contacts[0].point;
                GameFacade.Instance.EffectManager.Load(EFFECT.CRIT, pos);
                GameFacade.Instance.EffectManager.FlyRate(pos, 2);

                GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));
            }  
        }

        public override void OnHitAfter(Ball ball, Obstacle obt, Collision2D collision)
        {
            ball.Demage.Pop(this);
        }
    }

    //撞击墙面也可以获得分数
    public class BEffect_WALLSCORE : BEffect
    {
        public BEffect_WALLSCORE() {}

        public override string GetDescription()
        {
            return string.Format("弹珠撞击墙壁也可以获得分数。");
        }

        public override void OnEnterCollision(Ball ball, Collision2D collision)
        {
            if (ball.IsSimulate == true) return;

            if (collision.transform.GetComponent<Wall>() != null)
            {
                GameFacade.Instance.Game.UpdateScore(1);

                GameFacade.Instance.EffectManager.Load(EFFECT.BALLOON, collision.contacts[0].point);
                GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));
            }
        }
    }

    //每击落#枚碎片时可产生一颗炸弹
    public class BEffect_PIECEBOMB : BEffect
    {
        private const int m_CountMax = 5;
        private int m_Count;

        public BEffect_PIECEBOMB() 
        {
            m_Count = 0;
        }

        public override string GetDescription()
        {
            return string.Format("每击落{0}枚<sprite={1}>在原地留下一颗<sprite={2}>。", m_CountMax, (int)_C.SPRITEATLAS.GLASS, (int)_C.SPRITEATLAS.BOMB);
        }

        public override string ShowString()
        {
            return m_Count.ToString();
        }

        public override void OnHitBox(Ball ball, Box g, Collision2D collision)
        {
            var ghost = g.GetComponent<Ghost>();
            if (ghost == null) return;
            if (ghost.IsDead() == false) return;

            m_Count++;
            if (m_Count >= m_CountMax)
            {
                m_Count = 0;

                GameFacade.Instance.Game.PushBomb(collision.transform.localPosition);

                GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));
            }
        }
    }


    //炸弹的血量-1
    public class BEffect_BOMBHP : BEffect
    {
        public override string GetDescription()
        {
            return string.Format("击中<sprite={0}>时有概率直接引爆。", (int)_C.SPRITEATLAS.BOMB);
        }

        public override void OnHitBox(Ball ball, Box g, Collision2D collision)
        {
            var bomb = g.GetComponent<Bomb>();
            if (bomb != null) {
                if (RandomUtility.IsHit(50) == true) {
                    GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));

                    bomb.Dead();
                }
            }
        }
    }

    //弹珠每击中宝石#次立即获得1点积分
    public class BEffect_HITSCORE : BEffect
    {
        public BEffect_HITSCORE() 
        {

        }

        public override string GetDescription()
        {
            return string.Format("弹珠每次击中宝石额外获得1点分数。");
        }


        public override void OnHitAfter(Ball ball, Obstacle obt, Collision2D collision)
        {
            GameFacade.Instance.Game.UpdateScore(1);

            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));
        }
    }


    //撞击墙壁时总能朝高处反弹
    public class BEffect_WALLREFLEX : BEffect
    {
        public BEffect_WALLREFLEX() {}

        public override string GetDescription()
        {
            return string.Format("弹珠撞击墙壁时总能朝高处反弹。");
        }

        public override void OnEnterCollision(Ball ball, Collision2D collision)
        {
            // if (ball.IsSimulate == true) return;

            if (collision.transform.GetComponent<Wall>() != null)
            {
                // ball.Velocity = new Vector2(ball.Velocity.x, Math.Abs(ball.Velocity.y));
                ball.Velocity = new Vector2(ball.Velocity.x / Math.Abs(ball.Velocity.x) * Math.Abs(ball.Velocity.y), Math.Abs(ball.Velocity.x));

                if (ball.IsSimulate != true) {
                    GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));
                }
            }
        }
    }


    //撞击墙壁时反弹的更远
    public class BEffect_WALLFORCE : BEffect
    {
        public BEffect_WALLFORCE() {}

        public override string GetDescription()
        {
            return string.Format("弹珠撞击墙壁时反弹的更远。");
        }

        public override void OnEnterCollision(Ball ball, Collision2D collision)
        {
            // if (ball.IsSimulate == true) return;

            if (collision.transform.GetComponent<Wall>() != null)
            {
                var length = Math.Max(1, ball.Velocity.magnitude);
                ball.Velocity = ball.Velocity.normalized * length * 1.1f;

                if (ball.IsSimulate != true) {
                    GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));
                }
            }
        }
    }

    //弹珠飞行时间越长，造成的伤害越高
    //每飞行5秒，伤害提高1
    public class BEffect_FLY : BEffect
    {
        private HashSet<Ball> m_Balls = new HashSet<Ball>();

        public BEffect_FLY() {}

        public override string GetDescription()
        {
            return string.Format("弹珠飞行时间越长，造成的伤害越高。");
        }

        public override void OnHitBefore(Ball ball, Obstacle obt, Collision2D collision)
        {
            var value = (int)(ball.FlyTime / 5.0f);
            if (value > 0) {
                ball.Demage.PutADD(this, value);

                GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));
            }
        }

        public override void OnHitAfter(Ball ball, Obstacle obt, Collision2D collision)
        {
            ball.Demage.Pop(this);
        }
    }


    //TNT
    public class BEffect_TNT : BEffect
    {
        public override string GetDescription()
        {
            return string.Format(" <sprite={0}>有概率造成双倍伤害。", (int)_C.SPRITEATLAS.BOMB);
        }

        public override void OnBombBefore(Bomb bomb)
        {
            if (RandomUtility.IsHit(55) == true)
            {
                bomb.Demage.PutAUL(this, 1);

                GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));
            }
        }
    }

    //
    public class BEffect_BOMBRANGE : BEffect
    {
        public override string GetDescription()
        {
            return string.Format(" <sprite={0}>的爆炸范围扩大20%。", (int)_C.SPRITEATLAS.BOMB);
        }

        public override void OnBombBefore(Bomb bomb)
        {
            bomb.Radius.PutAUL(this, 0.2f);

            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));
        }
    }

    //击落碎片时在原地留下一颗宝石
    public class BEffect_PIECEREGEN : BEffect
    {
        public override string GetDescription()
        {
            return string.Format("击落<sprite={0}>时在原地留下一颗宝石。", (int)_C.SPRITEATLAS.GLASS);
        }

        public override void OnHitBox(Ball ball, Box g, Collision2D collision)
        {
            var ghost = g.GetComponent<Ghost>();
            if (ghost != null && ghost.IsDead() == true)
            {
                var o = GameFacade.Instance.Game.PushObstacle(collision.transform.localPosition, 1);
                o.DoScale();

                GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));
            }
        }
    }

    //场上必定出现炸弹。
    public class BEffect_BOMBRATE : BEffect
    {
        public override string GetDescription()
        {
            return string.Format("场上必定出现<sprite={0}>。", (int)_C.SPRITEATLAS.BOMB);
        }

        public override void OnDrawingObstacles(List<int> lists, AttributeValue draw_count)
        {
            bool is_exist = false;
            for (int i = lists.Count - 1; i >= 0; i--) {
                if (lists[i] == (int)_C.BOXTYPE.BOMB) {
                    is_exist = true;
                    break;
                }
            }

            if (is_exist == false) {
                lists.Add((int)_C.BOXTYPE.BOMB);

                GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));
            }
        }
    }


    //场上将出现更多的宝石
    public class BEffect_GEMMORE : BEffect
    {
        public override string GetDescription()
        {
            return string.Format("场上将出现更多的宝石。");
        }

        public override void OnDrawingObstacles(List<int> lists, AttributeValue draw_count)
        {
            draw_count.PutADD(this, 3);

            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));
        }
    }


    //回合结束时获得1金币
    public class BEffect_TURNCOIN : BEffect
    {
        public override string GetDescription()
        {
            return string.Format("回合结束时获得1<sprite={0}>。", (int)_C.SPRITEATLAS.COIN);
        }

        public override void OnPlayEnd()
        {
            GameFacade.Instance.Game.UpdateCoin(1);

            var item = GameFacade.Instance.Game.GameUI.GetRelicsSeat(Belong);
            if (item != null) {
                var e = GameFacade.Instance.EffectManager.LoadUIEffect(EFFECT.FLYCOIN, item.transform.position);
                e.GetComponent<FlyCoin>().Fly(0, false); 
            }

            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));
        }

    }

    //弹珠发射时，有概率临时提高1点伤害
    public class BEffect_TEMPDEMAGE : BEffect
    {
        public override string GetDescription()
        {
            return string.Format("弹珠发射时有概率临时提高1点伤害。");
        }


        public override void OnBallShoot(Ball ball, bool is_real_shoot)
        {
            if (is_real_shoot != true) return;
            if (ball.IsSimulate == true) return;

            if (RandomUtility.IsHit(30))
            {
                ball.Demage.PutADD(this, 1);

                var seat_item = GameFacade.Instance.Game.GameUI.GetBallSeat(ball);
                if (seat_item != null) {
                    seat_item.ShowFadeScale();
                }

                GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));
            }
        }

        public override void OnPlayEnd()
        {
            GameFacade.Instance.Game.Balls.ForEach(b => {
                b.Demage.Pop(this);
            });
        }
    }

    //结算时获得双倍的金币，但场上不会再出现碎片
    public class BEffect_DOUBLECOIN : BEffect
    {
        public override string GetDescription()
        {
            return string.Format("结算时获得双倍<sprite={0}>，但场上不会再出现<sprite={1}>。", (int)_C.SPRITEATLAS.COIN, (int)_C.SPRITEATLAS.GLASS);
        }

        //将碎片替换成宝石
        public override void OnDrawingObstacles(List<int> lists, AttributeValue draw_count)
        {
            for (int i = lists.Count - 1; i >= 0; i--) {
                if (lists[i] == (int)_C.BOXTYPE.GHOST) {
                    lists.RemoveAt(i);
                }
            }

            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));
        }

        public override void OnWillReceiveCoin(AttributeValue coin_number)
        {
            coin_number.PutAUL(this, 1);

            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));
        }
    }


    //每刷新一次弹珠列表，使弹珠的伤害倍率提高0.1倍
    public class BEffect_REFRESHBALL : BEffect
    {
        private int m_Count = 0;
        public override string GetDescription()
        {
            return string.Format("每次刷新合成列表，弹珠的伤害倍率提高0.1倍。(当前：X{0})", 0.1f * m_Count);
        }

        public override string ShowString()
        {
            return (0.1f * m_Count).ToString();
        }

        public override void OnRefreshEvents(List<ComplextEvent> events, bool is_video_play)
        {
            m_Count++;

            GameFacade.Instance.Game.Balls.ForEach(b => {
                b.Demage.PutAUL(this, 0.1f * m_Count);
            });

            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));
        }

        public override void OnPushBall(Ball ball)
        {
            ball.Demage.PutAUL(this, 0.1f * m_Count);
        }

        public override void OnBallShoot(Ball ball, bool is_real_shoot)
        {
            ball.Demage.PutAUL(this, 0.1f * m_Count);
        }
    }

    //弹珠槽+3，回合结束时-1
    public class BEffect_TEMPSEAT : BEffect
    {
        private int m_Max   = 3;
        private int m_Count = 3;
        public override string GetDescription()
        {
            return string.Format("可以额外携带{0}颗弹珠，回合结束时减少1颗。(当前：{1})", m_Max, m_Count);
        }

        public override string ShowString()
        {
            return m_Count.ToString();
        }

        public override void Execute()
        {
            GameFacade.Instance.Game.SeatCount.PutADD(this, m_Count);

            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHBALLS));
            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));
        }

        public override void OnPlayEnd()
        {
            if (m_Count == 0) return;

            m_Count--;
            GameFacade.Instance.Game.SeatCount.PutADD(this, m_Count);

            if (GameFacade.Instance.Game.Balls.Count > GameFacade.Instance.Game.SeatCount.ToNumber())
            {
                var ball = GameFacade.Instance.Game.Balls[GameFacade.Instance.Game.Balls.Count - 1];

                ball.Dispose();
                GameFacade.Instance.Game.Balls.Remove(ball);
            }

            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHBALLS));
            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));
        }
    }

    //回合结束时留在弹珠槽里的每颗弹珠有概率给予1金币
    public class BEffect_CHECKBALLCOIN : BEffect
    {
        public override string GetDescription()
        {
            return string.Format("回合结束时留在弹珠槽里的每颗弹珠有概率给予1<sprite={0}>", (int)_C.SPRITEATLAS.COIN);
        }

        public override void OnPlayEnd()
        {
            GameFacade.Instance.Game.Balls.ForEach(ball => {
                if (RandomUtility.IsHit(50)) 
                {
                    GameFacade.Instance.Game.UpdateCoin(1);

                    var item = GameFacade.Instance.Game.GameUI.GetBallSeat(ball);
                    if (item != null) {
                        var e = GameFacade.Instance.EffectManager.LoadUIEffect(EFFECT.FLYCOIN, item.transform.position);
                        e.GetComponent<FlyCoin>().Fly(0, false); 
                    }
                }
            });

            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));
        }
    }

    //碎片的售价有概率-1
    public class BEffect_GLASSCOST : BEffect
    {
        public override string GetDescription()
        {
            return string.Format("购买<sprite={0}>花费的<sprite={1}>有概率-1。", (int)_C.SPRITEATLAS.GLASS, (int)_C.SPRITEATLAS.COIN);
        }

        public override void OnComplextInit(ComplextEvent evt)
        {
            if (evt.EventType == _C.COMPLEXTEVEMT.GLASS) {
                if (RandomUtility.IsHit(50) == true) {
                    if (evt.Cost.ToNumber() > 1) {
                        evt.Cost.PutADD(this, -1);

                        GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));
                    }
                }
            }
        }
    }

    //刷新的价格-1
    public class BEffect_REFRESHCOST : BEffect
    {
        public override string GetDescription()
        {
            return string.Format("刷新弹珠花费的<sprite={0}>-1。",  (int)_C.SPRITEATLAS.COIN);
        }

        public override void Execute()
        {
            GameFacade.Instance.Game.RefreshCoin.PutADD(this, -1);

            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));
        }

        public override void Cancel()
        {
            GameFacade.Instance.Game.RefreshCoin.Pop(this);
        }

        public override void OnRefreshEvents(List<ComplextEvent> events, bool is_video_play)
        {
            if (is_video_play) return;
            
            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));
        }
    }

    //提高商店出售碎片的几率
    public class BEffect_GLASSRATE : BEffect
    {
        public override string GetDescription()
        {
            return string.Format("提高合成列表出现<sprite={0}>的几率。",  (int)_C.SPRITEATLAS.GLASS);
        }

        public override void Execute()
        {
            GameFacade.Instance.Game.GlassRate.PutADD(this, 10);
        }

        public override void Cancel()
        {
            GameFacade.Instance.Game.GlassRate.Pop(this);
        }
    }



    public class BEffect
    {
        public int ID;
        public Relics Belong;

        public static BEffect Create(int id)
        {
            switch (id)
            {
                case 1000:
                    return new BEffect_AIM();

                case 1001:
                    return new BEffect_CONTINUE();

                case 1002:
                    return new BEffect_GROW(); 

                case 1003:
                    return new BEffect_SANJIAO();

                case 1004:
                    return new BEffect_COIN();

                case 1005:
                    return new BEffect_UPGRADE();

                case 1006:
                    return new BEffect_FORCE();

                case 1007:
                    return new BEffect_ATTACK();

                case 1008:
                    return new BEffect_DRAWBALL();

                case 1009:
                    return new BEffect_HITRATE();

                case 1010:
                    return new BEffect_BLIZARD();

                case 1011:
                    return new BEffect_SLOT();

                case 1012:
                    return new BEffect_TIECHUI();

                case 1013:
                    return new BEffect_WALLET();

                case 1014:
                    return new BEffect_ZHUSHE();


                case 1016:
                    return new BEffect_REPEAT();

                case 1017:
                    return new BEffect_CRIT();

                case 1018:
                    return new BEffect_WALLSCORE();

                case 1019:
                    return new BEffect_PIECEBOMB();

                case 1020:
                    return new BEffect_BOMBHP();

                case 1021:
                    return new BEffect_HITSCORE();

                case 1022:
                    return new BEffect_WALLREFLEX();

                case 1023:
                    return new BEffect_WALLFORCE();

                case 1024:
                    return new BEffect_FLY();

                case 1025:
                    return new BEffect_TNT();

                case 1026:
                    return new BEffect_BOMBRANGE();

                case 1027:
                    return new BEffect_PIECEREGEN();

                case 1028:
                    return new BEffect_BOMBRATE();

                case 1029:
                    return new BEffect_GEMMORE();

                case 1031:
                    return new BEffect_TURNCOIN();

                case 1032:
                    return new BEffect_TEMPDEMAGE();

                case 1033:
                    return new BEffect_DOUBLECOIN();

                case 1034:
                    return new BEffect_REFRESHBALL();

                case 1035:
                    return new BEffect_TEMPSEAT();

                case 1036:
                    return new BEffect_CHECKBALLCOIN();

                case 1037:
                    return new BEffect_GLASSCOST();

                case 1038:
                    return new BEffect_REFRESHCOST();

                case 1039:
                    return new BEffect_GLASSRATE();
                    
                
                default:
                    return null;
            }
        }

        public virtual string ShowString()
        {
            return "";
        }

        public virtual void Execute()
        {

        }

        public virtual void Cancel()
        {

        }

        public virtual string GetDescription()
        {
            return "";
        }

        public virtual void OnPlayStart()
        {

        }

        public virtual void OnPlayEnd()
        {

        }

        public virtual void OnBallShoot(Ball ball, bool is_real_shoot)
        {

        }

        public virtual void OnHitBefore(Ball ball, Obstacle obt, Collision2D collision)
        {

        }

        public virtual void OnHitAfter(Ball ball, Obstacle obt, Collision2D collision)
        {

        }

        public virtual void OnComplextInit(ComplextEvent evt)
        {

        }

        public virtual void OnPushBall(Ball ball)
        {

        }

        public virtual void OnDrawingObstacles(List<int> lists, AttributeValue draw_count)
        {

        }

        public virtual void OnGlassUpdate(int count)
        {

        }

        public virtual void OnCoinUpdate(int count)
        {

        }

        public virtual void OnHitBox(Ball ball, Box g, Collision2D collision)
        {

        }

        public virtual void OnEnterCollision(Ball ball, Collision2D collision)
        {

        }

        public virtual void OnBombBefore(Bomb bomb)
        {

        }

        public virtual void OnWillReceiveCoin(AttributeValue coin_number)
        {

        }

        public virtual void OnRefreshEvents(List<ComplextEvent> events, bool is_video_play)
        {

        }
    }
}
