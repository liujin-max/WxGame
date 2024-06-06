using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    private CardData m_Data;

    public int ID {get {return m_Data.ID;}}
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

    //死亡分解中
    public bool IsEliminating = false;

    //固定的
    private bool m_IsFixed = false;
    public bool IsFixed {
        get {
            if (TYPE == _C.CARD_TYPE.FRAME) return true;

            return m_IsFixed;
        } 
        set {m_IsFixed = value;}
    }

    //可滑动
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

    //状态
    public _C.CARD_STATE STATE;

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
