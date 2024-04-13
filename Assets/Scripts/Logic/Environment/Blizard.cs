using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;



namespace CB
{
    /// <summary>
    /// 水晶矿脉 发现地下矿脉，场上产生大量<sprite=0>。
    /// </summary>
    public class Blizard : Environment
    {
        public Blizard()
        {
            Name = "水晶矿脉";
        }

        void Awake()
        {
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONDRAWINGOBSTACLE,    OnReponseDrawingObstacles);
        }

        void OnDestroy()
        {
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONDRAWINGOBSTACLE,    OnReponseDrawingObstacles);
        }

        public override string GetDescription()
        {
            return "发现地下矿脉，场上出现大量<sprite=0>。";
        }

        public override void OnEnter()
        {
            GameFacade.Instance.Game.Obstacles.ForEach(obstacle =>{
                obstacle.SetHP((int)(obstacle.HP * 1.5f));
            });
        }

        public override void OnLeave()
        {

        }

        void OnReponseDrawingObstacles(GameEvent gameEvent)
        {
            List<int> lists = (List<int>)gameEvent.GetParam(0);

            for (int i = 0; i < 8; i++)
            {
                lists.Add(-1);
            }
        }
    }
}