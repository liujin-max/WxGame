using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

//SwordHitRed
//MysticExplosionOrange
//
namespace CB
{
    public class GameWindow : MonoBehaviour
    {
        
        [SerializeField] private Button c_BtnSet;
        [SerializeField] private GameObject c_BarPivot;
        [HideInInspector] public GameObject BarPivot {get {return c_BarPivot;}}

        [SerializeField] private GameObject c_EffectPivot;
        [SerializeField] private TextMeshProUGUI c_scoreText;
        [SerializeField] private TextMeshProUGUI c_scoreTarget;
        [SerializeField] private Image c_scoreBar;
        private float m_Tscore;
        private float m_Mscore;
        private float m_Cscore;

        [SerializeField] private TextMeshProUGUI c_countText;

        [SerializeField] private TextMeshProUGUI c_coinText;

        [SerializeField] GameObject c_HitPivot;
        [SerializeField] Text c_HitCombo;
        [SerializeField] GameObject c_BubblePivot;
        private TextMeshProUGUI c_BubbleText;

        [SerializeField] GameObject c_ListPivot;

        private bool m_IsHiting = false;
        private CDTimer m_HitTimer = new CDTimer(2f);

        [SerializeField] private GameObject c_BallContent;
        private List<BallItem> m_BallItems = new List<BallItem>();
        [SerializeField] private GameObject c_SeatPivot;
        [HideInInspector] public GameObject SeatPivot{ get {return c_SeatPivot;}}
        private List<BallSeatItem> m_SeatItems = new List<BallSeatItem>();
        [SerializeField] private GameObject c_RelicsPivot;
        private List<RelicsSeatItem> m_RelicsItems = new List<RelicsSeatItem>();


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

        RelicsSeatItem new_relics_item(int order)
        {
            RelicsSeatItem item = null;
            if (m_RelicsItems.Count > order){
                item = m_RelicsItems[order];
            } else {
                GameObject obj = GameFacade.Instance.UIManager.LoadItem("Prefab/UI/Item/RelicsSeatItem", c_RelicsPivot.transform);
                item = obj.GetComponent<RelicsSeatItem>();
                item.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                m_RelicsItems.Add(item);
            }

            item.gameObject.SetActive(true);

            return item;
        }

        void Awake()
        {
            GameFacade.Instance.EventManager.AddHandler(EVENT.UI_FLUSHSCORE,    OnReponseFlushScore);
            GameFacade.Instance.EventManager.AddHandler(EVENT.UI_FLUSHCOUNT,    OnReponseFlushCount);
            GameFacade.Instance.EventManager.AddHandler(EVENT.UI_FLUSHBALLS,    OnReponseFlushBalls);
            GameFacade.Instance.EventManager.AddHandler(EVENT.UI_FLUSHHIT,      OnReponseFlushHit);
            GameFacade.Instance.EventManager.AddHandler(EVENT.UI_FLUSHCOIN,     OnReponseFlushCoin);
            GameFacade.Instance.EventManager.AddHandler(EVENT.UI_SHOWBUBBLE,    OnReponseBubble);
            GameFacade.Instance.EventManager.AddHandler(EVENT.UI_FLYCOIN,       OnReponseFlyCoin);
            GameFacade.Instance.EventManager.AddHandler(EVENT.UI_FLUSHRELICS,   OnReponseFlushRelics);
            

            c_BtnSet.onClick.AddListener(()=>{
                GameFacade.Instance.SoundManager.Load(SOUND.CLICK);

                GameFacade.Instance.Game.Pause();
                GameFacade.Instance.UIManager.LoadWindow("Prefab/UI/GameSetWindow", GameFacade.Instance.UIManager.BOARD);
            });
            

            c_ListPivot.GetComponent<Button>().onClick.AddListener(()=>{
                GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_SHOWBALLLIST, false));
            });

            c_SeatPivot.GetComponent<Button>().onClick.AddListener(()=>{
                GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_SHOWBALLLIST, true));
            });
        }

        // Start is called before the first frame update
        void Start()
        {
            c_scoreText.text = "0";
            c_countText.text = GameFacade.Instance.Game.Glass.ToString();

            c_HitPivot.SetActive(false);
            c_BubblePivot.SetActive(false);
            c_ListPivot.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (m_IsHiting == true) {
                m_HitTimer.Update(Time.deltaTime);
                if (m_HitTimer.IsFinished() == true) {
                    m_HitTimer.Reset();
                    m_IsHiting = false;

                    c_HitPivot.SetActive(false);
                    GameFacade.Instance.Game.ClearHit();
                }
            }

            if (m_Cscore != m_Tscore)
            {
                float step = m_Tscore - m_Cscore;

                float speed;
                if (step >= 0) {
                    speed = math.max(step / 0.5f, 5.0f);
                } else {
                    speed = math.min(step / 0.5f, -15.0f);
                }
                
                m_Cscore += speed * Time.deltaTime; //Math.Min(speed * Time.deltaTime, m_Tscore);

                if (step >= 0) {
                    if (m_Cscore >= m_Tscore) m_Cscore = m_Tscore;
                } else {
                    if (m_Cscore <= m_Tscore) m_Cscore = m_Tscore;
                }
                
                c_scoreBar.fillAmount   = m_Cscore / m_Mscore;
            }
        }

        public BallSeatItem GetBallSeat(Ball ball)
        {
            for (int i = 0; i < m_SeatItems.Count; i++) {
                var item = m_SeatItems[i];
                if (item.Ball != null && item.Ball == ball) {
                    return item;
                }
            }
            return null;
        }

        public RelicsSeatItem GetRelicsSeat(Relics relics)
        {
            for (int i = 0; i < m_RelicsItems.Count; i++) {
                var item = m_RelicsItems[i];
                if (item.m_Relics == relics) {
                    return item;
                }
            }
            return null;
        }

        public void ShowBallList(bool flag, List<Ball> balls, Action<int> callback)
        {
            c_ListPivot.SetActive(flag);

            if (flag == true)  {
                foreach (var item in m_BallItems) {
                    DestroyImmediate(item.gameObject);
                }
                m_BallItems.Clear();

                for (int i = 0; i < balls.Count; i++) {
                    var b = balls[i];
                    var item = GameFacade.Instance.UIManager.LoadItem("Prefab/UI/Item/BallItem", c_BallContent.transform).GetComponent<BallItem>();
                    item.Init(b);
                    m_BallItems.Add(item);
                }

                c_ListPivot.transform.Find("-/ScrollView").GetComponent<SlideScrollView>().Init((object obj)=> {
                    int page = (int)obj;

                    callback(page - 1);
                });
            } 
            else  {
                GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_SHOWBUBBLE, false));
            }
            
        }





















        private void OnReponseFlyCoin(GameEvent gameEvent)
        {
            int coin = Math.Min(7, (int)gameEvent.GetParam(0));

            Vector3 o_pos = c_coinText.transform.parent.transform.position;

            for (int i = 0; i < coin; i++)
            {
                var effect = GameFacade.Instance.EffectManager.Load(EFFECT.FLYCOIN, o_pos);
                effect.GetComponent<FlyCoin>().Fly(0.1f * i); 
            }
        }

        private void OnReponseBubble(GameEvent gameEvent)
        {
            bool show_flag = (bool)gameEvent.GetParam(0);
            c_BubblePivot.SetActive(show_flag);

            if (show_flag == true) {
                if (c_BubbleText == null) {
                    c_BubbleText = c_BubblePivot.transform.Find("Text").GetComponent<TextMeshProUGUI>();
                }
                // c_BubbleText.text = gameEvent.GetParam(1).ToString();
                c_BubbleText.GetComponent<ShakeText>().SetText(gameEvent.GetParam(1).ToString());
            } else {
                c_BubbleText.GetComponent<ShakeText>().Clear();
            }
        }

        private void OnReponseFlushScore(GameEvent gameEvent)
        {
            int score   = (int)gameEvent.GetParam(0);
            int max     = (int)gameEvent.GetParam(1);
            m_Mscore    = max;

            if(m_Tscore < max && score >= max)  {
                GameFacade.Instance.EffectManager.Load(EFFECT.SCORE, Vector3.zero, c_EffectPivot);
            }

            c_scoreText.text    = score.ToString();
            c_scoreTarget.text  = max.ToString();

            m_Tscore    = score;
            if ((bool)gameEvent.GetParam(2) == true) {
                m_Cscore    = score;
                c_scoreBar.fillAmount   = score / max;
            } 
        }

        private void OnReponseFlushCount(GameEvent gameEvent)
        {
            // c_countText.text = GameFacade.Instance.Game.Glass.ToString();
            if ((bool)gameEvent.GetParam(0) == true)
                c_countText.GetComponent<NumberTransition>().SetValue(GameFacade.Instance.Game.Glass);
            else
                c_countText.GetComponent<NumberTransition>().ForceValue(GameFacade.Instance.Game.Glass);
        }

        private void OnReponseFlushBalls(GameEvent gameEvent)
        {
            foreach (var seat in m_SeatItems) {
                seat.gameObject.SetActive(false);
            }

            var add_ball = gameEvent.GetParam(0);

            var balls = GameFacade.Instance.Game.Balls;
            if (GameFacade.Instance.Game.IsPlaying() ) {
                balls = GameFacade.Instance.Game.ShootQueue;
            }


            for (int i = 0; i < GameFacade.Instance.Game.SeatCount.ToNumber(); i++)
            {
                var item = new_seat_item(i);

                if(balls.Count > i) {
                    var ball = balls[i];
                    item.Init(ball);

                    if (add_ball != null && (Ball)add_ball == ball) {
                        item.DoScale();
                    }
                } else {
                    item.Init(null);
                }
            }
        }

        void OnReponseFlushHit(GameEvent gameEvent)
        {
            m_IsHiting  = true;
            m_HitTimer.Reset();

            int hit     = (int)gameEvent.GetParam(0);
            c_HitPivot.SetActive(hit > 0);
            c_HitCombo.text = hit.ToString();

            if (hit > 0)
            {
                c_HitCombo.transform.localScale = Vector3.zero;
                c_HitCombo.transform.DOScale(new Vector3(1.4f, 1.4f, 1.4f), 0.15f).OnComplete(()=> {
                    c_HitCombo.transform.DOScale(Vector3.one, 0.1f);
                });
            }
        }

        void OnReponseFlushCoin(GameEvent gameEvent)
        {
            // c_coinText.text = gameEvent.GetParam(0).ToString();
            if ((bool)gameEvent.GetParam(1) == true)
                c_coinText.GetComponent<NumberTransition>().SetValue((int)gameEvent.GetParam(0));
            else
                c_coinText.GetComponent<NumberTransition>().ForceValue((int)gameEvent.GetParam(0));
        }

        void OnReponseFlushRelics(GameEvent gameEvent)
        {
            foreach (var seat in m_RelicsItems) {
                seat.gameObject.SetActive(false);
            }

            var relicses = GameFacade.Instance.Game.Army.GetRelicses();
            for (int i = 0; i < relicses.Count; i++)
            {
                var item = new_relics_item(i);
                item.Init(relicses[i]);
            }
        }




        void OnDestroy()
        {
            GameFacade.Instance.EventManager.DelHandler(EVENT.UI_FLUSHSCORE,    OnReponseFlushScore);
            GameFacade.Instance.EventManager.DelHandler(EVENT.UI_FLUSHCOUNT,    OnReponseFlushCount);
            GameFacade.Instance.EventManager.DelHandler(EVENT.UI_FLUSHBALLS,    OnReponseFlushBalls);
            GameFacade.Instance.EventManager.DelHandler(EVENT.UI_FLUSHHIT,      OnReponseFlushHit);
            GameFacade.Instance.EventManager.DelHandler(EVENT.UI_FLUSHCOIN,     OnReponseFlushCoin);
            GameFacade.Instance.EventManager.DelHandler(EVENT.UI_SHOWBUBBLE,    OnReponseBubble);
            GameFacade.Instance.EventManager.DelHandler(EVENT.UI_FLYCOIN,       OnReponseFlyCoin);
            GameFacade.Instance.EventManager.DelHandler(EVENT.UI_FLUSHRELICS,   OnReponseFlushRelics);

        }
    }
}