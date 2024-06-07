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
    [SerializeField] private GameObject m_Eye_Left;
    [SerializeField] private GameObject m_Eye_Right;

    private Tweener m_ScaleTweener;

    private Card m_Card;
    private Vector3 m_TouchPos;
    private bool m_Dragging = false;

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
        if (m_Card.IsFixed) return;

        if (m_ScaleTweener != null) {
            Entity.transform.localScale = Vector3.one;
            m_ScaleTweener.Kill();
        }

        m_ScaleTweener = Entity.transform.DOShakeScale(0.4f, strength , 8, 50);
    }

    public void Shake(_C.DIRECTION direction)
    {
        if (m_Card.IsFixed) return;

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

    void ClickShake()
    {
        if (m_ScaleTweener != null) {
            Entity.transform.localScale = Vector3.one;
            m_ScaleTweener.Kill();
        }

        m_ScaleTweener = Entity.transform.DOShakeScale(0.5f, 0.2f , 8, 50);
    }

    public void DoScale(Vector3 scale, float time, Action callback = null)
    {
        if (m_ScaleTweener != null) {
            Entity.transform.localScale = Vector3.one;
            m_ScaleTweener.Kill();
        }

        m_ScaleTweener = Entity.transform.DOScale(scale, time).OnComplete(()=>{
            if (callback != null) callback();
        });
    }

    void FixedUpdate()
    {
        OnMouseDrag();

        {
            if (m_Card.IsEliminating) return;

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

                    m_Eye_Left.transform.localPosition = ToolUtility.FindPointOnCircle(new Vector2(-0.2f, 1), 0.15f, angle);
                    m_Eye_Right.transform.localPosition = ToolUtility.FindPointOnCircle(new Vector2( 0.2f, 1), 0.15f, angle);

                }
                else 
                {
                    m_Eye_Left.transform.localPosition = new Vector2(-0.2f, 1f);
                    m_Eye_Right.transform.localPosition = new Vector2(0.2f, 1f);
                }
            }
            else
            {
                m_Eye_Left.gameObject.SetActive(false);
                m_Eye_Right.gameObject.SetActive(false);
            }
        }
    }


    public void Dispose()
    {
        Destroy(gameObject);
    }

    #region 监听事件
    void OnMouseDown()
    {   
        if (m_Card.TYPE == _C.CARD_TYPE.FRAME || m_Card.STATE == _C.CARD_STATE.GHOST) return;

        ClickShake();
        if (!m_Card.Dragable) return;   //无法拖动的
        if (Field.Instance.Stage.MoveStep.IsClear()) return;    //没有行动步数了
        if (Field.Instance.GetCurrentFSMState() != _C.FSMSTATE.IDLE) return;
        if (Field.Instance.IsMoved) return;


        m_Dragging  = true;
        m_TouchPos  = Input.mousePosition;
    }

    //自定义的拖拽
    void OnMouseDrag()
    {
        if (!m_Dragging) return;

        Vector3 offset = Input.mousePosition - m_TouchPos;

        //滑动距离太短了
        float distance = 50f;
        if (Mathf.Abs(offset.x) <= distance && Mathf.Abs(offset.y) <= distance) return;

        if (Mathf.Abs(offset.x) > Mathf.Abs(offset.y))  //左右拖动
        {
            if (offset.x < 0) {
                Field.Instance.IsMoved = Field.Instance.MoveLeft(m_Card, true) != null;
            } else {
                Field.Instance.IsMoved = Field.Instance.MoveRight(m_Card, true) != null;
            }
        }
        else    //上下拖动
        {
            if (offset.y < 0) {
                Field.Instance.IsMoved = Field.Instance.MoveDown(m_Card, true) != null;
            } else {
                Field.Instance.IsMoved = Field.Instance.MoveTop(m_Card, true) != null;
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
