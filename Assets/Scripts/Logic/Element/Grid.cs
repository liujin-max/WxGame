using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    //-475, -475
    public int X;
    public int Y;

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
        return new Vector2(-475 + X * 105, -475 + Y * 105);
    }

    public bool IsEmpty()
    {
        return m_Card == null;
    }
}
