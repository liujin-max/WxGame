using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardView : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Card m_Card;
    public Card Card { get { return m_Card;}}

    [SerializeField] private Image m_Icon;
    [SerializeField] private Image m_Eye_Left;
    [SerializeField] private Image m_Eye_Right;

    private Vector3 m_Eye_Left_Pos;
    private Vector3 m_Eye_Right_Pos;
    private CanvasGroup m_CanvasGroup;


    private Vector2 m_TouchPos;
    private bool m_Dragging = false;

    void Awake()
    {
        m_CanvasGroup = GetComponent<CanvasGroup>();

        transform.localScale = Vector2.zero;
        transform.DOScale(1, 0.15f);

        EventManager.AddHandler(EVENT.UI_MOVECARD,   OnReponseMoveCard);
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.UI_MOVECARD,   OnReponseMoveCard);
    }

    public void Init(Card card)
    {
        m_Card = card;
        m_Card.Entity = this;

        // m_Icon.sprite = Resources.Load<Sprite>("UI/Card/" + card.ID);
        // m_Icon.SetNativeSize();
        if (card.ID == 100) m_Icon.color = Color.red;
        if (card.ID == 101) m_Icon.color = Color.yellow;
        if (card.ID == 102) m_Icon.color = Color.blue;

        m_Eye_Left_Pos = m_Eye_Left.transform.position;
        m_Eye_Right_Pos = m_Eye_Right.transform.position;

        FlushUI();
    }

    public void FlushUI()
    {
        m_CanvasGroup.alpha = m_Card.STATE == _C.CARD_STATE.NORMAL ? 1 : 0.4f;
    }

    public void Broken()
    {
        transform.DOScale(1.6f, 0.1f);
        m_CanvasGroup.DOFade(0.1f, 0.1f).OnComplete(()=>{
            m_Card.Entity = null;
            this.Destroy();
        });
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        var card = Field.Instance.GetMinDistanceSameCard(m_Card);
        if (card != null)
        {
            // float angle = Vector2.Angle(card.Grid.GetPosition() - m_Card.Grid.GetPosition(), Vector2.right);
            // Debug.Log("方块：" + m_Card.Grid.X + ", " + m_Card.Grid.Y + " 看向 " + card.Grid.X + ", " + card.Grid.Y + " 角度：" + angle);
            // Vector2 pos = ToolUtility.FindPointOnCircle(new Vector2(-30, 0), 10, angle);
            // m_Eye_Left.transform.localPosition = pos;

            // m_Eye_Right.transform.localPosition = ToolUtility.FindPointOnCircle(new Vector2( 30, 0), 10, angle);

            // m_Eye_Left.transform.LookAt(card.Entity.transform);
            // m_Eye_Right.transform.LookAt(card.Entity.transform);
                    // 使眼睛朝向鼠标
            
            

            // Vector3 mousePos = Input.mousePosition;
            // mousePos = Camera.main.ScreenToWorldPoint(mousePos);

            // Vector2 directionToMouse = (mousePos - m_Eye_Left_Pos).normalized;
            // m_Eye_Left.transform.position = (Vector2)m_Eye_Left_Pos + directionToMouse * 10;


            Vector2 l_direction = (card.Entity.transform.position - transform.position).normalized;
            m_Eye_Left.transform.position = (Vector2)m_Eye_Left_Pos + l_direction * 0.1f;

            Vector2 r_direction = (card.Entity.transform.position - transform.position).normalized;
            m_Eye_Right.transform.position = (Vector2)m_Eye_Right_Pos + r_direction * 0.1f;

            Debug.Log("坐标：" + m_Eye_Left_Pos + "| " + m_Eye_Right_Pos);
        }
        else 
        {
            m_Eye_Left.transform.localPosition = new Vector2(-30, 0);
            m_Eye_Right.transform.localPosition = new Vector2(30, 0);
        }

    }


    #region 监听事件
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!m_Card.Dragable) return;   //无法拖动的

        m_Dragging  = true;
        m_TouchPos  = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!m_Dragging) return;

        Vector2 offset = eventData.position - m_TouchPos;

        //滑动距离太短了
        if (Mathf.Abs(offset.x) <= 40 && Mathf.Abs(offset.y) <= 40) return;


        if (Mathf.Abs(offset.x) > Mathf.Abs(offset.y))  //左右拖动
        {
            if (offset.x < 0) {
                Field.Instance.MoveLeft(m_Card);
            } else {
                Field.Instance.MoveRight(m_Card);
            }
        }
        else    //上下拖动
        {
            if (offset.y < 0) {
                Field.Instance.MoveDown(m_Card);
            } else {
                Field.Instance.MoveTop(m_Card);
            }
        }


        m_Dragging = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_Dragging = false;
    }

    private void OnReponseMoveCard(GameEvent @event)
    {
        Card card = @event.GetParam(0) as Card;

        if (card == m_Card) {
            transform.DOLocalMove(m_Card.Grid.GetPosition(), 0.25f).OnComplete(()=>{
                EventManager.SendEvent(new GameEvent(EVENT.ONCARDMOVED, this));
            });
        }
    }
    #endregion
}
