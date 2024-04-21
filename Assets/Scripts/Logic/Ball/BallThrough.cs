using System.Collections;
using System.Collections.Generic;
using CB;
using UnityEngine;


namespace CB
{
    //引力弹珠
    public class BallThrough : Ball
    {
        private HashSet<Box> m_Boxs = new HashSet<Box>();

        public override string GetDescription()
        {
            var str = string.Format("弹珠飞行途中将吸引周围的宝石。");

            return str;
        }

        public override void LateUpdate()
        {
            base.LateUpdate();

            foreach (var box in m_Boxs) {
                box.Stop();
            }
            m_Boxs.Clear();

            var o_pos = transform.localPosition;
            GameFacade.Instance.Game.Obstacles.ForEach(obstacle => {
                var distance = Vector3.Distance(o_pos, obstacle.transform.localPosition);
                if (distance <= 2 && distance >= this.ColliderRadius + obstacle.ColliderRadius) {
                    m_Boxs.Add(obstacle);
                    
                    obstacle.Move(o_pos, 0.15f);
                }
            });
        }
    }
}

