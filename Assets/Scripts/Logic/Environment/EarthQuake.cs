using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;



namespace CB
{
    /// <summary>
    /// 地震 受地震影响，关卡内的宝石时不时会发生位移 
    /// </summary>
    public class EarthQuake : Environment
    {
        private CDTimer m_CDTimer = new CDTimer(2.0f);

        public EarthQuake()
        {
            Name = "地震";
        }

        public override string GetDescription()
        {
            return "受地震影响，宝石时不时会发生位移。";
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            m_CDTimer.Reset(RandomUtility.Random(100, 500) / 100.0f);
        }

        void Update()
        {
            m_CDTimer.Update(Time.deltaTime);
            if (m_CDTimer.IsFinished() == true) {
                m_CDTimer.Reset(RandomUtility.Random(150, 500) / 100.0f);

                Camera.main.transform.DOShakePosition(1.2f, 0.1f, 5, 50);

                GameFacade.Instance.Game.Obstacles.ForEach(obt => {
                    var angle   = RandomUtility.Random(0, 360);
                    var direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0).normalized * 0.4f;
                    var o_pos   = obt.transform.localPosition;
                    var to_pos  = o_pos + direction;

                    if (to_pos.x < _C.BOARD_LEFT || to_pos.x > _C.BOARD_RIGHT)
                    {
                        direction = new Vector3(direction.x * -1f, direction.y, 0);
                    } 

                    if (to_pos.y < _C.BOARD_BOTTOM || to_pos.y > _C.BOARD_TOP)
                    {
                        direction = new Vector3(direction.x, direction.y * -1f, 0);
                    }

                    obt.Move(o_pos + direction);
                });
            }
        }
    }
}