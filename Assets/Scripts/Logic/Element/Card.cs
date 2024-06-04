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

    //可滑动
    public bool Dragable {
        get {
            if (STATE == _C.CARD_STATE.GHOST) {
                return false;
            }
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
        m_Entity = new Jelly(this);
        m_Entity.Display();
    }

    public void Dispose()
    {
        m_Entity.Destroy();
        m_Entity = null;

    }

    


    #region 监听事件

    #endregion
    
}
