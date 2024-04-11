using UnityEngine;
using DG.Tweening;

public class FlyGlass : MonoBehaviour
{

    private SpriteRenderer m_Coin;

    void Awake()
    {
        m_Coin = transform.Find("coin").GetComponent<SpriteRenderer>();

        DoAnim();
    }

    void DoAnim()
    {
        Vector3 startPoint  = transform.position;
        Vector3 heightPoint = startPoint + new Vector3(0, 1f, 0); 


        Vector3[] path = new Vector3[] { startPoint, heightPoint };

        
        transform.DOPath(path, 0.6f, PathType.Linear, PathMode.TopDown2D).SetEase(Ease.OutCirc);
        m_Coin.DOFade(0f, 0.6f);
        transform.DOLocalRotate(new Vector3(0, 0, 180), 0.25f).SetLoops(-1);
    }
}
