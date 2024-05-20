using System.Collections;
using System.Collections.Generic;
using CB;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
// using UnityEngine.UIElements;
public class GhostWindow : MonoBehaviour
{
    [SerializeField] private GameObject c_Mask;
    [SerializeField] private Transform c_Pivot;
    [SerializeField] private Button c_SeatPivot;
    [SerializeField] private Button c_BtnSelect;
    [SerializeField] private Button c_BtnCancel;
    [SerializeField] private Button c_BtnRefresh;
    [SerializeField] private Button c_BtnGlass;
    [SerializeField] private Button c_BtnVideoRefresh;
    [SerializeField] private GameObject c_ButtonPivot;
    [SerializeField] private GameObject c_DescriptionPivot;
    [SerializeField] private TextMeshProUGUI c_Tip;

    private GhostItem m_SelectGhost = null;
    private List<GhostItem> m_GhostItems = new List<GhostItem>();
    private List<BallSeatItem> m_SeatItems = new List<BallSeatItem>();
    private CDTimer m_CDTimer = new CDTimer(0.6f);

    BallSeatItem new_seat_item(int order)
    {
        BallSeatItem item = null;
        if (m_SeatItems.Count > order){
            item = m_SeatItems[order];
        } else {
            GameObject obj = GameFacade.Instance.UIManager.LoadItem("Prefab/UI/Item/BallSeatItem", c_SeatPivot.transform);
            item = obj.GetComponent<BallSeatItem>();

            m_SeatItems.Add(item);
        }

        item.gameObject.SetActive(true);

        return item;
    }

    GhostItem new_ghost_item(int order)
    {
        GhostItem item = null;
        if (m_GhostItems.Count > order){
            item = m_GhostItems[order];
        } else {
            GameObject obj = GameFacade.Instance.UIManager.LoadItem("Prefab/UI/Item/GhostItem", c_Pivot);
            item = obj.GetComponent<GhostItem>();
            item.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
            m_GhostItems.Add(item);
        }

        item.gameObject.SetActive(true);

        return item;
    }

    void Awake()
    {
        EventManager.AddHandler(EVENT.UI_FLUSHBALLS,    OnReponseFlushBalls);
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.UI_FLUSHBALLS,    OnReponseFlushBalls);
    }

    void Start()
    {
        c_DescriptionPivot.SetActive(false);

        c_BtnRefresh.gameObject.SetActive(false);
        c_BtnSelect.gameObject.SetActive(false);
        c_BtnCancel.gameObject.SetActive(false);
        c_BtnGlass.gameObject.SetActive(false);
        c_BtnVideoRefresh.gameObject.SetActive(false);

        c_BtnSelect.onClick.AddListener(()=> {
            var flag = false;

            if (m_SelectGhost.m_Event.EventType == _C.COMPLEXTEVEMT.GLASS)
            {
                flag= GameFacade.Instance.Game.BuyGlass(m_SelectGhost.m_Event);
            }
            else
            {
                flag = GameFacade.Instance.Game.ComplextBall(m_SelectGhost.m_Event);
            }


            if (flag == true) {
                GameFacade.Instance.SoundManager.Load(SOUND.COST);

                GameFacade.Instance.EffectManager.LoadUIEffect(EFFECT.BUBBLEBREAK, m_SelectGhost.transform.position);
                

                List<ComplextEvent> events = GameFacade.Instance.Game.GenerateEvents();
                if (events != null) {
                    this.Init(events);
                }
            }
        });

        c_BtnCancel.onClick.AddListener(()=> {
            GameFacade.Instance.SoundManager.Load(SOUND.CLICK);

            if (GameFacade.Instance.Game.Balls.Count > 0) {
                GameFacade.Instance.Game.DOTransist(_C.FSMSTATE.GAME_IDLE, false);
            } else {
                GameFacade.Instance.FlyTip("请选择合成一颗弹珠");
                m_GhostItems.ForEach(item => {
                    item.Shake();
                });
            //    GameFacade.Instance.Game.DOTransist(_C.FSMSTATE.GAME_END, GameFacade.Instance.Game.Stage);
            }
        });

        c_BtnRefresh.onClick.AddListener(()=>{
            GameFacade.Instance.SoundManager.Load(SOUND.CLICK);

            List<ComplextEvent> events = GameFacade.Instance.Game.RefreshEvents();
            if (events != null) {
                GameFacade.Instance.SoundManager.Load(SOUND.COST);
                this.Init(events);
            }
        });

        //看广告，获取碎片
        c_BtnGlass.onClick.AddListener(()=>{
            GameFacade.Instance.Game.PushGlass(1, false);
            GameFacade.Instance.EffectManager.LoadUIEffect(EFFECT.FLYGLASS, c_BtnGlass.transform.position);
        });

        //看广告，刷新
        c_BtnVideoRefresh.onClick.AddListener(()=>{
            List<ComplextEvent> events = GameFacade.Instance.Game.RefreshEvents(is_video_play : true);
            if (events != null) {
                this.Init(events);
            }
        });

        c_SeatPivot.onClick.AddListener(()=>{
            GameFacade.Instance.Popup(string.Format("花费 {0}<sprite=1> 购买一个弹珠槽？", GameFacade.Instance.Game.AdditionPrice), ()=>{
                if (GameFacade.Instance.Game.BuyBallSeat() == true) {
                    GameFacade.Instance.SoundManager.Load(SOUND.COST);
                }
            });
        });

        //适配遮罩高度
        var seat_pivot = GameFacade.Instance.Game.GameUI.SeatPivot;
        var pos = new Vector3(seat_pivot.transform.position.x * 100, seat_pivot.transform.position.y * 100, 0);
        c_Mask.GetComponent<UIMaskUtility>().SetCenter(pos);
    }

    public void Init(List<ComplextEvent> events)
    {
        FlushUI();
        InitEvents(events);
        InitBalls();
    }

    void FlushUI()
    {
        c_Tip.text = string.Format("消耗 <sprite=0>（拥有{0}个）合成弹珠", GameFacade.Instance.Game.Glass);

        int cost = (int)GameFacade.Instance.Game.RefreshCoin.ToNumber();
        if (GameFacade.Instance.Game.Coin >= cost) {
            c_BtnRefresh.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = cost + " <sprite=1>";
        } else {
            c_BtnRefresh.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = _C.REDCOLOR + cost + " <sprite=1>";
        }
        
    }


    public void InitEvents(List<ComplextEvent> events)
    {
        if (m_SelectGhost != null) {
            m_SelectGhost.Select(false);
        }
        m_SelectGhost = null;

        foreach (var item in m_GhostItems) {
            item.gameObject.SetActive(false);
        }

        c_BtnSelect.gameObject.SetActive(false);
        c_DescriptionPivot.SetActive(false);


        int count = events.Count;

        for (int i = 0; i < events.Count; i++)
        {
            var evt = events[i];
            GhostItem item = new_ghost_item(i);
            item.transform.localPosition = new Vector3((i - ((events.Count - 1) / 2.0f)) * 300, 0, 0);
            item.Init(evt);

            item.Touch.GetComponent<MouseClickHandler>().Init(()=>{
                GameFacade.Instance.SoundManager.Load(SOUND.BUBBLE);
                Platform.Instance.VIBRATE(_C.VIBRATELEVEL.LIGHT);
                
                if (m_SelectGhost != null) {
                    m_SelectGhost.Select(false);
                }

                m_SelectGhost = item;
                m_SelectGhost.Select(true);

                c_BtnSelect.gameObject.SetActive(true);

                this.ShowDescription(evt);
            });
        }
    }

    void ShowDescription(ComplextEvent evt)
    {
        c_DescriptionPivot.SetActive(true);
        c_DescriptionPivot.transform.position = m_SelectGhost.transform.position + new Vector3(0, 2.5f, 0);


        var des_text = c_DescriptionPivot.transform.Find("Text").GetComponent<ShakeText>();


        if (evt.EventType == _C.COMPLEXTEVEMT.GLASS)
        {
            des_text.SetText("<#3297FF>碎片：</color>合成弹珠需要的材料");
        }
        else
        {
            var ball = m_SelectGhost.Ball;
            des_text.SetText(string.Format("<#3297FF>{0}<#FF6631>({1})</color>：</color>{2}", ball.Name, ball.Demage.ToNumber(), ball.GetDescription()));
        }
    }

    void InitBalls(Ball add_ball = null)
    {
        foreach (var seat in m_SeatItems) {
            seat.gameObject.SetActive(false);
        }

        var balls = GameFacade.Instance.Game.Balls;

        int max = (int)GameFacade.Instance.Game.SeatCount.ToNumber() + 1;
        for (int i = 0; i < max; i++)
        {
            var item = new_seat_item(i);

            if(balls.Count > i) {
                var ball = balls[i];
                item.Init(ball);

                if (add_ball != null && add_ball == ball) {
                    item.DoScale();
                }
            } else if (i == max - 1) {
                item.InitADD();
            } else {
                item.Init(null);
            }
        }
    }

    void Update()
    {
        if (m_CDTimer == null) return;

        m_CDTimer.Update(Time.deltaTime);
        if (m_CDTimer.IsFinished() == true)  {
            m_CDTimer = null;

            c_BtnRefresh.gameObject.SetActive(true);
            c_BtnCancel.gameObject.SetActive(true);
            // c_BtnGlass.gameObject.SetActive(true);
            // c_BtnVideoRefresh.gameObject.SetActive(true);
        }
    }



    private void OnReponseFlushBalls(GameEvent gameEvent)
    {
        Ball add_ball = (Ball)gameEvent.GetParam(0);

        InitBalls(add_ball);
    }
}
