using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SceneSwitch : MonoBehaviour
{
    [SerializeField] private Transform m_Pivot;
    [SerializeField] private Transform m_MaskTop;
    [SerializeField] private Transform m_MaskBottom;


    private Action m_Callback;
    private float m_Delay;


    public void Enter(Action callback, float delay = 0)
    {
        m_Callback  = callback;
        m_Delay     = delay;

        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPMASK, true));

        m_Pivot.localEulerAngles    = new Vector3(0, 0, 20f);
        m_MaskTop.localPosition     = new Vector3(0, 25, 0);
        m_MaskBottom.localPosition  = new Vector3(0,-25, 0);

        yield return new WaitForSeconds(m_Delay);

        m_MaskTop.DOLocalMove(new Vector3(0, 12f, 0), 0.3f).SetEase(Ease.OutBack);
        m_MaskBottom.DOLocalMove(new Vector3(0, -12f, 0), 0.3f).SetEase(Ease.OutBack);

        
        yield return new WaitForSeconds(0.3f);

        if (m_Callback != null) m_Callback();

        yield return new WaitForSeconds(0.15f);

        m_MaskTop.DOLocalMove(new Vector3(0, 25, 0), 0.2f).SetEase(Ease.InQuad);
        m_MaskBottom.DOLocalMove(new Vector3(0, -25, 0), 0.2f).SetEase(Ease.InQuad);

        yield return new WaitForSeconds(0.2f);
        EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPMASK, false));

        yield return null;
    }
}
