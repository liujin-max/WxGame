using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TipWindow : MonoBehaviour
{
    [SerializeField] private GameObject m_TipPivot;


    private Sequence m_TipSequence;

    void Awake()
    {
        EventManager.AddHandler(EVENT.UI_POPUPTIP,  OnTip);
    }

    void OnDestroy()
    {
        EventManager.AddHandler(EVENT.UI_POPUPTIP,  OnTip);
    }

    private void OnTip(GameEvent @event)
    {
        if (m_TipSequence != null) m_TipSequence.Kill();

        var text = (string)@event.GetParam(0);

        m_TipPivot.SetActive(true);

        m_TipSequence = 
    }
}
