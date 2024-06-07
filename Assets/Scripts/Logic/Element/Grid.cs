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
    public Transform Frame;
    private Transform m_Line1;
    private Transform m_Line2;
    private Transform m_Horn1;
    private Transform m_Horn2;

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

        var res = Order % 2 == 0 ? "UI/Element/grid_1" :  "UI/Element/grid_2";
        Frame   = m_Entity.transform.Find("Frame");
        Frame.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(res);


        m_Line1 = m_Entity.transform.Find("Line1Pivot");
        m_Line2 = m_Entity.transform.Find("Line2Pivot");
        m_Horn1 = m_Entity.transform.Find("Horn1Pivot");
        m_Horn2 = m_Entity.transform.Find("Horn2Pivot");

        FlushFrame();
    }

    void FlushFrame()
    {
        m_Line1.gameObject.SetActive(false);
        m_Line2.gameObject.SetActive(false);
        m_Horn1.gameObject.SetActive(false);
        m_Horn2.gameObject.SetActive(false);

        m_Line1.localScale = Vector3.one;
        m_Line2.localScale = Vector3.one;



        var up      = Field.Instance.GetGridByDirection(this, _C.DIRECTION.UP);
        var down    = Field.Instance.GetGridByDirection(this, _C.DIRECTION.DOWN);
        var left    = Field.Instance.GetGridByDirection(this, _C.DIRECTION.LEFT);
        var right   = Field.Instance.GetGridByDirection(this, _C.DIRECTION.RIGHT);


        //上左、左下、下右、右上
        bool is_line_used = false;
        bool is_horn_used = false;

        if (up == null || !up.IsValid)
        {
            if (left != null && left.IsValid && right != null && right.IsValid)
            {
                is_line_used = true;

                m_Line1.gameObject.SetActive(true);
                m_Line1.localEulerAngles = new Vector3(0, 0, -90);
            }

            if (left == null || !left.IsValid) 
            {
                is_horn_used = true;
                m_Horn1.gameObject.SetActive(true);
                m_Horn1.localEulerAngles = new Vector3(0, 0, 0);
            }
        }

        if (left == null || !left.IsValid)
        {
            if (up != null && up.IsValid && down != null && down.IsValid)
            {
                Transform transform = is_line_used ? m_Line2 : m_Line1;

                transform.gameObject.SetActive(true);
                transform.localEulerAngles = new Vector3(0, 0, 0);

                is_line_used = true;
            }

            if (down == null || !down.IsValid) 
            {
                Transform transform = is_horn_used ? m_Horn2 : m_Horn1;
                transform.gameObject.SetActive(true);
                transform.localEulerAngles = new Vector3(0, 0, 90);

                is_horn_used = true;
            }
        }

        if (down == null || !down.IsValid)
        {
            if (left != null && left.IsValid && right != null && right.IsValid)
            {

                Transform transform = is_line_used ? m_Line2 : m_Line1;

                transform.gameObject.SetActive(true);
                transform.localEulerAngles = new Vector3(0, 0, 90);

                is_line_used = true;
            }

            if (right == null || !right.IsValid) 
            {
                Transform transform = is_horn_used ? m_Horn2 : m_Horn1;
                transform.gameObject.SetActive(true);
                transform.localEulerAngles = new Vector3(0, 0, 180);

                is_horn_used = true;
            }
        }

        if (right == null || !right.IsValid)
        {
            if (up != null && up.IsValid && down != null && down.IsValid)
            {
                Transform transform = is_line_used ? m_Line2 : m_Line1;

                transform.gameObject.SetActive(true);
                transform.localEulerAngles = new Vector3(0, 0, 180);

                is_line_used = true;
            }

            if (up == null || !up.IsValid) 
            {
                Transform transform = is_horn_used ? m_Horn2 : m_Horn1;
                transform.gameObject.SetActive(true);
                transform.localEulerAngles = new Vector3(0, 0, -90);

                is_horn_used = true;
            }
        }


        //
        
    }

    public void Show(bool flag)
    {
        if (m_Entity != null) {
            m_Entity.gameObject.SetActive(flag);

            Frame.localScale = Vector3.one;

            FlushFrame();
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
