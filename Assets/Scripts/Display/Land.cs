using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


//负责对每个关卡做特殊处理
public class Land
{
    public Transform ENTITY;
    public Transform GRID_ROOT;
    public Transform ENTITY_ROOT;


    private Tweener m_Tweener = null;


    public Land()
    {
        ENTITY      = GameObject.Find("Field").transform;
        GRID_ROOT   = GameObject.Find("Field/Grids").transform;
        ENTITY_ROOT = GameObject.Find("Field/Entitys").transform;
    }

    public void DoShake()
    {
        Platform.Instance.VIBRATE(_C.VIBRATELEVEL.HEAVY);

        if (m_Tweener != null) {
            ENTITY.transform.localPosition = Vector3.zero;
            m_Tweener.Kill();
        }

        m_Tweener = ENTITY.DOShakePosition(0.5f, 0.35f, 12, 60);
    }

    public void DoSmallShake()
    {
        if (m_Tweener != null) {
            ENTITY.transform.localPosition = Vector3.zero;
            m_Tweener.Kill();
        }

        m_Tweener = ENTITY.DOShakePosition(0.1f, 0.15f, 15, 60);
    }
}
