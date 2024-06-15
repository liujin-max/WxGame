using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private GridJSON m_Data;
    public int Order {get{return m_Data.Order;}}
    public int X {get{return m_Data.X;}}
    public int Y {get{return m_Data.Y;}}

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

    //对应传送门
    public Grid m_Portal = null;
    public Grid Portal {
        get {
            if (m_Card != null && m_Card.ID == (int)_C.CARD.PORTAL) {
                if (m_Portal == null) {
                    m_Portal = Field.Instance.GetGrid((int)m_Data.Portal.x, (int)m_Data.Portal.y);
                }
                return m_Portal;
            }

            return null;
        }
    }


    private GameObject m_Entity;
    public GameObject Entity {get {return m_Entity;} }
    public Transform Frame;
    private Transform m_Line1;
    private Transform m_Line2;
    private Transform m_Horn1;
    private Transform m_Horn2;


    public Grid(GridJSON gridJSON, Vector2 position)
    {
        m_Data      = gridJSON;
        m_Position  = position;


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

        DrawLines();
    }

    //绘制描边线
    void DrawLines()
    {
        m_Line1.gameObject.SetActive(false);
        m_Line2.gameObject.SetActive(false);
        m_Horn1.gameObject.SetActive(false);
        m_Horn2.gameObject.SetActive(false);

        m_Line1.localScale = Vector3.one;
        m_Line2.localScale = Vector3.one;

        m_Line1.transform.localPosition = Vector3.zero;
        m_Line2.transform.localPosition = Vector3.zero;



        var up      = Field.Instance.GetGridByDirection(this, _C.DIRECTION.UP);
        var down    = Field.Instance.GetGridByDirection(this, _C.DIRECTION.DOWN);
        var left    = Field.Instance.GetGridByDirection(this, _C.DIRECTION.LEFT);
        var right   = Field.Instance.GetGridByDirection(this, _C.DIRECTION.RIGHT);


        //上左、左下、下右、右上
        bool is_line_used = false;
        bool is_horn_used = false;

        //左上
        if (up == null || !up.IsValid)
        {
            if (left != null && left.IsValid && right != null && right.IsValid)
            {
                
                Transform transform = is_line_used ? m_Line2 : m_Line1;

                transform.gameObject.SetActive(true);
                transform.localEulerAngles = new Vector3(0, 0, -90);

                is_line_used = true;

                //斜对角
                if (Field.Instance.GetValidGrid(X - 1, Y + 1) != null)
                {
                    transform.localPosition = new Vector3(0.4f, 0, 0);
                    transform.localScale = new Vector3(1, 0.67f, 1);
                }

                if (Field.Instance.GetValidGrid(X + 1, Y + 1) != null)
                {
                    transform.localPosition = new Vector3(-0.4f, 0, 0);
                    transform.localScale = new Vector3(1, 0.67f, 1);
                }
            }

            if (left == null || !left.IsValid) 
            {
                {
                    is_horn_used = true;
                    m_Horn1.gameObject.SetActive(true);
                    m_Horn1.localEulerAngles = new Vector3(0, 0, 0);
                }

                //如果下面有格子
                if (down != null && down.IsValid)
                {
                    Transform transform = is_line_used ? m_Line2 : m_Line1;

                    transform.gameObject.SetActive(true);
                    transform.localEulerAngles  = new Vector3(0, 0, 0);
                    transform.localScale        = new Vector3(1, 0.3f, 1);
                    transform.localPosition     = new Vector3(0, -0.3f, 0);

                    is_line_used    = true;
                }
            }
        }

        //左下
        if (left == null || !left.IsValid)
        {
            //上下都有格子，则处理线条
            if (up != null && up.IsValid && down != null && down.IsValid)
            {
                Transform transform = is_line_used ? m_Line2 : m_Line1;

                transform.gameObject.SetActive(true);
                transform.localEulerAngles = new Vector3(0, 0, 0);

                is_line_used = true;

                //斜对角
                if (Field.Instance.GetValidGrid(X - 1, Y + 1) != null || Field.Instance.GetValidGrid(X - 1, Y - 1) != null)
                {
                    transform.localScale = new Vector3(1, 0.28f, 1);
                }

            }

            //下面没有格子，则处理拐角
            if (down == null || !down.IsValid) 
            {
                {
                    Transform transform = is_horn_used ? m_Horn2 : m_Horn1;
                    transform.gameObject.SetActive(true);
                    transform.localEulerAngles = new Vector3(0, 0, 90);

                    is_horn_used = true;
                }

                //如果上面有格子
                if (up != null && up.IsValid)
                {
                    Transform transform = is_line_used ? m_Line2 : m_Line1;

                    transform.gameObject.SetActive(true);
                    transform.localEulerAngles  = new Vector3(0, 0, 0);
                    transform.localScale        = new Vector3(1, 0.3f, 1);
                    transform.localPosition     = new Vector3(0, 0.3f, 0);

                    is_line_used    = true;
                }
            }
        }

        //右下
        if (down == null || !down.IsValid)
        {
            if (left != null && left.IsValid && right != null && right.IsValid)
            {

                Transform transform = is_line_used ? m_Line2 : m_Line1;

                transform.gameObject.SetActive(true);
                transform.localEulerAngles = new Vector3(0, 0, 90);

                is_line_used = true;

                //斜对角
                if (Field.Instance.GetValidGrid(X - 1, Y - 1) != null)
                {
                    transform.localPosition = new Vector3(0.4f, 0, 0);
                    transform.localScale = new Vector3(1, 0.67f, 1);
                }

                if (Field.Instance.GetValidGrid(X + 1, Y - 1) != null)
                {
                    transform.localPosition = new Vector3(-0.4f, 0, 0);
                    transform.localScale = new Vector3(1, 0.67f, 1);
                }
            }

            if (right == null || !right.IsValid) 
            {
                {
                    Transform transform = is_horn_used ? m_Horn2 : m_Horn1;
                    transform.gameObject.SetActive(true);
                    transform.localEulerAngles = new Vector3(0, 0, 180);

                    is_horn_used = true;
                }

                //如果上面有格子
                if (up != null && up.IsValid)
                {
                    Transform transform = is_line_used ? m_Line2 : m_Line1;

                    transform.gameObject.SetActive(true);
                    transform.localEulerAngles  = new Vector3(0, 0, 180);
                    transform.localScale        = new Vector3(1, 0.3f, 1);
                    transform.localPosition     = new Vector3(0, 0.3f, 0);

                    is_line_used    = true;
                }
            }
        }

        //右上
        if (right == null || !right.IsValid)
        {
            if (up != null && up.IsValid && down != null && down.IsValid)
            {
                Transform transform = is_line_used ? m_Line2 : m_Line1;

                transform.gameObject.SetActive(true);
                transform.localEulerAngles = new Vector3(0, 0, 180);

                is_line_used = true;

                //斜对角
                if (Field.Instance.GetValidGrid(X + 1, Y + 1) != null || Field.Instance.GetValidGrid(X + 1, Y - 1) != null)
                {
                    transform.localScale = new Vector3(1, 0.28f, 1);
                }
            }

            if (up == null || !up.IsValid) 
            {
                {
                    Transform transform = is_horn_used ? m_Horn2 : m_Horn1;
                    transform.gameObject.SetActive(true);
                    transform.localEulerAngles = new Vector3(0, 0, -90);

                    is_horn_used = true;
                }

                //如果下面有格子
                if (down != null && down.IsValid)
                {
                    Transform transform = is_line_used ? m_Line2 : m_Line1;

                    transform.gameObject.SetActive(true);
                    transform.localEulerAngles  = new Vector3(0, 0, 180);
                    transform.localScale        = new Vector3(1, 0.3f, 1);
                    transform.localPosition     = new Vector3(0, -0.3f, 0);

                    is_line_used    = true;
                }
            }
        }


    }

    public void Show(bool flag)
    {
        if (m_Entity != null) {
            m_Entity.gameObject.SetActive(flag);

            Frame.localScale = Vector3.one;

            DrawLines();
        }
    }

    //当前传送门能否穿越
    //如果穿越后后格子有障碍物，则根据当前card是否是炸弹来判断能否穿越
    public bool IsPortalCanCross(Card card, _C.DIRECTION direction)
    {
        //不是传送门
        if (this.Portal == null) return false;

        int offset_x    = 0;
        int offset_y    = 0;
        if (direction == _C.DIRECTION.LEFT) offset_x = -1;
        else if (direction == _C.DIRECTION.RIGHT) offset_x = 1;
        else if (direction == _C.DIRECTION.UP) offset_y = 1;
        else if (direction == _C.DIRECTION.DOWN) offset_y = -1;

        var near_grid = Field.Instance.GetGrid(Portal.X + offset_x, Portal.Y + offset_y);
        if (near_grid == null) return false;

        if (!near_grid.IsValid) {
            return false;
        }

        if (!near_grid.IsEmpty)
        {
            if (card.IsBomb()) {
                return true;
            }
            return false;
        }

        return true;
    }

    //传送门指向特效
    public void Fly2Portal()
    {
        if (this.Portal == null) return;

        var start_pos   = this.Position;
        var end_pos     = m_Portal.Position;
        var e = GameFacade.Instance.EffectManager.Load(EFFECT.PORTAL_TRAIL, start_pos);
        e.GetComponent<Fly>().GO(start_pos, end_pos);
    }

    public void Dispose()
    {
        if (m_Entity != null) {
            GameObject.Destroy(m_Entity);
            m_Entity = null;
        }
    }
}
