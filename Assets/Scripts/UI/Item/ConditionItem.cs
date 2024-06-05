using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConditionItem : MonoBehaviour
{
    private Condition m_Condition;

    [SerializeField] private Image m_Icon;
    [SerializeField] private TextMeshProUGUI m_Count;


    void Awake()
    {
        EventManager.AddHandler(EVENT.UI_BROKENCARD,   OnCardBroken);
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.UI_BROKENCARD,   OnCardBroken);
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
            m_Count.text = "完成";
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
    private void OnCardBroken(GameEvent @event)
    {
        FlushUI();
    }
    #endregion
}
