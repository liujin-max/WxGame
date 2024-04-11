using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RateNumber : MonoBehaviour
{

    [SerializeField] private Text m_Text;

    void Awake()
    {
        transform.DOShakePosition(0.3f, new Vector3(20, 0, 0), 50, 50);
        transform.DOLocalMoveY(transform.transform.localPosition.y + 50, 0.6f);
    }

    public void Fly(float value)
    {
        m_Text.text = string.Format("x{0}", value);
    }
}
