using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CB;
using UnityEngine;


namespace CB
{
    //蓄力弹珠
    public class BallPower : Ball
    {
        private int m_Count = 0;

        //导出数据
        public override string Export()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append((int)Type);
            sb.Append(",");
            sb.Append(m_Count);

            return sb.ToString();
        }

        //同步存档
        public override void Sync(string record)
        {
            string[] elements = record.Split(',');
            m_Count = Convert.ToInt32(elements[1]);

            Demage.PutADD(this, m_Count);
        }

        public override void Init(_C.BALLTYPE type)
        {
            base.Init(type);

            EventManager.AddHandler(EVENT.ONPLAYEND,        OnReponsePlayEnd);
        }

        void OnDestroy()
        {
            EventManager.DelHandler(EVENT.ONPLAYEND,        OnReponsePlayEnd);
        }

        public override string GetDescription()
        {
            return string.Format("回合结束时若留在弹珠槽中，则伤害提高1点。");
        }


        void OnReponsePlayEnd(GameEvent gameEvent)
        {
            m_Count++;
            Demage.PutADD(this, m_Count);

            var seat_item = GameFacade.Instance.Game.GameUI.GetBallSeat(this);
            if (seat_item != null) {
                seat_item.ShowFadeScale();
            }
        }
    }
}

