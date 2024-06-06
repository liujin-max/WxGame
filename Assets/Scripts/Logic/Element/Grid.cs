using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    public int Order;
    public int X;
    public int Y;

    private Vector2 m_Position;
    public Vector2 Position {get {return m_Position;}}

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

    private GameObject m_Entity;
    public GameObject Entity {get {return m_Entity;} }

    public Grid(int order, int x, int y, Vector2 position)
    {
        Order   = order;
        X       = x;
        Y       = y;

        m_Position = position;
    }


    public void Display()
    {
        m_Entity = GameFacade.Instance.UIManager.LoadPrefab("Prefab/Element/Grid", Vector3.zero, Field.Instance.Land.GRID_ROOT);
        m_Entity.transform.localPosition = m_Position;
        m_Entity.transform.localEulerAngles = Vector3.zero;

        m_Entity.transform.Find("1").gameObject.SetActive(Order % 2 != 0);
        m_Entity.transform.Find("2").gameObject.SetActive(Order % 2 == 0);
    }

    public void Show(bool flag)
    {
        if (m_Entity != null) {
            m_Entity.gameObject.SetActive(flag);
        }
    }


    public void Dispose()
    {
        if (m_Entity != null) {
            GameObject.Destroy(m_Entity);
            m_Entity = null;
        }
    }
}
