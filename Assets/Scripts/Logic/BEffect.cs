using System;
using System.Collections;
using System.Collections.Generic;
using CB;
using UnityEngine;



namespace CB
{
    //瞄准线可以显示反弹的路线
    public class BEffect_AIM : BEffect
    {
        public BEffect_AIM() {}

        public override string GetDescription()
        {
            return "瞄准线可以显示反弹的路线。";
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
            return "每当弹珠连续击中相同类型的宝石时,伤害提高1倍。";
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
                    ball.m_Demage.PutAUL(this, (int)objs[1]);

                    GameFacade.Instance.EffectManager.FlyRate(collision.contacts[0].point, (int)objs[1]);

                    GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));
                }
                
            }
        }

        public override void OnHitAfter(Ball ball, Obstacle obt, Collision2D collision)
        {
            ball.m_Demage.Pop(this);
        }
    }


    //每拥有2枚碎片, 高级弹珠的伤害就增加1点
    public class BEffect_GROW : BEffect
    {
        public BEffect_GROW() {}
        
        public override string GetDescription()
        {
            return "每拥有2枚<sprite=0>,高阶弹珠的伤害增加1点。";
        }

        public override string ShowString()
        {
            return (GameFacade.Instance.Game.Glass * 0.5f).ToString();
        }

        public override void Execute()
        {
            GameFacade.Instance.Game.Balls.ForEach(b => {
                if (b.Type != _C.BALLTYPE.NORMAL && b.Type != _C.BALLTYPE.SMALL) {
                    b.m_Demage.PutADD(this, GameFacade.Instance.Game.Glass * 0.5f);
                }
            });
        }

        public override void Cancel()
        {
            GameFacade.Instance.Game.Balls.ForEach(b => {
                b.m_Demage.Pop(this);
            });
        }

        public override void OnGlassUpdate(int count)
        {
            GameFacade.Instance.Game.Balls.ForEach(b => {
                if (b.Type != _C.BALLTYPE.NORMAL && b.Type != _C.BALLTYPE.SMALL) {
                    b.m_Demage.PutADD(this, GameFacade.Instance.Game.Glass);
                }
            });
        }

        public override void OnPushBall(Ball ball)
        {
            if (ball.Type == _C.BALLTYPE.NORMAL || ball.Type == _C.BALLTYPE.SMALL) return;

            ball.m_Demage.PutADD(this, GameFacade.Instance.Game.Glass);
        }
    }


    //场上每存在一颗三角宝石,所有弹珠的伤害提高#倍
    public class BEffect_SANJIAO : BEffect
    {
        public BEffect_SANJIAO() {}

        public override string GetDescription()
        {
            return "场上每存在一颗<sprite=3>,所有弹珠的伤害提高0.2倍。";
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
                ball.m_Demage.PutAUL(this, count * 0.2f);
            }
        }

        public override void OnHitAfter(Ball ball, Obstacle obt, Collision2D collision)
        {
            ball.m_Demage.Pop(this);
        }
    }


    //结算时,场上每颗圆形宝石可转化成1枚金币
    public class BEffect_COIN : BEffect
    {
        public BEffect_COIN() {}

        public override string GetDescription()
        {
            return "结算时,场上每颗<sprite=4>转化成1枚<sprite=1>。";
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
            return "每次击中<sprite=2>，弹珠的弹力临时提高。";
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

        public override void OnComplextInit(ComplextEvent evt, BallData config)
        {
            if (evt.EventType == _C.COMPLEXTEVEMT.UPGRADE) {
                if (RandomUtility.IsHit(50) == true) {
                    if (config.Cost.ToNumber() > 1) {
                        config.Cost.PutADD(this, -1);

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
                b.m_Demage.PutADD(this, 1);
            });
        }

        public override void Cancel()
        {
            GameFacade.Instance.Game.Balls.ForEach(b => {
                b.m_Demage.Pop(this);
            });
        }

        public override void OnPushBall(Ball ball)
        {
            ball.m_Demage.PutADD(this, 1);
        }
    }


    //场上会额外生成一枚碎片
    public class BEffect_DRAWBALL : BEffect
    {
        public BEffect_DRAWBALL() {}
        public override string GetDescription()
        {
            return "击中<sprite=0>时有概率额外获得一枚。";
        }

        public override void OnHitGlass(Ball ball, Ghost g, Collision2D collision)
        {
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
                ball.m_Demage.PutAUL(this, 3);

                GameFacade.Instance.EffectManager.FlyRate(collision.contacts[0].point, 4);

                GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_TRIGGERRELICS, Belong));
            }
        }

        public override void OnHitAfter(Ball ball, Obstacle obt, Collision2D collision)
        {
            if (m_Count == 0) {
                ball.m_Demage.Pop(this);

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
            return "每拥有一枚<sprite=0>,<sprite=1>的伤害增加1点。";
        }

        public override string ShowString()
        {
            return GameFacade.Instance.Game.Glass.ToString();
        }

        public override void Execute()
        {
            GameFacade.Instance.Game.Balls.ForEach(b => {
                if (b.Type == _C.BALLTYPE.NORMAL) {
                    b.m_Demage.PutADD(this, GameFacade.Instance.Game.Glass);
                }
            });
        }

        public override void Cancel()
        {
            GameFacade.Instance.Game.Balls.ForEach(b => {
                b.m_Demage.Pop(this);
            });
        }

        public override void OnGlassUpdate(int count)
        {
            GameFacade.Instance.Game.Balls.ForEach(b => {
                if (b.Type == _C.BALLTYPE.NORMAL) {
                    b.m_Demage.PutADD(this, GameFacade.Instance.Game.Glass);
                }
            });
        }

        public override void OnPushBall(Ball ball)
        {
            if (ball.Type != _C.BALLTYPE.NORMAL) return;

            ball.m_Demage.PutADD(this, GameFacade.Instance.Game.Glass);
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
            return string.Format("拥有的弹珠越少，造成的伤害倍率越高(当前：X{0})", Rate);
        }

        public override string ShowString()
        {
            return string.Format("X{0}", Rate);
        }

        public override void Execute()
        {
            GameFacade.Instance.Game.Balls.ForEach(b => {
                b.m_Demage.PutAUL(this, Rate - 1);
            });
        }

        public override void Cancel()
        {
            GameFacade.Instance.Game.Balls.ForEach(b => {
                b.m_Demage.Pop(this);
            });
        }

        public override void OnPushBall(Ball ball)
        {
            GameFacade.Instance.Game.Balls.ForEach(b => {
                b.m_Demage.PutAUL(this, Rate - 1);
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
                b.m_Demage.PutADD(this, Count);
            });
        }

        public override void Cancel()
        {
            GameFacade.Instance.Game.Balls.ForEach(b => {
                b.m_Demage.Pop(this);
            });
        }

        public override void OnPushBall(Ball ball)
        {
            GameFacade.Instance.Game.Balls.ForEach(b => {
                b.m_Demage.PutADD(this, Count);
            });
        }

        public override void OnCoinUpdate(int coin)
        {
            GameFacade.Instance.Game.Balls.ForEach(b => {
                b.m_Demage.PutADD(this, Count);
            });
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


                case 1016:
                    return new BEffect_REPEAT();
                
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

        public virtual void OnComplextInit(ComplextEvent evt, BallData config)
        {

        }

        public virtual void OnPushBall(Ball ball)
        {

        }

        public virtual void OnDrawingObstacles(List<int> lists)
        {

        }

        public virtual void OnGlassUpdate(int count)
        {

        }

        public virtual void OnCoinUpdate(int count)
        {

        }

        public virtual void OnHitGlass(Ball ball, Ghost g, Collision2D collision)
        {

        }
    }
}
