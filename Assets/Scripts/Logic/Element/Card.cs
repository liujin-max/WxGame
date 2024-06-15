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

    //当前格子
    private Grid m_Grid;
    public Grid Grid {
        get {return m_Grid;} 
        set {m_Grid = value;}
    }

    //实体
    private Jelly m_Entity;
    public Jelly Entity {
        get {return m_Entity;}
    }

    //状态
    public _C.CARD_STATE STATE;

    private _C.DEAD_TYPE m_DeadType = _C.DEAD_TYPE.NORMAL;
    public _C.DEAD_TYPE DeadType {get {return m_DeadType;}}

    //是否固定的(不可拖动，也不可移动)
    private bool m_IsFixed = false;
    public bool IsFixed {
        get {
            if (TYPE == _C.CARD_TYPE.FRAME) return true;
            return m_IsFixed;
        }
    }

    //可拖动(不可拖动，但是不代表不能移动(可被连锁反应推动))
    private bool m_Dragable = true;
    public bool Dragable {
        get {
            if (TYPE == _C.CARD_TYPE.FRAME) return false;
            if (STATE == _C.CARD_STATE.GHOST) return false;
            if (IsEliminating || IsFixed) return false;

            return m_Dragable;
        }
        set {m_Dragable = value;}
    }

    //可击破的
    //石块只可被炸弹之类的击破
    public bool Breakable {get {return m_Data.Breakable;}}

    //本次移动所经过的格子
    public List<Grid> CrossGrids = new List<Grid>();
   
    //死亡分解中
    public bool IsEliminating = false;
    //衍生物ID
    public int DerivedID = -1;
    //准备分解
    public bool IsReady2Eliminate = false;


    //消除所处的方块link
    public Card Link;









    public Card(CardData cardData)
    {
        m_Data = cardData;
    }

    public void Display()
    {
        m_Entity= GameFacade.Instance.UIManager.LoadPrefab("Prefab/Element/Jelly", Vector3.zero, Field.Instance.Land.ENTITY_ROOT).GetComponent<Jelly>();
        m_Entity.SetPosition(m_Grid.Position);
        m_Entity.Init(this);
    }

    public bool IsBomb()
    {
        return this.ID == (int)_C.CARD.MISSILE || this.ID == (int)_C.CARD.BOMB;
    }

    public void DoBomb()
    {
        m_DeadType = _C.DEAD_TYPE.BOMB;
        IsReady2Eliminate = true;
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

    //连锁反应
    public void OnChain(_C.DIRECTION direction)
    {
        //木箱
        //爆炸且在原地生成新的方块
        if (this.ID == (int)_C.CARD.WOOD)   
        {
            m_DeadType = _C.DEAD_TYPE.DIGESTE;
            IsReady2Eliminate = true;
            return;
        }

        //普通方块会被推走逻辑
        Field.Instance.Move(this, direction);
    }

    //击破后
    public void OnAfterBroken()
    {
        //木箱
        //爆炸且在原地生成新的方块
        if (this.ID == (int)_C.CARD.WOOD)   
        {
            int rand    = RandomUtility.Random(0, Field.Instance.Stage.Cards.Count);
            Card card   = Field.Instance.PutCard(_C.CARD_STATE.NORMAL, Field.Instance.Stage.Cards[rand], m_Grid, true);

            // Debug.Log("木箱爆炸 产生：" + card.Name + " => " + card.Grid.X + ", " + card.Grid.Y);
        }
    }

    public void Dispose()
    {
        if (m_Grid != null) {
            if (m_Grid.Card == this) {
                m_Grid.Card = null;
            }
        }

        m_Entity.Dispose();
        m_Entity = null;

    } 
}
