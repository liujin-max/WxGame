using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CB
{
    #region 单回合内未击落方块。
    public class Achievement_FANG : Achievement
    {
        private int m_Count = 0;
        protected override void OnReponseHitAfter(GameEvent @event)
        {
            if (m_FinishFlag) return;

            Obstacle obt = (Obstacle)@event.GetParam(1);
            if (obt.IsDead() == true && obt.Order == 0)
            {
                m_Count++;
            }
        }  

        protected override void OnReponsePlayStart(GameEvent @event)
        {
            if (m_FinishFlag) return;

            m_Count = 0;
        }

        protected override void OnReponsePlayEnd(GameEvent @event)
        {
            if (m_FinishFlag) return;

            if (m_Count == 0)
            {
                this.Finish();
            }
        }
    }
    #endregion


    #region 单回合内未击落三角。
    public class Achievement_SANJIAO : Achievement
    {
        private int m_Count = 0;
        protected override void OnReponseHitAfter(GameEvent @event)
        {
            if (m_FinishFlag) return;

            Obstacle obt = (Obstacle)@event.GetParam(1);
            if (obt.IsDead() == true && obt.Order == 1)
            {
                m_Count++;
            }
        }  

        protected override void OnReponsePlayStart(GameEvent @event)
        {
            if (m_FinishFlag) return;

            m_Count = 0;
        }

        protected override void OnReponsePlayEnd(GameEvent @event)
        {
            if (m_FinishFlag) return;

            if (m_Count == 0)
            {
                this.Finish();
            }
        }
    }
    #endregion

    #region 单回合内未击落圆。
    public class Achievement_YUAN : Achievement
    {
        private int m_Count = 0;
        protected override void OnReponseHitAfter(GameEvent @event)
        {
            if (m_FinishFlag) return;

            Obstacle obt = (Obstacle)@event.GetParam(1);
            if (obt.IsDead() == true && obt.Order == 3)
            {
                m_Count++;
            }
        }  

        protected override void OnReponsePlayStart(GameEvent @event)
        {
            if (m_FinishFlag) return;

            m_Count = 0;
        }

        protected override void OnReponsePlayEnd(GameEvent @event)
        {
            if (m_FinishFlag) return;

            if (m_Count == 0)
            {
                this.Finish();
            }
        }
    }
    #endregion

    #region 单回合内未击落菱形。
    public class Achievement_LING : Achievement
    {
        private int m_Count = 0;
        protected override void OnReponseHitAfter(GameEvent @event)
        {
            if (m_FinishFlag) return;

            Obstacle obt = (Obstacle)@event.GetParam(1);
            if (obt.IsDead() == true && obt.Order == 2)
            {
                m_Count++;
            }
        }  

        protected override void OnReponsePlayStart(GameEvent @event)
        {
            if (m_FinishFlag) return;

            m_Count = 0;
        }

        protected override void OnReponsePlayEnd(GameEvent @event)
        {
            if (m_FinishFlag) return;

            if (m_Count == 0)
            {
                this.Finish();
            }
        }
    }
    #endregion

    #region 单回合击落6方块。
    public class Achievement_KILLFANG : Achievement
    {
        private int m_Count = 0;
        protected override void OnReponseHitAfter(GameEvent @event)
        {
            if (m_FinishFlag) return;

            Obstacle obt = (Obstacle)@event.GetParam(1);
            if (obt.IsDead() == true && obt.Order == 0)
            {
                m_Count++;
                if (m_Count >= 6) this.Finish();
            }
        }  

        protected override void OnReponsePlayStart(GameEvent @event)
        {
            if (m_FinishFlag) return;

            m_Count = 0;
        }
    }
    #endregion


    #region 单回合击落6三角。
    public class Achievement_KILLSANJIAO : Achievement
    {
        private int m_Count = 0;
        protected override void OnReponseHitAfter(GameEvent @event)
        {
            if (m_FinishFlag) return;

            Obstacle obt = (Obstacle)@event.GetParam(1);
            if (obt.IsDead() == true && obt.Order == 1)
            {
                m_Count++;
                if (m_Count >= 6) this.Finish();
            }
        }  

        protected override void OnReponsePlayStart(GameEvent @event)
        {
            if (m_FinishFlag) return;

            m_Count = 0;
        }
    }
    #endregion


    #region 单回合击落6圆。
    public class Achievement_KILLYUAN : Achievement
    {
        private int m_Count = 0;
        protected override void OnReponseHitAfter(GameEvent @event)
        {
            if (m_FinishFlag) return;

            Obstacle obt = (Obstacle)@event.GetParam(1);
            if (obt.IsDead() == true && obt.Order == 3)
            {
                m_Count++;
                if (m_Count >= 6) this.Finish();
            }
        }  

        protected override void OnReponsePlayStart(GameEvent @event)
        {
            if (m_FinishFlag) return;

            m_Count = 0;
        }
    }
    #endregion


    #region 单回合击落6菱形。
    public class Achievement_KILLLING : Achievement
    {
        private int m_Count = 0;
        protected override void OnReponseHitAfter(GameEvent @event)
        {
            if (m_FinishFlag) return;

            Obstacle obt = (Obstacle)@event.GetParam(1);
            if (obt.IsDead() == true && obt.Order == 2)
            {
                m_Count++;
                if (m_Count >= 6) this.Finish();
            }
        }  

        protected override void OnReponsePlayStart(GameEvent @event)
        {
            if (m_FinishFlag) return;

            m_Count = 0;
        }
    }
    #endregion

    #region 单回合内击落6个碎片。
    public class Achievement_KILLGLASS : Achievement
    {
        private int m_Count = 0;
        protected override void OnReponseHitBox(GameEvent @event)
        {
            if (m_FinishFlag) return;

            Ghost ghost = (Ghost)@event.GetParam(1);
            if (ghost != null && ghost.IsDead() == true)
            {
                m_Count++;
                if (m_Count >= 6) this.Finish();
            }
        }  

        protected override void OnReponsePlayStart(GameEvent @event)
        {
            if (m_FinishFlag) return;

            m_Count = 0;
        }
    }
    #endregion


    #region 单回合击落3个炸弹
    public class Achievement_KILLBOMB : Achievement
    {
        private int m_Count = 0;
        protected override void OnReponseHitBox(GameEvent @event)
        {
            if (m_FinishFlag) return;

            Bomb bomb = (Bomb)@event.GetParam(1);
            if (bomb != null && bomb.IsDead() == true)
            {
                m_Count++;
                if (m_Count >= 3) this.Finish();
            }
        }  

        protected override void OnReponsePlayStart(GameEvent @event)
        {
            if (m_FinishFlag) return;

            m_Count = 0;
        }
    }
    #endregion

    #region 单局游戏内累计获得100金币
    public class Achievement_COIN100 : Achievement
    {
        private int m_Count = 0;
        private int m_Max   = 100;

        protected override void OnReponseCoinUpdate(GameEvent @event)
        {
            if (m_FinishFlag) return;

            int value = (int)@event.GetParam(0);
            m_Count += value;

            if (m_Count >= m_Max) this.Finish();
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
        protected override void OnReponseCoinUpdate(GameEvent @event)
        {
            if (m_FinishFlag) return;

            int value = (int)@event.GetParam(0);
            m_Count += value;

            if (m_Count >= m_Max) this.Finish();
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
        protected override void OnReponseCoinUpdate(GameEvent @event)
        {
            if (m_FinishFlag) return;

            int value = (int)@event.GetParam(0);
            m_Count += value;

            if (m_Count >= m_Max) this.Finish();
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
        protected override void OnReponseCoinUpdate(GameEvent @event)
        {
            if (m_FinishFlag) return;

            int value = (int)@event.GetParam(0);
            m_Count += value;

            if (m_Count >= m_Max) this.Finish();
        }

        protected override void OnReponseGameStart(GameEvent @event)
        {
            m_Count = 0;
        }
    }
    #endregion

    #region 单局游戏内累计击落10枚碎片
    public class Achievement_GLASS10 : Achievement
    {
        private int m_Count = 0;
        private int m_Max   = 10;
        protected override void OnReponseGlassUpdate(GameEvent @event)
        {
            if (m_FinishFlag) return;

            int value = (int)@event.GetParam(1);
            m_Count += value;

            if (m_Count >= m_Max) this.Finish();
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
        protected override void OnReponseGlassUpdate(GameEvent @event)
        {
            if (m_FinishFlag) return;

            int value = (int)@event.GetParam(1);
            m_Count += value;

            if (m_Count >= m_Max) this.Finish();
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
        protected override void OnReponseGlassUpdate(GameEvent @event)
        {
            if (m_FinishFlag) return;

            int value = (int)@event.GetParam(1);
            m_Count += value;

            if (m_Count >= m_Max) this.Finish();
        }

        protected override void OnReponseGameStart(GameEvent @event)
        {
            if (m_FinishFlag) return;

            m_Count = 0;
        }
    }
    #endregion

    #region 单局游戏内累计击落10枚炸弹(开局赠送炸弹弹珠)
    public class Achievement_REVERSEBOMB : Achievement
    {
        private int m_Count = 0;
        private int m_Max   = 10;
        protected override void OnReponseGameStart(GameEvent @event)
        {
            if (m_FinishFlag) return;

            m_Count = 0;
        }

        protected override void OnReponseHitBox(GameEvent @event)
        {
            if (m_FinishFlag) return;

            Bomb bomb = (Bomb)@event.GetParam(1);
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

        protected override void OnReponsePlayEnd(GameEvent @event)
        {
            if (m_FinishFlag) return;

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

        protected override void OnReponsePlayEnd(GameEvent @event)
        {
            if (m_FinishFlag) return;

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

        protected override void OnReponsePlayEnd(GameEvent @event)
        {
            if (m_FinishFlag) return;

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

        protected override void OnReponsePlayEnd(GameEvent @event)
        {
            if (m_FinishFlag) return;

            if (GameFacade.Instance.User.Score >= m_Max)
            {
                this.Finish();
            }
        }

    }
    #endregion

    #region 单次发射击落5枚碎片。

    #endregion











    //成就
    public class Achievement
    {
        private AchievementData m_Data;
        public int ID { get{return m_Data.ID;}}
        public string Description { get{return m_Data.Description;}}

        protected bool m_FinishFlag = false;

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

        public void Finish()
        {
            if (m_FinishFlag == true) return;

            //存储
            m_FinishFlag = true;

            Debug.Log("完成成就:" + ID);

            //通知UI 成就完成
            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_ACHIEVEMENTPOP, this));
        }

        public void Dispose()
        {
            
        }

        void Init(AchievementData data)
        {
            m_Data = data;

            GameFacade.Instance.EventManager.AddHandler(EVENT.ONGAMESTART,      OnReponseGameStart);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONGAMEEND,        OnReponseGameEnd);

            GameFacade.Instance.EventManager.AddHandler(EVENT.ONPLAYSTART,      OnReponsePlayStart);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONPLAYEND,        OnReponsePlayEnd);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONBALLHITAFTER,   OnReponseHitAfter);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONBALLHITBOX,     OnReponseHitBox);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONCOINUPDATE,     OnReponseCoinUpdate);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONGLASSUPDATE,    OnReponseGlassUpdate);
            
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

