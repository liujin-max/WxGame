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
            EventManager.AddHandler(EVENT.ONDRAWINGOBSTACLE,    OnReponseDrawingObstacles);
        }

        void OnDestroy()
        {
            EventManager.DelHandler(EVENT.ONDRAWINGOBSTACLE,    OnReponseDrawingObstacles);
        }

        public override string GetDescription()
        {
            return "发现地下矿脉，场上出现大量<sprite=0>。";
        }

        public override void OnEnter()
        {
            base.OnEnter();

            GameFacade.Instance.Game.Obstacles.ForEach(obstacle =>{
                obstacle.SetHP(obstacle.HP * 2);
            });
        }

        void OnReponseDrawingObstacles(GameEvent gameEvent)
        {
            List<int> lists = (List<int>)gameEvent.GetParam(0);

            for (int i = 0; i < 5; i++)
            {
                lists.Add((int)_C.BOXTYPE.GHOST);
            }
        }
    }
}