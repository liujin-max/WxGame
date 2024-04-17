using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BallSeatItem : MonoBehaviour
{
    [SerializeField] private Image c_Icon;

    public void Init(int type = -1)
    {
        if (type == -1) {
            c_Icon.gameObject.SetActive(false);
            return;
        } 

        c_Icon.gameObject.SetActive(true);

        var config = CONFIG.GetBallData((_C.BALLTYPE)type);
        c_Icon.sprite = Resources.Load<Sprite>(config.Icon);
        c_Icon.SetNativeSize();
        
    }

    public void DoScale()
    {
        GameFacade.Instance.EffectManager.Load(EFFECT.COMPLEXT, Vector3.zero, gameObject);

        c_Icon.transform.localScale = Vector3.zero;

        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(0.35f);
        sequence.AppendCallback(()=>{
            GameFacade.Instance.EffectManager.Load(EFFECT.COMPLEX, new Vector3(0, 50f, 0), gameObject);
        });
        sequence.Append(c_Icon.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.15f));
        sequence.Append(c_Icon.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.1f));
        sequence.Play();
    }
}
