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
    private bool m_Dragged = false;

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
            Entity.color = new Color(Entity.color.r, Entity.color.g, Entity.color.b, 0.5f);
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
            case _C.DIRECTION.UP:
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

    //不要传入callback了，被Kill的时候callback会不执行了
    //如果非要callback，记录下callback，在kill的同时执行callback, 在callback内部销毁记录着的callback自身
    public Tweener DoScale(Vector3 scale, float time)
    {
        if (m_ScaleTweener != null) {
            Entity.transform.localScale = Vector3.one;
            m_ScaleTweener.Kill();
        }

        m_ScaleTweener = Entity.transform.DOScale(scale, time);

        return m_ScaleTweener;
    }

    void DrawEyes()
    {
        if (m_Card.IsEliminating) return;

        if (m_Card.STATE == _C.CARD_STATE.NORMAL && m_Card.TYPE == _C.CARD_TYPE.JELLY)
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

                m_Eye_Left.transform.localPosition = ToolUtility.FindPointOnCircle(new Vector2(-0.2f, 0.9f), 0.15f, angle);
                m_Eye_Right.transform.localPosition = ToolUtility.FindPointOnCircle(new Vector2( 0.2f, 0.9f), 0.15f, angle);

            }
            else 
            {
                m_Eye_Left.transform.localPosition = new Vector2(-0.2f, 0.9f);
                m_Eye_Right.transform.localPosition = new Vector2(0.2f, 0.9f);
            }
        }
        else
        {
            m_Eye_Left.gameObject.SetActive(false);
            m_Eye_Right.gameObject.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        OnMouseDrag();

        //眼睛动画
        DrawEyes();
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
                Field.Instance.IsMoved = Field.Instance.Move(m_Card, _C.DIRECTION.LEFT, true) != null;
            } else {
                Field.Instance.IsMoved = Field.Instance.Move(m_Card, _C.DIRECTION.RIGHT, true) != null;
            }
        }
        else    //上下拖动
        {
            if (offset.y < 0) {
                Field.Instance.IsMoved = Field.Instance.Move(m_Card, _C.DIRECTION.DOWN, true) != null;
            } else {
                Field.Instance.IsMoved = Field.Instance.Move(m_Card, _C.DIRECTION.UP, true) != null;
            }
        }


        m_Dragging  = false;
        m_Dragged   = true;
    }

    void OnMouseUp()
    {
        if (!m_Dragged) {
            Field.Instance.GetCards(m_Card.ID, _C.CARD_STATE.NORMAL).ForEach(c => {
                c.Entity.ClickShake();
            });
        }

        m_Dragging  = false;
        m_Dragged   = false;
    }

    #endregion
}
