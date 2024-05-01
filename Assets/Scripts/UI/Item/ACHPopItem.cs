using System.Collections;
using System.Collections.Generic;
using CB;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ACHPopItem : MonoBehaviour
{
    [SerializeField] private ShakeText m_Text;
    [SerializeField] private CanvasGroup m_CanvasGroup;

    Sequence m_Sequence = null;

    void Awake()
    {
        m_CanvasGroup = transform.GetComponent<CanvasGroup>();
    }

    public void Init(Achievement achievement)
    {
        m_Text.SetText(achievement.GetDescription());

        DoAnim();
    }

    void DoAnim()
    {
        GameFacade.Instance.SoundManager.Load(SOUND.ACHIEVEMENT);
        
        transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        m_CanvasGroup.DOFade(1, 0f);

        if (m_Sequence != null) m_Sequence.Kill();

        m_Sequence = DOTween.Sequence();
        m_Sequence.Append(transform.DOScale(1f, 0.1f));
        m_Sequence.Append(transform.DOShakePosition(0.15f, 20f, 15, 50));
        m_Sequence.AppendInterval(1.5f);
        m_Sequence.Append(m_CanvasGroup.DOFade(0, 0.3f));
        m_Sequence.AppendCallback(()=>{
            this.Show(false);
        });
        m_Sequence.Play();
    }

    public void Show(bool flag)
    {
        gameObject.SetActive(flag);
    }

    public bool IsShow()
    {
        return gameObject.activeSelf;
    }
}
