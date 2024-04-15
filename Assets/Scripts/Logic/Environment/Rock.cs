using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;



namespace CB
{
    /// <summary>
    /// 岩石
    /// </summary>
    public class Rock : Environment
    {
        public Rock()
        {
            Name = "岩石";
        }

        void Awake()
        {
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONAFTERDRAWOBT,   OnReponseAfterDrawObstacles);
        }

        void OnDestroy()
        {
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONAFTERDRAWOBT,   OnReponseAfterDrawObstacles);
        }

        public override string GetDescription()
        {
            return "进入岩石层，场上不再生成<sprite=0>。";
        }

        public override void OnEnter()
        {

        }

        public override void OnLeave()
        {

        }

        void OnReponseAfterDrawObstacles(GameEvent gameEvent)
        {
            List<int> lists = (List<int>)gameEvent.GetParam(0);

            for (int i = lists.Count - 1; i >= 0; i--)
            {
                if (lists[i] == -1)
                {
                    lists.RemoveAt(i);
                }
            }
        }
    }
}