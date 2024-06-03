using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    //-475, -475
    public int X;
    public int Y;

    private bool m_ValidFlag = true;
    public bool IsValid {
        get { return m_ValidFlag;}
        set { m_ValidFlag = value;}
    }

    public bool IsEmpty {get { return m_Card == null;}}

    private Card m_Card;
    public Card Card {
        get {return m_Card;} 
        set {m_Card = value;}
    }

    public Grid(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Vector2 GetPosition()
    {
        return new Vector2(-435 + X * 145, -435 + Y * 145);
    }
}
