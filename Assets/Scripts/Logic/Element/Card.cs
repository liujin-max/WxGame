using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Card
{
    private CardData m_Data;
    public CardData Data {get {return m_Data;}}

    public int ID {get {return m_Data.ID;}}
    public string Name {get {return m_Data.Name;}}
    public _C.CARD_TYPE TYPE {get {return m_Data.Type;}}

    //状态
    public _C.CARD_STATE STATE;


    //当前格子
    private Grid m_Grid;
    public Grid Grid {
        get {return m_Grid;} 
        set {m_Grid = value;}
    }

    //存储各种临时状态
    private CardState m_StateFlag;
    public CardState StateFlag {
        get {return m_StateFlag;} 
        set {m_StateFlag = value;}
    }

    //实体
    private Jelly m_Entity;
    public Jelly Entity {
        get {return m_Entity;}
    }

   


    //可拖动(不可拖动)
    private bool m_Dragable = true;
    public bool Dragable {
        get {
            if (TYPE == _C.CARD_TYPE.FRAME) return false;
            if (STATE == _C.CARD_STATE.GHOST) return false;
            if (m_StateFlag.IsEliminating || IsFixed) return false;

            return m_Dragable;
        }
        set {m_Dragable = value;}
    }

    //是否固定的(不可拖动，也不可受连锁反应而移动)
    private bool m_IsFixed = false;
    public bool IsFixed {
        get {
            if (TYPE == _C.CARD_TYPE.FRAME) return true;
            return m_IsFixed;
        }
    }

    //不受传送带移动
    public bool IsInValidByBelt {
        get {
            if (this.ID == (int)_C.CARD.PORTAL) return true;
            
            return false;
        }
    }
    
    //可击破的
    //石块只可被炸弹之类的击破
    public bool Breakable {get {return m_Data.Breakable;}}


    //本次移动所经过的格子
    public List<Grid> CrossGrids = new List<Grid>();
   










    public Card(CardData cardData)
    {
        m_Data  = cardData;

        m_StateFlag = new CardState(this);
    }

    public void Display()
    {
        m_Entity= GameFacade.Instance.UIManager.LoadPrefab("Prefab/Element/Jelly", Vector3.zero, Field.Instance.Land.ENTITY_ROOT).GetComponent<Jelly>();
        m_Entity.SetPosition(m_Grid.Position);
        m_Entity.Init(this);
    }

    public void SetPosition(Vector2 pos)
    {
        if (m_Entity == null) return;
        m_Entity.SetPosition(pos);
    }

    public Vector2 GetPosition()
    {
        if (m_Entity == null) return m_Grid.Position;

        return m_Entity.transform.localPosition;
    }

    public bool IsBomb()
    {
        return this.ID == (int)_C.CARD.MISSILE || this.ID == (int)_C.CARD.BOMB;
    }

    public void DoBomb()
    {
        m_StateFlag.DeadType = _C.DEAD_TYPE.BOMB;
        m_StateFlag.IsReady2Eliminate = true;
    }

    //移动中处理
    public void OnMove(Grid grid, _C.DIRECTION direction)
    {
        //飞弹
        //清除经过路径上的所有方块
        if (this.ID == (int)_C.CARD.MISSILE)
        {
            foreach (var card in Field.Instance.Cards) {
                if (card.Grid == grid && card != this && card.Breakable == true) {
                    card.DoBomb();
                    Field.Instance.RemoveCard(card);
                    GameFacade.Instance.DisplayEngine.Put(DisplayEngine.Track.Common, new DisplayEvent_BrokenCard(card));

                    break;
                } 
            }
        }
    }

    //移动后的处理
    public void OnAfterMove(_C.DIRECTION direction)
    {
        if (IsBomb() == true)
        {
            DoBomb();

            //炸弹
            //清除目标区域2格范围内所有方块
            if (this.ID == (int)_C.CARD.BOMB)
            {
                foreach (var card in Field.Instance.Cards) {
                    if (card != this && card.Breakable == true) {
                        if (Mathf.Abs(card.Grid.X - m_Grid.X) + Mathf.Abs(card.Grid.Y - m_Grid.Y) <= 2) {
                            card.DoBomb();
                        }
                    } 
                }
            }
        }
    }


    //受到连锁反应
    public void OnChain(_C.DIRECTION direction)
    {
        //木箱
        //爆炸且在原地生成新的方块
        if (this.ID == (int)_C.CARD.WOOD)   
        {
            m_StateFlag.DeadType = _C.DEAD_TYPE.DIGESTE;
            m_StateFlag.IsReady2Eliminate = true;

            Field.Instance.RemoveCard(this);
            GameFacade.Instance.DisplayEngine.Put(DisplayEngine.Track.Common, new DisplayEvent_BrokenCard(this));

            return;
        }

        //普通方块会被推走逻辑
        Field.Instance.Move(this, direction);
    }

    //连锁反应结束后
    public void OnAfterChain()
    {
        //衍生物
        if (m_StateFlag.DerivedID > 0) {
            Field.Instance.PutCard(_C.CARD_STATE.NORMAL, GameFacade.Instance.DataCenter.GetCardData(m_StateFlag.DerivedID), m_Grid);   
        }


    }

    //击破后
    public void OnAfterBroken()
    {
        //木箱
        //爆炸且在原地生成新的方块
        if (this.ID == (int)_C.CARD.WOOD)   
        {
            int rand    = RandomUtility.Random(0, Field.Instance.Stage.Cards.Count);
            Field.Instance.PutCard(_C.CARD_STATE.NORMAL, Field.Instance.Stage.Cards[rand], m_Grid, true);
        }

        //连击奖励
        if (m_StateFlag.Combo >= 3)
        {
            if (Field.Instance.Stage.NeedCheckStep()) {
                Field.Instance.Stage.UpdateMoveStep(1);
                EventManager.SendEvent(new GameEvent(EVENT.UI_UPDATESTEP, true));

                GameFacade.Instance.EffectManager.Load(EFFECT.FLYSTEP, m_Entity.transform.position);
            } 
            else if (Field.Instance.Stage.NeedCheckTimer()) {
                Field.Instance.Stage.UpdateCountDown(5);
                EventManager.SendEvent(new GameEvent(EVENT.UI_UPDATETIME, true));

                GameFacade.Instance.EffectManager.Load(EFFECT.FLYTIME, m_Entity.transform.position);
            } 
            else {
                //收集
                Field.Instance.Stage.Collect(this.ID, 1);
                EventManager.SendEvent(new GameEvent(EVENT.UI_UPDATESCORE));
            }
        }
    }

    public void Dispose()
    {
        // Debug.Log("销毁：" + this.Name + " => " + m_Grid.X + ", " + m_Grid.Y);

        if (m_Grid != null) {
            if (m_Grid.Card == this) {
                m_Grid.Card = null;
            }
        }

        // if (m_Entity == null) return;
        
        m_Entity.Dispose();
        m_Entity = null;

    } 
}
