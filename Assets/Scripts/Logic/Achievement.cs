using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;


namespace CB
{
    #region 单回合内未击中方块。
    public class Achievement_FANG : Achievement
    {
        private int m_Count = 0;

        public override string GetDescription()
        {
            return "单回合内未击中<sprite=2>";
        }

        protected override void OnReponseHitObstacle(GameEvent @event)
        {
            if (!IsActive) return;

            Obstacle obt = (Obstacle)@event.GetParam(0);
            if (obt.Order == 0)
            {
                m_Count++;
            }
        }  

        protected override void OnReponsePlayStart(GameEvent @event)
        {
            if (!IsActive) return;

            m_Count = 0;
        }

        protected override void OnReponsePlayEnd(GameEvent @event)
        {
            if (!IsActive) return;

            if (m_Count == 0)
            {
                this.Finish();
            }
        }
    }
    #endregion


    #region 单回合内未击中三角。
    public class Achievement_SANJIAO : Achievement
    {
        private int m_Count = 0;

        public override string GetDescription()
        {
            return "单回合内未击中<sprite=3>";
        }

        protected override void OnReponseHitObstacle(GameEvent @event)
        {
            if (!IsActive) return;

            Obstacle obt = (Obstacle)@event.GetParam(0);
            if (obt.Order == 1)
            {
                m_Count++;
            }
        }  

        protected override void OnReponsePlayStart(GameEvent @event)
        {
            if (!IsActive) return;

            m_Count = 0;
        }

        protected override void OnReponsePlayEnd(GameEvent @event)
        {
            if (!IsActive) return;

            if (m_Count == 0)
            {
                this.Finish();
            }
        }
    }
    #endregion


    #region 单回合内未击中圆。
    public class Achievement_YUAN : Achievement
    {
        private int m_Count = 0;

        public override string GetDescription()
        {
            return "单回合内未击中<sprite=4>";
        }

        protected override void OnReponseHitObstacle(GameEvent @event)
        {
            if (!IsActive) return;

            Obstacle obt = (Obstacle)@event.GetParam(0);
            if (obt.Order == 3)
            {
                m_Count++;
            }
        }  

        protected override void OnReponsePlayStart(GameEvent @event)
        {
            if (!IsActive) return;

            m_Count = 0;
        }

        protected override void OnReponsePlayEnd(GameEvent @event)
        {
            if (!IsActive) return;

            if (m_Count == 0)
            {
                this.Finish();
            }
        }
    }
    #endregion


    #region 单回合内未击中菱形。
    public class Achievement_LING : Achievement
    {
        private int m_Count = 0;
        public override string GetDescription()
        {
            return "单回合内未击中<sprite=5>";
        }

        protected override void OnReponseHitObstacle(GameEvent @event)
        {
            if (!IsActive) return;

            Obstacle obt = (Obstacle)@event.GetParam(0);
            if (obt.Order == 2)
            {
                m_Count++;
            }
        }  

        protected override void OnReponsePlayStart(GameEvent @event)
        {
            if (!IsActive) return;

            m_Count = 0;
        }

        protected override void OnReponsePlayEnd(GameEvent @event)
        {
            if (!IsActive) return;

            if (m_Count == 0)
            {
                this.Finish();
            }
        }
    }
    #endregion


    #region 单回合击落#方块。
    public class Achievement_KILLFANG : Achievement
    {
        private int m_Count = 0;
        private int m_Max   = 8;
        public override string GetDescription()
        {
            return string.Format("单回合内击落<size=46><#FFCC4A>{0}</color></size>个<sprite=2>", m_Max);
        }

        protected override void OnReponseHitObstacle(GameEvent @event)
        {
            if (!IsActive) return;

            Obstacle obt = (Obstacle)@event.GetParam(0);
            if (obt.IsDead() == true && obt.Order == 0)
            {
                m_Count++;
                if (m_Count >= m_Max) this.Finish();
            }
        }  

        protected override void OnReponsePlayStart(GameEvent @event)
        {
            if (!IsActive) return;

            m_Count = 0;
        }
    }
    #endregion


    #region 单回合击落#三角。
    public class Achievement_KILLSANJIAO : Achievement
    {
        private int m_Count = 0;
        private int m_Max   = 8;
        public override string GetDescription()
        {
            return string.Format("单回合内击落<size=46><#FFCC4A>{0}</color></size>个<sprite=3>", m_Max);
        }

        protected override void OnReponseHitObstacle(GameEvent @event)
        {
            if (!IsActive) return;

            Obstacle obt = (Obstacle)@event.GetParam(0);
            if (obt.IsDead() == true && obt.Order == 1)
            {
                m_Count++;
                if (m_Count >= m_Max) this.Finish();
            }
        } 

        protected override void OnReponsePlayStart(GameEvent @event)
        {
            if (!IsActive) return;

            m_Count = 0;
        }
    }
    #endregion


    #region 单回合击落#圆。
    public class Achievement_KILLYUAN : Achievement
    {
        private int m_Count = 0;
        private int m_Max   = 8;
        public override string GetDescription()
        {
            return string.Format("单回合内击落<size=46><#FFCC4A>{0}</color></size>个<sprite=4>", m_Max);
        }

        protected override void OnReponseHitObstacle(GameEvent @event)
        {
            if (!IsActive) return;

            Obstacle obt = (Obstacle)@event.GetParam(0);
            if (obt.IsDead() == true && obt.Order == 3)
            {
                m_Count++;
                if (m_Count >= m_Max) this.Finish();
            }
        }  

        protected override void OnReponsePlayStart(GameEvent @event)
        {
            if (!IsActive) return;

            m_Count = 0;
        }
    }
    #endregion


    #region 单回合击落#菱形。
    public class Achievement_KILLLING : Achievement
    {
        private int m_Count = 0;
        private int m_Max   = 8;
        public override string GetDescription()
        {
            return string.Format("单回合内击落<size=46><#FFCC4A>{0}</color></size>个<sprite=5>", m_Max);
        }

        protected override void OnReponseHitObstacle(GameEvent @event)
        {
            if (!IsActive) return;

            Obstacle obt = (Obstacle)@event.GetParam(0);
            if (obt.IsDead() == true && obt.Order == 2)
            {
                m_Count++;
                if (m_Count >= m_Max) this.Finish();
            }
        }  

        protected override void OnReponsePlayStart(GameEvent @event)
        {
            if (!IsActive) return;

            m_Count = 0;
        }
    }
    #endregion


    #region 单回合内击落#个碎片。
    public class Achievement_KILLGLASS : Achievement
    {
        private int m_Count = 0;
        private int m_Max   = 8;
        public override string GetDescription()
        {
            return string.Format("单回合内击落<size=46><#FFCC4A>{0}</color></size>枚<sprite=0>", m_Max);
        }

        protected override void OnReponseHitBox(GameEvent @event)
        {
            if (!IsActive) return;

            Ghost ghost = ((Box)@event.GetParam(1)).GetComponent<Ghost>();
            if (ghost != null && ghost.IsDead() == true)
            {
                m_Count++;
                if (m_Count >= m_Max) this.Finish();
            }
        }  

        protected override void OnReponsePlayStart(GameEvent @event)
        {
            if (!IsActive) return;

            m_Count = 0;
        }
    }
    #endregion


    #region 单回合击落3个炸弹
    public class Achievement_KILLBOMB : Achievement
    {
        private int m_Count = 0;
        private int m_Max   = 3;
        public override string GetDescription()
        {
            return string.Format("单回合内击落<size=46><#FFCC4A>{0}</color></size>个<sprite=7>", m_Max);
        }

        protected override void OnReponseHitBox(GameEvent @event)
        {
            if (!IsActive) return;

            Bomb bomb = ((Box)@event.GetParam(1)).GetComponent<Bomb>();
            if (bomb != null && bomb.IsDead() == true)
            {
                m_Count++;
                if (m_Count >= m_Max) this.Finish();
            }
        }  

        protected override void OnReponsePlayStart(GameEvent @event)
        {
            if (!IsActive) return;

            m_Count = 0;
        }
    }
    #endregion


    #region 单局游戏内累计获得50金币
    public class Achievement_COIN100 : Achievement
    {
        private int m_Count = 0;
        private int m_Max   = 50;

        public override string GetDescription()
        {
            return string.Format("单局游戏内累计获得<size=46><#FFCC4A>{0}</color></size><sprite=1>", m_Max);
        }

        protected override void OnReponseCoinUpdate(GameEvent @event)
        {
            if (!IsActive) return;

            bool is_reward = (bool)@event.GetParam(2);
            if (!is_reward) return;

            int value = (int)@event.GetParam(1);
            if (value > 0) {
                m_Count += value;

                if (m_Count >= m_Max) this.Finish();
            }
        }

        protected override void OnReponseGameStart(GameEvent @event)
        {
            m_Count = 0;
        }
    }
    #endregion


    #region 单局游戏内累计获得250金币
    public class Achievement_COIN250 : Achievement
    {
        private int m_Count = 0;
        private int m_Max   = 250;

        public override string GetDescription()
        {
            return string.Format("单局游戏内累计获得<size=46><#FFCC4A>{0}</color></size><sprite=1>", m_Max);
        }

        protected override void OnReponseCoinUpdate(GameEvent @event)
        {
            if (!IsActive) return;

            bool is_reward = (bool)@event.GetParam(2);
            if (!is_reward) return;

            int value = (int)@event.GetParam(1);
            if (value > 0) {
                m_Count += value;

                if (m_Count >= m_Max) this.Finish();
            }
        }

        protected override void OnReponseGameStart(GameEvent @event)
        {
            m_Count = 0;
        }
    }
    #endregion


    #region 单局游戏内累计获得500金币
    public class Achievement_COIN500 : Achievement
    {
        private int m_Count = 0;
        private int m_Max   = 500;

        public override string GetDescription()
        {
            return string.Format("单局游戏内累计获得<size=46><#FFCC4A>{0}</color></size><sprite=1>", m_Max);
        }

        protected override void OnReponseCoinUpdate(GameEvent @event)
        {
            if (!IsActive) return;

            bool is_reward = (bool)@event.GetParam(2);
            if (!is_reward) return;

            int value = (int)@event.GetParam(1);
            if (value > 0) {
                m_Count += value;

                if (m_Count >= m_Max) this.Finish();
            }
        }

        protected override void OnReponseGameStart(GameEvent @event)
        {
            m_Count = 0;
        }
    }
    #endregion


    #region 单局游戏内累计获得1000金币
    public class Achievement_COIN1000 : Achievement
    {
        private int m_Count = 0;
        private int m_Max   = 1000;

        public override string GetDescription()
        {
            return string.Format("单局游戏内累计获得<size=46><#FFCC4A>{0}</color></size><sprite=1>", m_Max);
        }

        protected override void OnReponseCoinUpdate(GameEvent @event)
        {
            if (!IsActive) return;

            bool is_reward = (bool)@event.GetParam(2);
            if (!is_reward) return;

            int value = (int)@event.GetParam(1);
            if (value > 0) {
                m_Count += value;

                if (m_Count >= m_Max) this.Finish();
            }
        }

        protected override void OnReponseGameStart(GameEvent @event)
        {
            m_Count = 0;
        }
    }
    #endregion


    #region 单局游戏内累计击落#枚碎片
    public class Achievement_GLASS10 : Achievement
    {
        private int m_Count = 0;
        private int m_Max   = 15;

        public override string GetDescription()
        {
            return string.Format("单局游戏内累计击落<size=46><#FFCC4A>{0}</color></size>枚<sprite=0>", m_Max);
        }

        protected override void OnReponseGlassUpdate(GameEvent @event)
        {
            if (!IsActive) return;

            bool is_reward = (bool)@event.GetParam(2);
            if (!is_reward) return;

            int value = (int)@event.GetParam(1);
            if (value > 0) {
                m_Count += value;

                if (m_Count >= m_Max) this.Finish();
            }
        }

        protected override void OnReponseGameStart(GameEvent @event)
        {
            m_Count = 0;
        }
    }
    #endregion


    #region 单局游戏内累计击落50枚碎片
    public class Achievement_GLASS50 : Achievement
    {
        private int m_Count = 0;
        private int m_Max   = 50;

        public override string GetDescription()
        {
            return string.Format("单局游戏内累计击落<size=46><#FFCC4A>{0}</color></size>枚<sprite=0>", m_Max);
        }

        protected override void OnReponseGlassUpdate(GameEvent @event)
        {
            if (!IsActive) return;

            bool is_reward = (bool)@event.GetParam(2);
            if (!is_reward) return;

            int value = (int)@event.GetParam(1);
            if (value > 0) {
                m_Count += value;

                if (m_Count >= m_Max) this.Finish();
            }
        }

        protected override void OnReponseGameStart(GameEvent @event)
        {
            m_Count = 0;
        }
    }
    #endregion


    #region 单局游戏内累计击落150枚碎片
    public class Achievement_GLASS150 : Achievement
    {
        private int m_Count = 0;
        private int m_Max   = 150;

        public override string GetDescription()
        {
            return string.Format("单局游戏内累计击落<size=46><#FFCC4A>{0}</color></size>枚<sprite=0>", m_Max);
        }

        protected override void OnReponseGlassUpdate(GameEvent @event)
        {
            if (!IsActive) return;

            bool is_reward = (bool)@event.GetParam(2);
            if (!is_reward) return;

            int value = (int)@event.GetParam(1);
            if (value > 0) {
                m_Count += value;

                if (m_Count >= m_Max) this.Finish();
            }
        }

        protected override void OnReponseGameStart(GameEvent @event)
        {
            if (!IsActive) return;

            m_Count = 0;
        }
    }
    #endregion


    #region 单局游戏内累计击落10枚炸弹(开局赠送炸弹弹珠)
    public class Achievement_REVERSEBOMB : Achievement
    {
        private int m_Count = 0;
        private int m_Max   = 10;

        public override string GetDescription()
        {
            return string.Format("单局游戏内累计击落<size=46><#FFCC4A>{0}</color></size>枚<sprite=7>", m_Max);
        }

        protected override void OnReponseGameStart(GameEvent @event)
        {
            if (!IsActive) return;

            m_Count = 0;
        }

        protected override void OnReponseHitBox(GameEvent @event)
        {
            if (!IsActive) return;

            Bomb bomb = ((Box)@event.GetParam(1)).GetComponent<Bomb>();
            if (bomb != null && bomb.IsDead() == true)
            {
                m_Count++;
                if (m_Count >= m_Max) this.Finish();
            }
        }  
    }
    #endregion


    #region 最高记录达到10层
    public class Achievement_SCORE10 : Achievement
    {
        private int m_Max   = 10;

        public override string GetDescription()
        {
            return string.Format("最高记录达到<size=46><#FFCC4A>{0}</color></size>层", m_Max);
        }

        protected override void OnReponsePlayEnd(GameEvent @event)
        {
            if (!IsActive) return;

            if (GameFacade.Instance.User.Score >= m_Max)
            {
                this.Finish();
            }
        }

    }
    #endregion


    #region 最高记录达到25层
    public class Achievement_SCORE25 : Achievement
    {
        private int m_Max   = 25;

        public override string GetDescription()
        {
            return string.Format("最高记录达到<size=46><#FFCC4A>{0}</color></size>层", m_Max);
        }

        protected override void OnReponsePlayEnd(GameEvent @event)
        {
            if (!IsActive) return;

            if (GameFacade.Instance.User.Score >= m_Max)
            {
                this.Finish();
            }
        }

    }
    #endregion


    #region 最高记录达到50层
    public class Achievement_SCORE50 : Achievement
    {
        private int m_Max   = 50;

        public override string GetDescription()
        {
            return string.Format("最高记录达到<size=46><#FFCC4A>{0}</color></size>层", m_Max);
        }

        protected override void OnReponsePlayEnd(GameEvent @event)
        {
            if (!IsActive) return;

            if (GameFacade.Instance.User.Score >= m_Max)
            {
                this.Finish();
            }
        }

    }
    #endregion


    #region 最高记录达到100层
    public class Achievement_SCORE100 : Achievement
    {
        private int m_Max   = 100;

        public override string GetDescription()
        {
            return string.Format("最高记录达到<size=46><#FFCC4A>{0}</color></size>层", m_Max);
        }

        protected override void OnReponsePlayEnd(GameEvent @event)
        {
            if (!IsActive) return;

            if (GameFacade.Instance.User.Score >= m_Max)
            {
                this.Finish();
            }
        }

    }
    #endregion


    #region 单颗弹珠击落#枚碎片。
    public class Achievement_BALLHITGLASS : Achievement
    {
        private int m_Max   = 6;
        private Dictionary<Ball, int> m_Records = new Dictionary<Ball, int>();

        public override string GetDescription()
        {
            return string.Format("单颗弹珠击落<size=46><#FFCC4A>{0}</color></size>枚<sprite=0>", m_Max);
        }

        protected override void OnReponseHitBox(GameEvent @event)
        {
            if (!IsActive) return;

            Ball ball = (Ball)@event.GetParam(0);
            Ghost ghost = ((Box)@event.GetParam(1)).GetComponent<Ghost>();
            if (ghost != null && ghost.IsDead() == true)
            {
                if (!m_Records.ContainsKey(ball)) {
                    m_Records.Add(ball, 0);
                } 

                m_Records[ball] += 1;

                if (m_Records[ball] >= m_Max) {
                    this.Finish();
                }
            }
        }

        protected override void OnReponsePlayEnd(GameEvent @event)
        {
            if (!IsActive) return;

            m_Records.Clear();
        }
    }
    #endregion


    #region 同一颗弹珠被回收3次。 
    public class Achievement_BALLRECYCLE : Achievement
    {
        private int m_Max   = 3;
        private Dictionary<Ball, int> m_Records = new Dictionary<Ball, int>();

        public override string GetDescription()
        {
            return string.Format("同一颗弹珠被回收<size=46><#FFCC4A>{0}</color></size>次", m_Max);
        }

        protected override void OnReponseBallRecycle(GameEvent @event)
        {
            if (!IsActive) return;

            var ball = (Ball)@event.GetParam(0);

            if (!m_Records.ContainsKey(ball)) {
                m_Records.Add(ball, 0);
            } 

            m_Records[ball] += 1;

            if (m_Records[ball] >= m_Max) {
                this.Finish();
            }
        }
    }
    #endregion


    #region 弹珠没有击中任何物体。
    public class Achievement_BALLMISS : Achievement
    {
        private HashSet<Ball> m_Records = new HashSet<Ball>();

        public override string GetDescription()
        {
            return string.Format("弹珠没有击中任何物体");
        }

        protected override void OnReponsePlayEnd(GameEvent @event)
        {
            if (!IsActive) return;

            m_Records.Clear();
        }

        protected override void OnReponseHitAfter(GameEvent @event)
        {
            if (!IsActive) return;

            Ball ball = (Ball)@event.GetParam(0);
            if (!m_Records.Contains(ball)) {
                m_Records.Add(ball);
            }
        }

        protected override void OnReponseHitBox(GameEvent @event)
        {
            if (!IsActive) return;

            Ball ball = (Ball)@event.GetParam(0);
            if (!m_Records.Contains(ball)) {
                m_Records.Add(ball);
            } 
        }

        protected override void OnReponseEnterGround(GameEvent @event)
        {
            if (!IsActive) return;

            Ball ball = (Ball)@event.GetParam(0);
            if (!m_Records.Contains(ball)) {
                this.Finish();
            } 
        }
    }
    #endregion


    #region 回合开始时，弹珠槽中拥有5颗普通弹珠
    public class Achievement_NORMALBALL : Achievement
    {
        private int m_Max = 5;

        public override string GetDescription()
        {
            return string.Format("回合开始时弹珠槽中拥有<size=46><#FFCC4A>{0}</color></size>颗<sprite={1}>", m_Max, (int)_C.SPRITEATLAS.BALL);
        }

        protected override void OnReponsePlayStart(GameEvent @event)
        {
            if (!IsActive) return;

            int count = 0;
            GameFacade.Instance.Game.Balls.ForEach(ball => {
                if (ball.Type == _C.BALLTYPE.NORMAL) {
                    count++;
                }
            });

            if (count >= m_Max) {
                this.Finish();
            }
        }

    }
    #endregion


    #region 回合开始时，弹珠槽中同时拥有(方/圆/三角/菱)四颗弹珠
    public class Achievement_OBTBALL : Achievement
    {
        public override string GetDescription()
        {
            return string.Format("回合开始时弹珠槽中同时拥有(方/圆/三角/菱)四颗弹珠");
        }

        protected override void OnReponsePlayStart(GameEvent @event)
        {
            if (!IsActive) return;

            bool ball_sanjiao = false;
            bool ball_yuan = false;
            bool ball_fang = false;
            bool ball_ling = false;

            GameFacade.Instance.Game.Balls.ForEach(ball => {
                if (ball.Type == _C.BALLTYPE.FANG) ball_fang = true; 
                else if (ball.Type == _C.BALLTYPE.SANJIAO) ball_sanjiao = true;
                else if (ball.Type == _C.BALLTYPE.YUAN) ball_yuan = true;
                else if (ball.Type == _C.BALLTYPE.LING) ball_ling = true;
            });

            if (ball_sanjiao && ball_yuan && ball_fang && ball_ling) {
                this.Finish();
            }
        }

    }
    #endregion


    #region 黑洞弹珠首次命中宝石时就形成黑洞
    public class Achievement_BLACKHOLE : Achievement
    {
        private Dictionary<Ball, int> m_Records = new Dictionary<Ball, int>();
        public override string GetDescription()
        {
            return string.Format("黑洞弹珠首次命中宝石即形成黑洞");
        }

        protected override void OnReponseBlackHole(GameEvent @event)
        {
            if (!IsActive) return;

            Ball ball = (Ball)@event.GetParam(0);

            if (!m_Records.ContainsKey(ball)) {
                return;
            }

            if (m_Records[ball] == 1) this.Finish();
        }

        protected override void OnReponseHitAfter(GameEvent @event)
        {
            if (!IsActive) return;

            Ball ball = (Ball)@event.GetParam(0);
            if (!m_Records.ContainsKey(ball)) {
                m_Records.Add(ball, 0);
            }

            m_Records[ball] += 1;
        }

    }
    #endregion


    #region 单回合内未击落碎片
    public class Achievement_GLASSMISS : Achievement
    {
        private int m_Count = 0;
        public override string GetDescription()
        {
            return string.Format("单回合内未击落<sprite={0}>", (int)_C.SPRITEATLAS.GLASS);
        }

        protected override void OnReponseHitBox(GameEvent @event)
        {
            if (!IsActive) return;

            Ghost ghost = ((Box)@event.GetParam(1)).GetComponent<Ghost>();
            if (ghost != null && ghost.IsDead() == true)
            {
                m_Count++;
            }
        }  

        protected override void OnReponsePlayStart(GameEvent @event)
        {
            if (!IsActive) return;

            m_Count = 0;
        }

        protected override void OnReponsePlayEnd(GameEvent @event)
        {
            if (!IsActive) return;

            if (m_Count == 0)
            {
                this.Finish();
            }
        }

    }
    #endregion


    #region 弹珠的飞行时间超过#秒
    public class Achievement_FLYTIME : Achievement
    {
        private int m_Max = 25;
        public override string GetDescription()
        {
            return string.Format("弹珠的飞行时间超过<size=46><#FFCC4A>{0}</color></size>秒", m_Max);
        }

        protected override void OnReponseBallFly(GameEvent @event)
        {
            if (!IsActive) return;

            Ball ball = (Ball)@event.GetParam(0);

            if (ball.IsSimulate) return;

            if (ball.FlyTime >= m_Max) {
                this.Finish();
            }
        }
    }
    #endregion


    #region 合成1次弹珠
    public class Achievement_COMPLEX : Achievement
    {
        private int m_Count = 0;
        private int m_Max = 1;
        public override string GetDescription()
        {
            return string.Format("合成<size=46><#FFCC4A>{0}</color></size>次弹珠", m_Max);
        }

        protected override void OnReponseComplexBall(GameEvent @event)
        {
            if (!IsActive) return;

            m_Count++;
            if (m_Count >= m_Max) {
                this.Finish();
            }
        }
    }
    #endregion


    #region 购买1次道具
    public class Achievement_BUYRELICS : Achievement
    {
        private int m_Count = 0;
        private int m_Max = 1;
        public override string GetDescription()
        {
            return string.Format("购买<size=46><#FFCC4A>{0}</color></size>次道具", m_Max);
        }

        protected override void OnReponseBuyRelics(GameEvent @event)
        {
            if (!IsActive) return;

            m_Count++;
            if (m_Count >= m_Max) {
                this.Finish();
            }
        }
    }
    #endregion


    #region 连续#次击中相同类型的宝石
    public class Achievement_CONTINUEHIT : Achievement
    {
        private Dictionary<Ball, int> m_Records = new Dictionary<Ball, int>();
        private int m_Last = -1;
        private int m_Max = 10;
        public override string GetDescription()
        {
            return string.Format("弹珠连续<size=46><#FFCC4A>{0}</color></size>次击中相同类型的宝石", m_Max);
        }

        protected override void OnReponseBallShoot(GameEvent @event)
        {
            if (!IsActive) return;

            Ball ball = (Ball)@event.GetParam(0);

            if (m_Records.ContainsKey(ball)) {
                m_Records[ball] = 0;
            }
        }

        protected override void OnReponseHitAfter(GameEvent @event)
        {
            if (!IsActive) return;

            Ball ball = (Ball)@event.GetParam(0);
            Obstacle obt = (Obstacle)@event.GetParam(1);

            if (!m_Records.ContainsKey(ball)) {
                m_Records.Add(ball, 0);
            }

            if (m_Last == -1) {
                m_Last = obt.Order;
                m_Records[ball] += 1;
            } else {
                if (m_Last == obt.Order) {
                    m_Records[ball] += 1;
                } else {
                    m_Last = obt.Order;
                    m_Records[ball] = 1;
                }
            }

            

            if (m_Records[ball] >= m_Max) {
                this.Finish();
            }
        }

        protected override void OnReponsePlayEnd(GameEvent @event)
        {
            if (!IsActive) return;

            m_Records.Clear();
        }
    }
    #endregion


    #region 弹珠连续#次击中宝石
    public class Achievement_CONTINUEHITOBT : Achievement
    {
        private Dictionary<Ball, int> m_Records = new Dictionary<Ball, int>();
        private int m_Max = 15;
        public override string GetDescription()
        {
            return string.Format("弹珠不中断的连续<size=46><#FFCC4A>{0}</color></size>次击中宝石", m_Max);
        }

        protected override void OnReponseHitAfter(GameEvent @event)
        {
            if (!IsActive) return;

            Ball ball = (Ball)@event.GetParam(0);

            if (!m_Records.ContainsKey(ball)) {
                m_Records.Add(ball, 0);
            }

            m_Records[ball] += 1;

            if (m_Records[ball] >= m_Max) {
                this.Finish();
            }
        }

        protected override void OnReponsePlayEnd(GameEvent @event)
        {
            if (!IsActive) return;

            m_Records.Clear();
        }

        protected override void OnReponseEnterCollision(GameEvent @event)
        {
            if (!IsActive) return;

            var ball = (Ball)@event.GetParam(0);
            
            if (ball.IsSimulate == true) return;
            if (ball.IsRecycle == true) return;

            var collision = (Collision2D)@event.GetParam(1);
            if (collision.transform.GetComponent<Wall>() != null)
            {
                if (m_Records.ContainsKey(ball)) {
                    m_Records[ball] = 0;
                }
            }
        }

        protected override void OnReponseHitBox(GameEvent @event)
        {
            if (!IsActive) return;

            Ball ball = (Ball)@event.GetParam(0);
            if (m_Records.ContainsKey(ball)) {
                m_Records[ball] = 0;
            }
        }
    }
    #endregion


    #region 清空场上所有宝石
    public class Achievement_CLEAROTB : Achievement
    {
        public override string GetDescription()
        {
            return string.Format("清空场上所有宝石");
        }

        protected override void OnReponseHitObstacle(GameEvent @event)
        {
            if (!IsActive) return;

            bool is_clear = true;
            GameFacade.Instance.Game.Obstacles.ForEach(obstacle => {
                if (obstacle.IsDead() == false) {
                    is_clear = false;
                }
            });

            if (is_clear) {
                this.Finish();
            }
        }
    }
    #endregion


    #region 单回合内累计发射5颗弹珠
    public class Achievement_CUMHIT : Achievement
    {
        private int m_Count = 0;
        private int m_Max   = 5;
        public override string GetDescription()
        {
            return string.Format("单回合内累计发射<size=46><#FFCC4A>{0}</color></size>颗弹珠", m_Max);
        }

        protected override void OnReponseBallShoot(GameEvent @event)
        {
            if (!IsActive) return;

            var ball = (Ball)@event.GetParam(0);

            if (ball.IsSimulate) return;

            var flag = (bool)@event.GetParam(1);
            if (flag) {
                m_Count++;

                if (m_Count >= m_Max) {
                    this.Finish();
                }
            }
        }

        protected override void OnReponsePlayStart(GameEvent @event)
        {
            if (!IsActive) return;

            m_Count= 0;
        }
    }
    #endregion





    //成就
    public class Achievement
    {
        private AchievementData m_Data;
        public int ID { get {return m_Data.ID;}}

        protected bool m_FinishFlag = false;
        public bool IsFinished { get {return m_FinishFlag;}}

        protected RewardEffect m_Effect = null;

        //是否激活中
        public bool IsActive {
            get {
                if (m_FinishFlag) return false;

                //判断前置是否完成了
                if (m_Data.Previous != default(int))
                {
                    var data = GameFacade.Instance.DataCenter.GetAchievement(m_Data.Previous);
                    return data.IsFinished;
                }
                return true;
            }
        }

        public bool IsShow{
            get {
                //判断前置是否完成了
                if (m_Data.Previous != default(int))
                {
                    var data = GameFacade.Instance.DataCenter.GetAchievement(m_Data.Previous);
                    return data.IsFinished;
                }
                return true;
            }
        }

        private static Dictionary<int, Func<Achievement>> m_classDictionary = new Dictionary<int, Func<Achievement>> {
            { 10001, () => new Achievement_FANG()},
            { 10002, () => new Achievement_SANJIAO()},
            { 10003, () => new Achievement_YUAN()},
            { 10004, () => new Achievement_LING()},
            { 10005, () => new Achievement_KILLFANG()},
            { 10006, () => new Achievement_KILLSANJIAO()},
            { 10007, () => new Achievement_KILLYUAN()},
            { 10008, () => new Achievement_KILLLING()},
            { 10009, () => new Achievement_KILLGLASS()},
            { 10010, () => new Achievement_KILLBOMB()},
            { 10011, () => new Achievement_COIN100()},
            { 10012, () => new Achievement_COIN250()},
            { 10013, () => new Achievement_COIN500()},
            { 10014, () => new Achievement_COIN1000()},
            { 10015, () => new Achievement_GLASS10()},
            { 10016, () => new Achievement_GLASS50()},
            { 10017, () => new Achievement_GLASS150()},
            { 10018, () => new Achievement_REVERSEBOMB()},
            { 10019, () => new Achievement_SCORE10()},
            { 10020, () => new Achievement_SCORE25()},
            { 10021, () => new Achievement_SCORE50()},
            { 10022, () => new Achievement_SCORE100()},
            { 10023, () => new Achievement_BALLHITGLASS()},
            { 10024, () => new Achievement_BALLRECYCLE()},
            { 10025, () => new Achievement_BALLMISS()},
            { 10026, () => new Achievement_NORMALBALL()},
            { 10027, () => new Achievement_OBTBALL()},
            { 10028, () => new Achievement_BLACKHOLE()},
            { 10029, () => new Achievement_GLASSMISS()},
            { 10030, () => new Achievement_FLYTIME()},
            { 10031, () => new Achievement_COMPLEX()},
            { 10032, () => new Achievement_BUYRELICS()},
            { 10033, () => new Achievement_CONTINUEHIT()},
            { 10034, () => new Achievement_CONTINUEHITOBT()},
            { 10035, () => new Achievement_CLEAROTB()},
            { 10036, () => new Achievement_CUMHIT()},
        };

        public static Achievement Create(AchievementData data)
        {
            Achievement achievement = null;

            if (m_classDictionary.ContainsKey(data.ID)) {
                achievement = m_classDictionary[data.ID]();
            } else {
                achievement = new Achievement();
            }

            achievement.Init(data);

            return achievement;
        }

        public virtual string GetDescription()
        {
            return "";
        }

        public void Sync()
        {
            m_FinishFlag = true;
        }

        public void Finish()
        {
            if (m_FinishFlag == true) return;
            //存储
            m_FinishFlag = true;
            GameFacade.Instance.User.SetAchievement(ID);

            //通知UI 成就完成
            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_ACHIEVEMENTPOP, this));
        }

        //金币奖励
        public int GetCoin()
        {
            if (m_Effect != null) {
                return m_Effect.GetCoin();
            }

            return 0;
        }

        //碎片奖励
        public int GetGlass()
        {
            if (m_Effect != null) {
                return m_Effect.GetGlass();
            }
            return 0;
        }

        public void DoReward()
        {
            if (!m_FinishFlag) return;

            if (m_Effect != null)
            {
                m_Effect.DoReward();
            }
        }

        public string GetRewardString()
        {
            return "";
        }

        public void Dispose()
        {
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONGAMESTART,      OnReponseGameStart);
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONGAMEEND,        OnReponseGameEnd);
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONPLAYSTART,      OnReponsePlayStart);
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONPLAYEND,        OnReponsePlayEnd);
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONHITOBSTACLE,    OnReponseHitObstacle);
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONBALLHITAFTER,   OnReponseHitAfter);
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONBALLHITBOX,     OnReponseHitBox);
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONCOINUPDATE,     OnReponseCoinUpdate);
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONGLASSUPDATE,    OnReponseGlassUpdate);
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONBALLSHOOT,      OnReponseBallShoot);
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONBALLRECYCLE,    OnReponseBallRecycle);
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONENTERGROUND,    OnReponseEnterGround);
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONCREATEBLACKHOLE,OnReponseBlackHole);
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONBALLDEAD,       OnReponseBallDead);
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONBALLFLY,        OnReponseBallFly);
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONCOMPLEXBALL,    OnReponseComplexBall);
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONBUYRELICS,      OnReponseBuyRelics);
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONENTERCOLLISION, OnReponseEnterCollision);

        }

        void ParseEffect()
        {
            if (string.IsNullOrEmpty(m_Data.Effect)) return;

            string[] str_array = m_Data.Effect.Split(':');
            string key  = str_array[0];
            int value   = Convert.ToInt32(str_array[1]);

            m_Effect    = RewardEffect.Create(key, value);
        }

        void Init(AchievementData data)
        {
            m_Data = data;

            ParseEffect();

            

            GameFacade.Instance.EventManager.AddHandler(EVENT.ONGAMESTART,      OnReponseGameStart);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONGAMEEND,        OnReponseGameEnd);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONPLAYSTART,      OnReponsePlayStart);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONPLAYEND,        OnReponsePlayEnd);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONHITOBSTACLE,    OnReponseHitObstacle);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONBALLHITAFTER,   OnReponseHitAfter);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONBALLHITBOX,     OnReponseHitBox);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONCOINUPDATE,     OnReponseCoinUpdate);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONGLASSUPDATE,    OnReponseGlassUpdate);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONBALLSHOOT,      OnReponseBallShoot);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONBALLRECYCLE,    OnReponseBallRecycle);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONENTERGROUND,    OnReponseEnterGround);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONCREATEBLACKHOLE,OnReponseBlackHole);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONBALLDEAD,       OnReponseBallDead);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONBALLFLY,        OnReponseBallFly);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONCOMPLEXBALL,    OnReponseComplexBall);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONBUYRELICS,      OnReponseBuyRelics);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONENTERCOLLISION, OnReponseEnterCollision);
            
        }

        protected virtual void OnReponseEnterCollision(GameEvent @event)
        {

        }

        protected virtual void OnReponseBuyRelics(GameEvent @event)
        {

        }

        protected virtual void OnReponseComplexBall(GameEvent @event)
        {

        }

        protected virtual void OnReponseBallFly(GameEvent @event)
        {

        }

        protected virtual void OnReponseBallDead(GameEvent @event)
        {

        }

        protected virtual void OnReponseHitObstacle(GameEvent @event)
        {

        }

        protected virtual void OnReponseBlackHole(GameEvent @event)
        {

        }

        protected virtual void OnReponseEnterGround(GameEvent @event)
        {

        }

        protected virtual void OnReponseBallRecycle(GameEvent @event)
        {

        }

        protected virtual void OnReponseBallShoot(GameEvent @event)
        {

        }

        protected virtual void OnReponseGlassUpdate(GameEvent @event)
        {

        }

        protected virtual void OnReponseGameEnd(GameEvent @event)
        {

        }

        protected virtual void OnReponseGameStart(GameEvent @event)
        {

        }

        protected virtual void OnReponseCoinUpdate(GameEvent @event)
        {

        }

        protected virtual void OnReponseHitBox(GameEvent @event)
        {

        }

        protected virtual void OnReponsePlayStart(GameEvent @event)
        {

        }

        protected virtual void OnReponseHitAfter(GameEvent @event)
        {

        }   

        protected virtual void OnReponsePlayEnd(GameEvent @event)
        {

        }

    }
}

