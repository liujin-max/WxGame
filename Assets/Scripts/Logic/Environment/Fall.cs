using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace CB
{
    /// <summary>
    /// 
    /// </summary>
    public class Fall : Environment
    {
        public Fall()
        {
            Name = "下坠";
        }

        public override string GetDescription()
        {
            return "场上的宝石不停的下坠。";
        }
        
        void Update()
        {
            if(m_IsEnter != true) return;


            GameFacade.Instance.Game.Obstacles.ForEach(o =>{
                o.transform.localPosition += new Vector3(0, -1 * Time.deltaTime, 0);

                if (o.transform.localPosition.y <= (_C.BOARD_BOTTOM - 1)) {
                    o.transform.localPosition += new Vector3(0, _C.BOARD_TOP + Math.Abs(_C.BOARD_BOTTOM) + 1, 0);
                    o.DoScale();
                }
            });

            GameFacade.Instance.Game.Boxs.ForEach(g =>{
                g.transform.localPosition += new Vector3(0, -1 * Time.deltaTime, 0);

                if (g.transform.localPosition.y <= (_C.BOARD_BOTTOM - 1)) {
                    g.transform.localPosition += new Vector3(0, _C.BOARD_TOP + Math.Abs(_C.BOARD_BOTTOM) + 1, 0);
                    g.DoScale();
                }
            });
        }
    }
}