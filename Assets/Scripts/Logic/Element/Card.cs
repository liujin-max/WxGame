using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    private CardData m_Data;

    public int ID {get {return m_Data.ID;}}

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

    //可滑动
    public bool Dragable {
        get {
            if (STATE == _C.CARD_STATE.GHOST) {
                return false;
            }

            if (IsEliminating) return false;

            return true;
        }
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
