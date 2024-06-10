using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ShineStar : MonoBehaviour
{
    void Awake()
    {
        float scale = RandomUtility.Random(600, 1000) / 1000.0f;
        transform.localScale = Vector3.zero;
        transform.DOScale(scale, 0.4f).SetEase(Ease.OutBack).OnComplete(()=>{
            transform.GetComponent<SpriteRenderer>().DOFade(0, 0.5f);
        });
    }
}
