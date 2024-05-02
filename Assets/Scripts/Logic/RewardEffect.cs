using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;


namespace CB
{
    #region 金币
    public class RewardEffect_Coin : RewardEffect
    {
        public override int GetCoin()
        {
            return m_Value;
        }

        public override void DoReward()
        {
            GameFacade.Instance.Game.UpdateCoin(m_Value);
        }
    }
    #endregion


    #region 碎片
    public class RewardEffect_Glass : RewardEffect
    {
        public override int GetGlass()
        {
            return m_Value;
        }

        public override void DoReward()
        {
            GameFacade.Instance.Game.PushGlass(m_Value);
        }
    }
    #endregion


    #region 弹珠
    public class RewardEffect_Ball : RewardEffect
    {
        public override void DoReward()
        {
            GameFacade.Instance.Game.BreechBall(GameFacade.Instance.Game.PushBall(_C.BALL_ORIGIN_POS, (_C.BALLTYPE)m_Value));
        }
    }
    #endregion


    #region 道具
    public class RewardEffect_Relics : RewardEffect
    {
        public override void DoReward()
        {
            GameFacade.Instance.Game.Army.PushRelics(m_Value);
        }
    }
    #endregion


    #region 解锁道具
    public class RewardEffect_Unlock : RewardEffect
    {
        public override void DoReward()
        {
            RelicsData data = GameFacade.Instance.Game.GetRelicsData(m_Value);
            data.Unlock = true;
        }
    }
    #endregion









    //奖励效果器
    public class RewardEffect
    {
        protected string m_Name;
        protected int m_Value;

        private static Dictionary<string, Func<RewardEffect>> m_classDictionary = new Dictionary<string, Func<RewardEffect>> {
            { "金币", () => new RewardEffect_Coin()},
            { "碎片", () => new RewardEffect_Glass()},
            { "弹珠", () => new RewardEffect_Ball()},
            { "道具", () => new RewardEffect_Relics()},
            { "解锁", () => new RewardEffect_Unlock()},
        };

        public static RewardEffect Create(string name, int value)
        {
            RewardEffect effect = null;

            if (m_classDictionary.ContainsKey(name)) {
                effect = m_classDictionary[name]();
            } else {
                effect = new RewardEffect();
            }

            effect.Init(name, value);

            return effect;
        }

        void Init(string name, int value)
        {
            m_Name  = name;
            m_Value = value;
        }

        public virtual int GetCoin()
        {
            return 0;
        }

        public virtual int GetGlass()
        {
            return 0;
        }

        public virtual void DoReward()
        {

        }
    }
}
