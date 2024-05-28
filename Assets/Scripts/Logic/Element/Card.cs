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

    public bool IsEliminate = false;

    public Card(CardData cardData)
    {
        m_Data = cardData;
    }
}
