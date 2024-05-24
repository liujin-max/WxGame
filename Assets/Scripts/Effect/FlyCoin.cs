using UnityEngine;
using DG.Tweening;

public class FlyCoin : MonoBehaviour
{
    private CDTimer m_DelayTimer = null;
    private SpriteRenderer m_Coin;

    private bool m_IsRandomPos;

    public void Fly(float delay = 0, bool is_random_pos = true)
    {
        m_IsRandomPos = is_random_pos;

        m_Coin = transform.Find("coin").GetComponent<SpriteRenderer>();
        m_Coin.gameObject.SetActive(false);

        m_DelayTimer = new CDTimer(delay);
    }

    void DoAnim()
    {
        m_Coin.gameObject.SetActive(true);

        Vector3 startPoint = transform.position;
        if (m_IsRandomPos == true) startPoint = new Vector3(transform.position.x + RandomUtility.Random(-50, 50) / 100.0f, transform.position.y, 0);

        transform.position =  startPoint;

        Vector3 heightPoint = startPoint + new Vector3(0, 1.5f, 0); 

        Vector3 endPoint = startPoint;

        Vector3[] path = new Vector3[] { startPoint, heightPoint, endPoint };

        
        // transform.DOPath(path, 0.7f, PathType.Linear, PathMode.TopDown2D).SetEase(Ease.InOutSine).OnComplete(()=> {
        //     m_Coin.DOFade(0.2f, 0.5f);
        // });
        transform.DOJump(endPoint, 1.5f, 1, 0.7f).SetEase(Ease.OutQuad).OnComplete(() => {
            m_Coin.DOFade(0.2f, 0.5f);
        });
        

        transform.DOLocalRotate(new Vector3(0, 180, 0), 0.25f).SetLoops(-1);
    }

    void Update()
    {
        if (m_DelayTimer != null)
        {
            m_DelayTimer.Update(Time.deltaTime);
            if (m_DelayTimer.IsFinished() == true)
            {
                m_DelayTimer = null;

                DoAnim();
            }
        }
    }
}
