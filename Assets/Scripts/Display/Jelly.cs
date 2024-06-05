using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;


//方块实体
public class Jelly : MonoBehaviour
{
    public SpriteRenderer Entity;

    private Tweener m_ScaleTweener;

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

        Entity.sprite  = Resources.Load<Sprite>("UI/Element/jelly_" + card.ID);

        Flush();
    }

    public void SetPosition(Vector2 vector2)
    {
        transform.localPosition = vector2;
    }

    public void Flush()
    {
        if (m_Card.STATE == _C.CARD_STATE.GHOST) {
            Entity.color = new Color(Entity.color.r, Entity.color.g, Entity.color.b, 0.4f);
        } else {
            Entity.color = new Color(Entity.color.r, Entity.color.g, Entity.color.b, 1.0f);
        }
    }

    public void DoFade(float alpha, float time, Action action)
    {
        Entity.DOFade(alpha, time).OnComplete(()=>{
            action();
        });
    }

    public void Shake(Vector2 strength)
    {
        if (m_ScaleTweener != null) m_ScaleTweener.Kill();

        m_ScaleTweener = Entity.transform.DOShakeScale(0.5f, strength , 8, 50);
    }

    public void Shake(_C.DIRECTION direction)
    {
        switch (direction)
        {
            case _C.DIRECTION.TOP:
                this.Shake(new Vector2(0, 0.2f));
                break;

            case _C.DIRECTION.DOWN:
                this.Shake(new Vector2(0, 0.2f));
                break;

            case _C.DIRECTION.LEFT:
                this.Shake(new Vector2(0.2f, 0));
                break;

            case _C.DIRECTION.RIGHT:
                this.Shake(new Vector2(0.2f, 0));
                break;
        }
    }

    public void Shake()
    {
        if (m_ScaleTweener != null) m_ScaleTweener.Kill();

        m_ScaleTweener = Entity.transform.DOShakeScale(0.5f, 0.2f , 8, 50);
    }

    public void DoScale(Vector3 scale, float time, Action callback = null)
    {
        if (m_ScaleTweener != null) m_ScaleTweener.Kill();

        m_ScaleTweener = Entity.transform.DOScale(scale, time).OnComplete(()=>{
            if (callback != null) callback();
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
        if (Field.Instance.IsMoved) return;
        
        Field.Instance.IsMoved = true;

        m_Dragging  = true;
        m_TouchPos  = Input.mousePosition;

        Shake();
    }

    //自定义的拖拽
    void OnMouseDrag()
    {
        if (!m_Dragging) return;

        Vector3 offset = Input.mousePosition - m_TouchPos;

        //滑动距离太短了
        float distance = 40f;
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
