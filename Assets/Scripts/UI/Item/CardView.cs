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

        m_Icon.sprite = Resources.Load<Sprite>("UI/Card/" + card.ID);
        m_Icon.SetNativeSize();
    }

    public void Broken()
    {
        transform.DOScale(2, 0.1f);
        m_CanvasGroup.DOFade(0.1f, 0.1f).OnComplete(()=>{
            m_Card.Entity = null;
            Destroy(gameObject);
        });
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
