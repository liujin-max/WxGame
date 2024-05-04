using System.Collections;
using System.Collections.Generic;
using CB;
using UnityEngine;


namespace CB
{
    //水晶弹珠
    public class BallGlass : Ball
    {
        private int m_Glass = 1;

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
            return string.Format("回合结束时若留在发射槽中，则获得{0}<sprite={1}>。", m_Glass, (int)_C.SPRITEATLAS.GLASS);
        }


        void OnReponsePlayEnd(GameEvent gameEvent)
        {
            GameFacade.Instance.Game.PushGlass(m_Glass);

            var item = GameFacade.Instance.Game.GameUI.GetBallSeat(this);
            if (item != null) {
                GameFacade.Instance.EffectManager.LoadUIEffect(EFFECT.FLYGLASS, item.transform.position);
            }
            
        }
    }
}

