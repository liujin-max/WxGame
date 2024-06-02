using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    private CardData m_Data;

    public int ID {get {return m_Data.ID;}}

    private Grid m_Grid;
    public Grid Grid {
        get {return m_Grid;} 
        set {m_Grid = value;}
    }

    //实体
    private CardView m_Entity;
    public CardView Entity {
        get {return m_Entity;}
        set {m_Entity = value;}
    }


    public Card(CardData cardData)
    {
        m_Data = cardData;


        
        EventManager.AddHandler(EVENT.ONCARDBROKEN,   OnCardBroken);
    }

    public void Dispose()
    {
        EventManager.DelHandler(EVENT.ONCARDBROKEN,   OnCardBroken);
    }


    #region 监听事件
    private void OnCardBroken(GameEvent @event)
    {
        var card = @event.GetParam(0) as Card;
        if (card == this) {
            this.m_Entity.Broken();
        }
    }
    #endregion
    
}
