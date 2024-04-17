using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;


namespace CB
{
    // 装备库 负责管理我拥有的装备
    // 魂模块：每把最多携带3个魂
    public class Army
    {
        private List<Relics> m_Relicses = new List<Relics>();
        private Dictionary<int, Relics> m_RelicsDic = new Dictionary<int, Relics>();


        public AttributeValue SeatCount = new AttributeValue(99);


        public void Awake()
        {
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONPLAYEND,        OnReponsePlayEnd);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONBALLSHOOT,      OnReponseBallShoot);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONBALLHITBEFORE,  OnReponseHitBefore);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONBALLHITAFTER,   OnReponseHitAfter);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONCOMPLEXINIT,    OnReponseComplextInit);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONBALLPUSH,       OnReponseBallPush);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONDRAWINGOBSTACLE,OnReponseDrawingObstacles);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONGLASSUPDATE,    OnReponseGlassUpdate);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONCOINUPDATE,     OnReponseCoinUpdate);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONBALLHITBOX,     OnReponseHitBox);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONENTERCOLLISION, OnReponseEnterCollision);
            
        }

        public void Dispose()
        {
            m_Relicses.ForEach(r => {
                r.Unload();
            });
            m_Relicses.Clear();
            m_RelicsDic.Clear();
            SeatCount.Clear();


            GameFacade.Instance.EventManager.DelHandler(EVENT.ONPLAYEND,        OnReponsePlayEnd);
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONBALLSHOOT,      OnReponseBallShoot);
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONBALLHITBEFORE,  OnReponseHitBefore);
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONBALLHITAFTER,   OnReponseHitAfter);
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONCOMPLEXINIT,    OnReponseComplextInit);
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONBALLPUSH,       OnReponseBallPush);
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONDRAWINGOBSTACLE,OnReponseDrawingObstacles);
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONGLASSUPDATE,    OnReponseGlassUpdate);
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONCOINUPDATE,     OnReponseCoinUpdate);
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONBALLHITBOX,     OnReponseHitBox);
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONENTERCOLLISION, OnReponseEnterCollision);
        }

        public Relics PushRelics(int id)
        {
            if (m_Relicses.Count >= SeatCount.ToNumber()) return null;

            RelicsData data = CONFIG.GetRelicsData(id);
            Relics relics   = new Relics(data);
            relics.Equip();
            m_Relicses.Add(relics);
            m_RelicsDic.Add(id, relics);

            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHRELICS));

            return relics;
        }

        public void RemoveRelics(Relics relics)
        {
            relics.Unload();
            m_Relicses.Remove(relics);
            m_RelicsDic.Remove(relics.ID);

            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHRELICS));
        }

        public List<Relics> GetRelicses()
        {
            return m_Relicses;
        }

        public Relics GetRelics(int id)
        {
            Relics relics;
            if (m_RelicsDic.TryGetValue(id, out relics)) {
                return relics;
            }
            return null;
        }

        public bool IsFull()
        {
            return m_Relicses.Count >= SeatCount.ToNumber();
        }


        //////////////////////////////////////////

        void OnReponsePlayEnd(GameEvent gameEvent)
        {
            m_Relicses.ForEach(relics => {
                relics.GetEffects().ForEach(e => {
                    e.OnPlayEnd();
                });
            });
        }

        ///发射宝石
        void OnReponseBallShoot(GameEvent gameEvent)
        {
            m_Relicses.ForEach(relics => {
                relics.GetEffects().ForEach(e => {
                    e.OnBallShoot((Ball)gameEvent.GetParam(0), (bool)gameEvent.GetParam(1));
                });
            });
        }

        //击中宝石前
        void OnReponseHitBefore(GameEvent gameEvent)
        {
            m_Relicses.ForEach(relics => {
                relics.GetEffects().ForEach(e => {
                    e.OnHitBefore((Ball)gameEvent.GetParam(0), (Obstacle)gameEvent.GetParam(1), (Collision2D)gameEvent.GetParam(2));
                });
            });
        }

        //击中宝石后
        void OnReponseHitAfter(GameEvent gameEvent)
        {
            m_Relicses.ForEach(relics => {
                relics.GetEffects().ForEach(e => {
                    e.OnHitAfter((Ball)gameEvent.GetParam(0), (Obstacle)gameEvent.GetParam(1), (Collision2D)gameEvent.GetParam(2));
                });
            });
        }

        void OnReponseComplextInit(GameEvent gameEvent)
        {
            m_Relicses.ForEach(relics => {
                relics.GetEffects().ForEach(e => {
                    e.OnComplextInit((ComplextEvent)gameEvent.GetParam(0), (BallData)gameEvent.GetParam(1));
                });
            });
        }

        void OnReponseBallPush(GameEvent gameEvent)
        {
            m_Relicses.ForEach(relics => {
                relics.GetEffects().ForEach(e => {
                    e.OnPushBall((Ball)gameEvent.GetParam(0));
                });
            });
        }

        void OnReponseDrawingObstacles(GameEvent gameEvent)
        {
             m_Relicses.ForEach(relics => {
                relics.GetEffects().ForEach(e => {
                    e.OnDrawingObstacles((List<int>)gameEvent.GetParam(0));
                });
            });
        }

        void OnReponseGlassUpdate(GameEvent gameEvent)
        {
            m_Relicses.ForEach(relics => {
                relics.GetEffects().ForEach(e => {
                    e.OnGlassUpdate((int)gameEvent.GetParam(0));
                });
            });
        }

        void OnReponseCoinUpdate(GameEvent gameEvent)
        {
            m_Relicses.ForEach(relics => {
                relics.GetEffects().ForEach(e => {
                    e.OnCoinUpdate((int)gameEvent.GetParam(0));
                });
            });
        }

        //击中碎片后
        void OnReponseHitBox(GameEvent gameEvent)
        {
            m_Relicses.ForEach(relics => {
                relics.GetEffects().ForEach(e => {
                    e.OnHitBox((Ball)gameEvent.GetParam(0), (Box)gameEvent.GetParam(1), (Collision2D)gameEvent.GetParam(2));
                });
            });
        }

        //碰撞物体
        void OnReponseEnterCollision(GameEvent gameEvent)
        {
            m_Relicses.ForEach(relics => {
                relics.GetEffects().ForEach(e => {
                    e.OnEnterCollision((Ball)gameEvent.GetParam(0), (Collision2D)gameEvent.GetParam(1));
                });
            });
        }
    }
}

