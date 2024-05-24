using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LongPressButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    private CDTimer m_PressTimer = new CDTimer(0.6f); // 设置长按时间为1秒
    private CDTimer m_PressStep = new CDTimer(0.3f);

    private bool m_PressFlag = false;
    private Action m_Callback;


    public void Init(Action callback)
    {
        m_Callback = callback;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        m_PressFlag = true;
        m_PressTimer.Reset();
        m_PressStep.Reset();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_PressFlag = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (m_PressTimer.IsFinished() == true) return;  //意味着长按过了，则不响应click

        m_Callback();
    }

    void Update()
    {
        if (!m_PressFlag) return;

        m_PressTimer.Update(Time.deltaTime);
        if (m_PressTimer.IsFinished() == true)
        {
            m_PressStep.Update(Time.deltaTime);
            if (m_PressStep.IsFinished() == true)
            {
                m_PressStep.Reset();

                m_Callback();
            }
        }
    }

}
