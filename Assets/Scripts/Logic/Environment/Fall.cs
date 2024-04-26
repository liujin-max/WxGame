using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;



namespace CB
{
    /// <summary>
    /// 
    /// </summary>
    public class Fall : Environment
    {
        private CDTimer m_CDTimer = new CDTimer(1.6f);
        private int m_HPMin = 10000;
        private int m_HPMax = 0;

        public Fall()
        {
            Name = "坠落";
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
            return "场上的宝石不停的坠落。";
        }

        public override void OnEnter()
        {
            base.OnEnter();

            GameFacade.Instance.Game.Obstacles.ForEach(o =>{
                if (o.HP < m_HPMin) m_HPMin = o.HP;
                if (o.HP > m_HPMax) m_HPMax = o.HP;
            });
        }

        void OnReponseAfterDrawObstacles(GameEvent gameEvent)
        {
            List<int> lists = (List<int>)gameEvent.GetParam(0);

            for (int i = lists.Count - 1; i >= 0; i--)
            {
                if (lists[i] < 0)
                {
                    lists.RemoveAt(i);
                }
            }
        }

        void Update()
        {
            if(m_IsEnter != true) return;
            if (GameFacade.Instance.Game.PauseFlag == true) return;


            m_CDTimer.Update(Time.deltaTime);
            if (m_CDTimer.IsFinished() == true)
            {
                m_CDTimer.Reset();

                int random = RandomUtility.Random(2, 6);

                if (random > 0) {
                    Vector2 topLeft     = new Vector2(_C.BOARD_LEFT, _C.BOARD_TOP + 0.8f);
                    Vector2 bottomRight = new Vector2(_C.BOARD_RIGHT, _C.BOARD_TOP - 0.2f);

                    Vector2[] randomPoints = ToolUtility.GenerateRandomPoints(topLeft, bottomRight, random, _C.OBSTACLE_OFFSET);

                    foreach (var pos in randomPoints) {
                        if (RandomUtility.IsHit(15) == true) {
                            GameFacade.Instance.Game.PushGhost((Vector3)pos);
                        } else {
                            var o = GameFacade.Instance.Game.PushObstacle((Vector3)pos, RandomUtility.Random(m_HPMin, m_HPMax));
                            o.DoScale();
                        }
                    }
                }
            }

            GameFacade.Instance.Game.Obstacles.ForEach(o =>{
                o.transform.localPosition += new Vector3(0, -1 * Time.deltaTime, 0);

                if (o.transform.localPosition.y <= (_C.BOARD_BOTTOM - 1)) {
                    o.ForceDead();
                }
            });

            for (int i = GameFacade.Instance.Game.Boxs.Count - 1; i >= 0; i--)
            {
                var box = GameFacade.Instance.Game.Boxs[i];
                box.transform.localPosition += new Vector3(0, -1 * Time.deltaTime, 0);

                if (box.transform.localPosition.y <= (_C.BOARD_BOTTOM - 1)) {
                    box.Dispose();
                    GameFacade.Instance.Game.Boxs.Remove(box);
                }
            }
        }
    }
}