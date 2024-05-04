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
            EventManager.AddHandler(EVENT.ONAFTERDRAWOBT,   OnReponseAfterDrawObstacles);
        }

        void OnDestroy()
        {
            EventManager.DelHandler(EVENT.ONAFTERDRAWOBT,   OnReponseAfterDrawObstacles);
        }

        public override string GetDescription()
        {
            return "场上不再生成<sprite=0>。";
        }

        void OnReponseAfterDrawObstacles(GameEvent gameEvent)
        {
            List<int> lists = (List<int>)gameEvent.GetParam(0);

            for (int i = lists.Count - 1; i >= 0; i--)
            {
                if (lists[i] == (int)_C.BOXTYPE.GHOST)
                {
                    lists.RemoveAt(i);
                }
            }
        }
    }
}