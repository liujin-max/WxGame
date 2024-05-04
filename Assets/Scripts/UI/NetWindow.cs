using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetWindow : MonoBehaviour
{
    [SerializeField] private GameObject Mask;

    private int m_Refrence = 0;
    void Awake()
    {
        Mask.SetActive(false);

        EventManager.AddHandler(EVENT.UI_NETUPDATE, OnNetUpdate);
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.UI_NETUPDATE, OnNetUpdate);
    }

    void OnNetUpdate(GameEvent @event)
    {
        bool flag = (bool)@event.GetParam(0);

        m_Refrence += flag ? 1 : -1;

        Mask.SetActive(m_Refrence > 0);
    }
}
