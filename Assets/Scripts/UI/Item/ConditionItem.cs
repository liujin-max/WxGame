using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConditionItem : MonoBehaviour
{
    private Condition m_Condition;
    public Condition Condition {get { return m_Condition;}}

    [SerializeField] private Image m_Icon;
    [SerializeField] private TextMeshProUGUI m_Count;

    private bool m_IsShaking = false;


    void Awake()
    {
        EventManager.AddHandler(EVENT.UI_UPDATECONDITION,   OnFlushUI);
        
    }

    void Start()
    {
        m_Icon.GetComponent<Button>().onClick.AddListener(()=>{
            if (m_Condition.ID == (int)_C.CARD.UNIVERSAL) return;
            
            Field.Instance.GetCards(m_Condition.ID, _C.CARD_STATE.NORMAL).ForEach(c => {
                c.Entity.ClickShake();
            });
        });
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.UI_UPDATECONDITION,   OnFlushUI);
    }

    public void Init(Condition condition)
    {
        m_Condition = condition;

        m_Icon.sprite  = Resources.Load<Sprite>("UI/Element/jelly_" + condition.ID);
        m_Icon.SetNativeSize();

        FlushUI();
    }

    void FlushUI()
    {
        if (m_Condition.IsFinished() == true)
        {
            m_Count.text = "<#16AE00>完成";
        }
        else
        {
            m_Count.text = m_Condition.Count.Current.ToString();
        }
    }

    public void Show(bool flag)
    {
        gameObject.SetActive(flag);
    }


    #region 监听事件
    private void OnFlushUI(GameEvent @event)
    {
        Condition cd = @event.GetParam(0) as Condition;

        if (cd == m_Condition) {
            FlushUI();

            if (m_IsShaking) return;

            m_IsShaking = true;
            transform.DOShakeScale(0.3f, 0.2f, 20, 50).OnComplete(()=>{
                m_IsShaking = false;
            });
        } 
    }
    #endregion
}
