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
        if (card.ID == 10001) m_Icon.color = Color.red;
        if (card.ID == 10002) m_Icon.color = Color.yellow;
        if (card.ID == 10003) m_Icon.color = Color.blue;
        if (card.ID == 10004) m_Icon.color = Color.green;


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
        if (m_Card.STATE == _C.CARD_STATE.NORMAL)
        {
            m_Eye_Left.gameObject.SetActive(true);
            m_Eye_Right.gameObject.SetActive(true);

            var card = Field.Instance.GetMinDistanceSameCard(m_Card);
            if (card != null)
            {
                Vector2 t_pos = card.Entity.transform.localPosition;
                Vector2 o_pos = m_Card.Entity.transform.localPosition;

                float angle = Vector2.Angle(t_pos - o_pos, Vector2.right);
                if (t_pos.y < o_pos.y) {
                    angle *= -1;
                }

                // Debug.Log("方块：" + m_Card.Grid.X + ", " + m_Card.Grid.Y + " 看向 " + card.Grid.X + ", " + card.Grid.Y + " 角度：" + angle);
                m_Eye_Left.transform.localPosition = ToolUtility.FindPointOnCircle(new Vector2(-25, 0), 15, angle);
                m_Eye_Right.transform.localPosition = ToolUtility.FindPointOnCircle(new Vector2( 25, 0), 15, angle);

            }
            else 
            {
                m_Eye_Left.transform.localPosition = new Vector2(-25, 0);
                m_Eye_Right.transform.localPosition = new Vector2(25, 0);
            }
        }
        else
        {
            m_Eye_Left.gameObject.SetActive(false);
            m_Eye_Right.gameObject.SetActive(false);
        }

    }


    #region 监听事件
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!m_Card.Dragable) return;   //无法拖动的
        if (Field.Instance.Stage.MoveStep.IsClear()) return;    //没有行动步数了

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
            transform.DOLocalMove(m_Card.Grid.Position, 0.3f).OnComplete(()=>{
                EventManager.SendEvent(new GameEvent(EVENT.ONCARDMOVED, this));
            });
        }
    }
    #endregion
}
