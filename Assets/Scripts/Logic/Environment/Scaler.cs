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
    public class Scaler : Environment
    {
        public Scaler()
        {
            Name = "枯竭";
        }

        public override string GetDescription()
        {
            return "场上的宝石比平时小一圈。";
        }

        public override void OnEnter()
        {
            GameFacade.Instance.Game.Obstacles.ForEach(obstacle => {
                obstacle.m_Scale.PutAUL(this, -0.15f);
                obstacle.JudgeScale();
            });
        }

        public override void OnLeave()
        {

        }
    }
}