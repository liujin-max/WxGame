using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public float m_Time = 0;
    public bool m_IsLoop = false;

    private Action m_Callback;

    public void SetCallback(Action action)
    {
        m_Callback = action;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsLoop) return ;
        
        m_Time -= Time.deltaTime;

        if (m_Time <= 0)
        {
            if (m_Callback != null) {
                m_Callback.Invoke();
            }

            Destroy(gameObject);
        }
    }
}
