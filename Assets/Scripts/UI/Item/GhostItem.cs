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
    [SerializeField] GameObject c_Light;
    [SerializeField] private RawImage c_Icon;
    [SerializeField] private TextMeshProUGUI c_Cost;





    void Awake()
    {
        c_Light.SetActive(false);
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

            StringBuilder sb = new StringBuilder();
            
            if (GameFacade.Instance.Game.m_Coin < _C.GLASSPRICE) {
                sb.Append(_C.REDCOLOR);
            } 
            sb.Append(_C.GLASSPRICE.ToString() + " <sprite=1>");

            c_Cost.text = sb.ToString();

        }
        else
        {
            m_Event.BallConfig  = CONFIG.CreateBallData(GameFacade.Instance.CsvManager.GetStringArray(CsvManager.TableKey_Ball, (int)evt.Type));
            var config      = m_Event.BallConfig;
            c_Icon.texture  = Resources.Load<Texture2D>(config.Icon);


            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.ONCOMPLEXINIT, m_Event, config));


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

            StringBuilder sb = new StringBuilder();

            if (GameFacade.Instance.Game.Glass < config.Cost.ToNumber()) {
                sb.Append(_C.REDCOLOR);
            } else if (config.Cost.ToNumber() < config.Cost.GetBase()) {
                sb.Append(_C.GREENCOLOR);
            }

            sb.Append(config.Cost.ToNumber().ToString() + " <sprite=0>");
            
            c_Cost.text = sb.ToString();
        }
            


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


    void OnDestroy()
    {
        if (m_Ball != null) {
            Destroy(m_Ball.gameObject);
        }
    }
}
