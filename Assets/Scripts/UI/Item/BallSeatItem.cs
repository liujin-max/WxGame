using System.Collections;
using System.Collections.Generic;
using CB;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BallSeatItem : MonoBehaviour
{
    [SerializeField] private Image c_Icon;

    public Ball Ball;

    public void Init(Ball ball)
    {
        Ball = ball;
        
        if (ball == null) {
            c_Icon.gameObject.SetActive(false);
            return;
        } 

        c_Icon.gameObject.SetActive(true);

        c_Icon.sprite = Resources.Load<Sprite>(ball.Config.Icon);
        c_Icon.SetNativeSize();
        
    }

    public void DoScale()
    {
        GameFacade.Instance.EffectManager.Load(EFFECT.COMPLEXT, Vector3.zero, gameObject);

        c_Icon.transform.localScale = Vector3.zero;

        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(0.35f);
        sequence.AppendCallback(()=>{
            GameFacade.Instance.EffectManager.LoadUIEffect(EFFECT.COMPLEX, transform.position + new Vector3(0, 0.5f, 0));
        });
        sequence.Append(c_Icon.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.15f));
        sequence.Append(c_Icon.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.1f));
        sequence.Play();
    }

    public void ShowFadeScale()
    {
        GameFacade.Instance.SoundManager.Load(SOUND.TRIGGER);
        
        GameObject obj = new GameObject("Image");
        obj.transform.SetParent(this.transform);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero;

        Image imageComponent = obj.AddComponent<Image>();
        imageComponent.sprite = c_Icon.sprite;
        imageComponent.SetNativeSize();
        
        Sequence seq = DOTween.Sequence();
        seq.Join(imageComponent.transform.DOScale(2f, 0.4f));
        seq.Join(imageComponent.DOFade(0f, 0.4f).OnComplete(()=>{
            Destroy(obj);
        }));

        seq.Play();

    }
}
