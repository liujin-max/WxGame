using System.Collections;
using System.Collections.Generic;
using CB;
using DG.Tweening;
using UnityEngine;


namespace CB
{
    //震荡
    public class BallWave : Ball
    {
        private int m_Count = 0;

        private GameObject m_Effect;

        public override void UpgradeTo(int level)
        {
            base.UpgradeTo(level);

            this.Demage.SetBase(0);
        }

        public override string GetDescription()
        {
            var str = "弹珠没有伤害。击中宝石为自身蓄能，积攒能量后释放震荡波，对全场宝石造成1点伤害";

            return str;
        }


        //碰撞逻辑
        public override void OnCollisionEnter2D(Collision2D collision)
        {
            this.TriggerEnter(collision);
            
            this.OnHitElement(collision);
            
            if (collision.transform.GetComponent<Obstacle>() != null)
            {
                m_Count++;

                if (m_Effect == null) {
                    m_Effect = GameFacade.Instance.EffectManager.Load(EFFECT.ENCHANT, Vector3.zero, gameObject);
                    m_Effect.GetComponent<Effect>().SetCallback(()=>{
                        m_Effect = null;
                    });
                }

                

                if (m_Count >= 10)
                {
                    m_Count = 0;

                    //释放全场冲击波  
                    GameFacade.Instance.EffectManager.Load(EFFECT.NOVA, transform.localPosition);
                    
                    Camera.main.transform.DOShakePosition(0.3f, 0.2f, 15, 50);

                    GameFacade.Instance.Game.Obstacles.ForEach(obt => {
                        obt.OnHit(null, 1);
                    });

                }
            }
        }
    }
}

