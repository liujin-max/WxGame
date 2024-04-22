using System.Collections;
using System.Collections.Generic;
using CB;
using DG.Tweening;
using UnityEngine;

public class BlackHole : Box
{
    private float m_Radius = 2f;
    private CDTimer m_LifeTimer = new CDTimer(1f);

    void Start () 
    {
        var o_pos = transform.localPosition;

        GameFacade.Instance.Game.Obstacles.ForEach(obstacle => {
            if (Vector3.Distance(o_pos, obstacle.transform.localPosition) <= m_Radius)
            {
                //使宝石无效
                obstacle.SetValid(false);

                // 将对象移向黑洞
                obstacle.transform.DOLocalMove(o_pos, 1f).SetEase(Ease.InOutQuad);
                obstacle.transform.DOScale(Vector3.zero, 1f).OnComplete(()=>{
                    obstacle.RemoveShield();
                    obstacle.OnHit(null, obstacle.HP);
                    obstacle.ForceDead();
                });
            }
        });
    }

    void Update()
    {
        m_LifeTimer.Update(Time.deltaTime);
        if (m_LifeTimer.IsFinished() == true)
        {
            GameFacade.Instance.EffectManager.Load(EFFECT.BLACKHOLEBOOM, transform.localPosition);
            this.Dead();
        }
    }
}
