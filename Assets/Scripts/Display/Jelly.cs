using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


//方块实体
public class Jelly : MonoBehaviour
{
    public SpriteRenderer Entity;
    [SerializeField] private SpriteRenderer m_Emoji;

    private Tweener m_ShakeTweener;
    private Tweener m_ScaleTweener;

    private Card m_Card;
    private Vector3 m_EntityPos;
    private Vector3 m_TouchPos;
    private bool m_Dragging = false;
    private bool m_Dragged = false;

    private CDTimer m_EmojiTimer = new CDTimer(0.1f);
    private bool m_IsMoving = false;

    void Awake()
    {
        m_EntityPos = Entity.transform.localPosition;
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
        if (!m_Card.Dragable) return;

        if (m_ShakeTweener != null) {
            Entity.transform.localPosition = m_EntityPos;
            m_ShakeTweener.Kill();
        }

        m_ShakeTweener = Entity.transform.DOShakePosition(0.3f, strength , 15, 50);
    }

    public void Shake(_C.DIRECTION direction)
    {
        if (!m_Card.Dragable) return;

        switch (direction)
        {
            case _C.DIRECTION.UP:
                this.Shake(new Vector2(0, 0.05f));
                break;

            case _C.DIRECTION.DOWN:
                this.Shake(new Vector2(0, 0.05f));
                break;

            case _C.DIRECTION.LEFT:
                this.Shake(new Vector2(0.05f, 0));
                break;

            case _C.DIRECTION.RIGHT:
                this.Shake(new Vector2(0.05f, 0));
                break;
        }
    }

    public void ClickShake()
    {
        if (m_ShakeTweener != null) {
            Entity.transform.localPosition = m_EntityPos;
            m_ShakeTweener.Kill();
        }

        GameFacade.Instance.SoundManager.Load(SOUND.HIT);

        m_ShakeTweener = Entity.transform.DOShakePosition(0.3f, 0.05f , 20, 50);
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

    public void DoPunchScale()
    {
        if (m_ScaleTweener != null) {
            Entity.transform.localScale = Vector3.one;
            m_ScaleTweener.Kill();
        }

        Entity.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.15f);
    }

    void DrawEmoji()
    {
        if (m_Card.STATE == _C.CARD_STATE.GHOST || m_Card.TYPE == _C.CARD_TYPE.FRAME) {
            m_Emoji.gameObject.SetActive(false);
            return;
        }

        if (m_Card.IsBomb() == true) {
            m_Emoji.gameObject.SetActive(false);
            return;
        }

        m_Emoji.gameObject.SetActive(true);

        if (m_Card.IsEliminating) {
            m_Emoji.sprite = Resources.Load<Sprite>("UI/Emoji/5");
            return;
        }
        
        if (m_IsMoving == true) {
            m_Emoji.sprite = Resources.Load<Sprite>("UI/Emoji/6");
            return;
        }

        

        m_EmojiTimer.Update(Time.deltaTime);
        if (m_EmojiTimer.IsFinished() == true) {
            m_EmojiTimer.Reset(RandomUtility.Random(200, 800) / 100.0f);

            int id = RandomUtility.Random(1, 5);
            m_Emoji.sprite = Resources.Load<Sprite>("UI/Emoji/" + id);
        }
    }

    public void DoMove(bool flag, _C.DIRECTION direction)
    {
        m_IsMoving = flag;
        m_EmojiTimer.Full();

        if (flag == true)
        {
            if (direction == _C.DIRECTION.LEFT || direction == _C.DIRECTION.RIGHT) {
                this.DoScale(new Vector3(1.2f, 0.8f, 0), 0.1f);
            }
            else {
                this.DoScale(new Vector3(0.8f, 1.2f, 0), 0.1f);
            }
        }
        else
        {
            this.DoPunchScale();
        }
    }

    void FixedUpdate()
    {
        OnMouseDrag();

        //表情动画
        DrawEmoji();
    }


    public void Dispose()
    {
        Destroy(gameObject);
    }

    #region 监听事件

    void OnMouseDown()
    {   
        if (!m_Card.Dragable) return;   //无法拖动的
        if (Field.Instance.Stage.NeedCheckStep() && Field.Instance.Stage.IsStepClear()) return;    //没有行动步数了
        if (Field.Instance.Stage.NeedCheckTimer() && Field.Instance.Stage.IsTimerClear()) return;    //没有时间了
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
            if (m_Card.TYPE == _C.CARD_TYPE.JELLY && m_Card.STATE == _C.CARD_STATE.NORMAL) {
                Field.Instance.GetCards(m_Card.ID, _C.CARD_STATE.NORMAL).ForEach(c => {
                    c.Entity.ClickShake();
                });
            }

            if (m_Card.TYPE == _C.CARD_TYPE.SPECIAL) {
                this.ClickShake();
            }

            if (m_Card.ID == (int)_C.CARD.PORTAL) {
                if (m_Card.Grid.Portal != null) {
                    m_Card.Grid.Fly2Portal();
                    this.ClickShake();
                    m_Card.Grid.Portal.Card.Entity.ClickShake();
                }
                    
            }
        }

        m_Dragging  = false;
        m_Dragged   = false;
    }

    #endregion
}
