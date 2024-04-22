using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace CB
{
    /// <summary>
    /// 
    /// </summary>
    public class BanSwitch : Environment
    {
        public BanSwitch()
        {
            Name = "禁锢";
        }

        public override string GetDescription()
        {
            return "无法更换弹珠的发射顺序。";
        }


        public override void OnEnter()
        {
            base.OnEnter();

            GameFacade.Instance.Game.SwitchBanFlag = true;

            var seat_pivot = GameFacade.Instance.Game.GameUI.GetSeatPivot();

            ImageGray imageGray = seat_pivot.GetComponent<ImageGray>();
            if (imageGray == null) {
                imageGray = seat_pivot.AddComponent<ImageGray>();
            }
            imageGray.TurnGray(true);

            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_SHOWBALLLIST, false));
        }

        public override void OnLeave()
        {
            base.OnLeave();

            GameFacade.Instance.Game.SwitchBanFlag = false;

            var seat_pivot = GameFacade.Instance.Game.GameUI.GetSeatPivot();
            ImageGray imageGray = seat_pivot.GetComponent<ImageGray>();
            if (imageGray == null) {
                imageGray = seat_pivot.AddComponent<ImageGray>();
            }
            imageGray.TurnGray(false);
        }
    }
}