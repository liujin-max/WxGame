using System.Collections;
using System.Collections.Generic;
using System.Text;
using CB;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GhostItem : MonoBehaviour
{
    public ComplextEvent m_Event;
    // public BallData m_Config;
    private Ball m_Ball;
    public Ball Ball {get {return m_Ball;}}

    private Tweener m_Tweener;


    public Button Touch;
    [SerializeField] GameObject c_Bubble;
    [SerializeField] GameObject c_Light;
    [SerializeField] private RawImage c_Icon;
    [SerializeField] private TextMeshProUGUI c_Cost;





    void Awake()
    {
        c_Light.SetActive(false);

        EventManager.AddHandler(EVENT.UI_FLUSHCOIN,     OnReponseFlushCoin);
        EventManager.AddHandler(EVENT.UI_FLUSHCOUNT,    OnReponseFlushGlass);
    }

    public void Init(ComplextEvent evt)
    {
        m_Event     = evt;

        if (m_Ball != null) {
            Destroy(m_Ball.gameObject);
        }


        if (m_Event.EventType == _C.COMPLEXTEVEMT.GLASS)
        {
            c_Icon.texture = Resources.Load<Texture2D>("UI/Texture/Glass");
            c_Icon.SetNativeSize();
        }
        else
        {
            var config  = CONFIG.CreateBallData(GameFacade.Instance.CsvManager.GetStringArray(CsvManager.TableKey_Ball, (int)evt.Type));

            c_Icon.texture  = Resources.Load<Texture2D>(config.Icon);
            c_Icon.SetNativeSize();

            GameObject prefab = Resources.Load<GameObject>(config.Ball);
            var obj     = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
            obj.SetActive(false);

            m_Ball    = obj.GetComponent<Ball>();
            m_Ball.Init(m_Event.Type);
            m_Ball.Simulate(false);
            
            if (m_Event.EventType == _C.COMPLEXTEVEMT.NEW) {
                m_Ball.UpgradeTo(1);
            } else {
                var b = GameFacade.Instance.Game.GetBall(m_Event.Type);
                m_Ball.UpgradeTo(b.Level + 1);
            }
        }
            
        FlushCost(evt);
        DoScale();
    }

    void FlushCost(ComplextEvent evt)
    {
        StringBuilder sb = new StringBuilder();

        if (m_Event.EventType == _C.COMPLEXTEVEMT.GLASS)
        {
            if (GameFacade.Instance.Game.Coin < evt.Cost.ToNumber()) {
                sb.Append(_C.REDCOLOR);
            } else if (evt.Cost.ToNumber() < evt.Cost.GetBase()) {
                sb.Append(_C.GREENCOLOR);
            }

            sb.Append(evt.Cost.ToNumber().ToString() + " <sprite=1>");
        }
        else
        {
            if (GameFacade.Instance.Game.Glass < evt.Cost.ToNumber()) {
                sb.Append(_C.REDCOLOR);
            } else if (evt.Cost.ToNumber() < evt.Cost.GetBase()) {
                sb.Append(_C.GREENCOLOR);
            }

            sb.Append(evt.Cost.ToNumber().ToString() + " <sprite=0>");
        }

        c_Cost.text = sb.ToString();
    }

    void DoScale()
    {
        GameFacade.Instance.SoundManager.Load(SOUND.BUBBLE);
        
        c_Bubble.transform.localScale = Vector3.zero;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(c_Bubble.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InBounce));
        sequence.Append(c_Bubble.transform.DOShakeScale(0.25f, 0.4f, vibrato: 15, randomness: 50));
        sequence.Play();
    }


    public void Select(bool flag)
    {
        if (m_Tweener != null) {
            m_Tweener.Kill();
            m_Tweener = null;
        }
        
        c_Light.SetActive(flag);

        if (flag == true) {
            Touch.transform.localScale = Vector3.one;
            // 创建抖动和缩放效果
            m_Tweener = Touch.transform.DOShakeScale(0.3f, 0.5f, vibrato: 15, randomness: 50, fadeOut: true);
        } else {
            m_Tweener = Touch.transform.DOScale(Vector3.one, 0.1f);
        }
    }

    void OnReponseFlushCoin(GameEvent gameEvent)
    {
        this.FlushCost(m_Event);
    }

    void OnReponseFlushGlass(GameEvent gameEvent)
    {
        this.FlushCost(m_Event);
    }

    void OnDestroy()
    {
        if (m_Ball != null) {
            Destroy(m_Ball.gameObject);
        }

        EventManager.DelHandler(EVENT.UI_FLUSHCOIN,     OnReponseFlushCoin);
        EventManager.DelHandler(EVENT.UI_FLUSHCOUNT,    OnReponseFlushGlass);
    }
}
