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

    private Sequence m_Sequence;



    [SerializeField] GameObject c_Light;
    [SerializeField] private RawImage c_Title;
    [SerializeField] private TextMeshProUGUI c_Name;
    [SerializeField] private TextMeshProUGUI c_Level;
    [SerializeField] private TextMeshProUGUI c_Description;
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

            if (evt.EventType == _C.COMPLEXTEVEMT.UPGRADE) {
            c_Title.texture = Resources.Load<Texture2D>("UI/Game/Game_upgrade_title_upgrade");
        } else {
            c_Title.texture = Resources.Load<Texture2D>("UI/Game/Game_upgrade_title_complex");
        }
        c_Title.SetNativeSize();



        if (m_Event.EventType == _C.COMPLEXTEVEMT.GLASS)
        {
            c_Icon.texture = Resources.Load<Texture2D>("UI/Texture/Glass");

            c_Name.text     = "碎片";
            c_Level.text    = "";
            c_Description.text = "合成弹珠的材料";

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



            c_Name.text     = m_Ball.Name;
            c_Level.text    = evt.EventType == _C.COMPLEXTEVEMT.NEW ? "新!!" : ("Lv." + m_Ball.Level);
            c_Description.text = m_Ball.GetDescription();

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
        if (m_Sequence != null) {
            m_Sequence.Kill();
            m_Sequence = null;
        }
        

        c_Light.SetActive(flag);

        m_Sequence = DOTween.Sequence();
        
        if (flag == true) {
            transform.SetAsLastSibling();

            var origin_pos = transform.localPosition;

            // 按顺序添加Tween动作
            m_Sequence.Append(transform.DOShakePosition(0.2f, new Vector3(10, 3f, 0) , vibrato: 25, randomness: 50, fadeOut: true).OnComplete(()=>{
                transform.localPosition = origin_pos;
            }));
            m_Sequence.Append(transform.DOScale(1.2f, 0.2f).SetEase(Ease.OutBack));
            

        } else {
            m_Sequence.Append(transform.DOScale(Vector3.one, 0.1f));
        }

        // 启动Sequence
        m_Sequence.Play();
    }


    void OnDestroy()
    {
        if (m_Ball != null) {
            Destroy(m_Ball.gameObject);
        }
    }
}
