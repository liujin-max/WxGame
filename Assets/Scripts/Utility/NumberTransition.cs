using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NumberTransition : MonoBehaviour
{
    private int m_TargetNumber = 0;
    private float m_CurrentNumber = 0;
    private float m_Offset = 0;



    private TextMeshProUGUI m_Text;

    void Awake()
    {
        m_Text = transform.GetComponent<TextMeshProUGUI>();
    }

    public void SetValue(int value)
    {
        m_TargetNumber = value;
        m_Offset    = m_TargetNumber - m_CurrentNumber;

        m_Text.text = ((int)m_CurrentNumber).ToString();
    }

    public void ForceValue(int value)
    {
        m_TargetNumber = value;
        m_CurrentNumber = value;
        m_Text.text = ((int)m_CurrentNumber).ToString();
    }

    void Update()
    {
        if (m_TargetNumber == (int)m_CurrentNumber) return;

        
        var speed   = m_Offset * Time.deltaTime;

        if (speed > 0) {
            speed   = Math.Max(0.3f, speed);
        } else {
            speed   = Math.Min(-0.3f, speed);
        }

        m_CurrentNumber += speed;

        var offset  = m_TargetNumber - (int)m_CurrentNumber;
        if (offset > 0) {
            if (m_CurrentNumber >= m_TargetNumber) {
                m_CurrentNumber = m_TargetNumber;
            }
        } else {
            if (m_CurrentNumber <= m_TargetNumber) {
                m_CurrentNumber = m_TargetNumber;
            }
        }

        m_Text.text = ((int)m_CurrentNumber).ToString();
    }
}
