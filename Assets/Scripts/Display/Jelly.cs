using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;


//方块实体
public class Jelly : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_Frame;

    private Card m_Card;
    private Vector3 m_TouchPos;
    private bool m_Dragging = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init(Card card)
    {
        m_Card = card;

        if (card.ID == 10001) m_Frame.color = Color.red;
        if (card.ID == 10002) m_Frame.color = Color.yellow;
        if (card.ID == 10003) m_Frame.color = Color.blue;
        if (card.ID == 10004) m_Frame.color = Color.green;

        Flush();
    }

    public void SetPosition(Vector2 vector2)
    {
        transform.localPosition = vector2;
    }

    public void Flush()
    {
        if (m_Card.STATE == _C.CARD_STATE.GHOST) {
            m_Frame.color = new Color(m_Frame.color.r, m_Frame.color.g, m_Frame.color.b, 0.4f);
        } else {
            m_Frame.color = new Color(m_Frame.color.r, m_Frame.color.g, m_Frame.color.b, 1.0f);
        }
    }

    public void DoFade(float alpha, float time, Action action)
    {
        m_Frame.DOFade(alpha, time).OnComplete(()=>{
            action();
        });
    }

    void FixedUpdate()
    {
        OnMouseDrag();
    }

    public void Dispose()
    {
        Destroy(gameObject);
    }

    #region 监听事件
    void OnMouseDown()
    {
        if (!m_Card.Dragable) return;   //无法拖动的
        if (Field.Instance.Stage.MoveStep.IsClear()) return;    //没有行动步数了

        m_Dragging  = true;
        m_TouchPos  = Input.mousePosition;
    }

    //自定义的拖拽
    void OnMouseDrag()
    {
        if (!m_Dragging) return;

        Vector3 offset = Input.mousePosition - m_TouchPos;

        //滑动距离太短了
        float distance = 0.4f;
        if (Mathf.Abs(offset.x) <= distance && Mathf.Abs(offset.y) <= distance) return;

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

    void OnMouseUp()
    {
        m_Dragging = false;
    }

    #endregion
}
