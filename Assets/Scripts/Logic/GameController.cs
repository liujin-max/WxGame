using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using LitJson;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;





namespace CB
{
    public struct ComplextEvent
    {
        public _C.COMPLEXTEVEMT EventType;
        public _C.BALLTYPE Type;

        // public BallData BallConfig;
        public AttributeValue Cost;

    }

    public class GameController: MonoBehaviour
    {


        public Transform c_ballPivot;
        public Transform c_obtPivot;
        public GameObject c_takeAim;
        public Collider2D c_borad;
        private GameWindow _GameUI;
        [HideInInspector] public GameWindow GameUI
        {
            get { return _GameUI; }
            set { _GameUI = value; }
        }

        private Simulator m_Simulator;
        [HideInInspector] public Simulator Simulator {get { return m_Simulator;}}

        private Difficult m_Difficult;
        private EnvironmentController m_Environment;
        [HideInInspector] public EnvironmentController Environment {get { return m_Environment;}}

        private Army m_Army = new Army();
        [HideInInspector] public Army Army {get { return m_Army;}}

 
        private Tweener t_Aim_Tweener;
        private Vector3 m_Aim_Opos;


        #region ======= ======= ======= ======= 
        private FSM<GameController> m_FSM;



        internal bool m_StartFlag = false;
        public bool PauseFlag = false;

        internal int m_ORank = 0;    //初始排名
        internal bool m_IsNewScore = false;
        internal int m_Hit = 0;     //每轮的击打次数

        
        protected int m_Score = 0;
        public int Score {
            get {return Crypt.DE(m_Score);}
            set { m_Score = Crypt.EN(value);}
        }
        protected int m_Stage = 0;
        public int Stage {
            get {return Crypt.DE(m_Stage);}
            set { m_Stage = Crypt.EN(value);}
        }
        protected int m_Coin = 0;    //本局获得的金币
        public int Coin {
            get { return Crypt.DE(m_Coin); }
            set { m_Coin = Crypt.EN(value);}
        }
        protected int m_Glass = 0;   //获得的玻璃碎片
        public int Glass {
            get { return Crypt.DE(m_Glass); }
            set { m_Glass = Crypt.EN(value);}
        }

        //每局复活次数
        private int m_RebornTimes = 0;
        public int RebornTimes{ 
            get {return m_RebornTimes;}
            set { m_RebornTimes = value;}
        }
        internal int m_RefreshTimes = 0;
        [HideInInspector] public AttributeValue RefreshCoin = new AttributeValue(2); //{ get {return 2 + m_RefreshTimes;} }

        private Vector3 m_FingerPos;
        [HideInInspector] public Vector3 FingerPos 
        { 
            get { return m_FingerPos;}
            set { m_FingerPos = value;}
        }

        
        private List<Ball> m_Balls = new List<Ball>();   //球
        [HideInInspector] public List<Ball> Balls { get { return m_Balls;}}

        [HideInInspector] public List<Ball> ShootQueue = new List<Ball>();
        [HideInInspector] public List<Ball> ShootCache = new List<Ball>();    //每回合发射的弹珠记录
        [HideInInspector] public Ball CurrentBall = null;

        private List<Ball> m_BallSmalls = new List<Ball>();
        [HideInInspector] public List<Ball> SmallBalls { get { return m_BallSmalls;}}

        private List<Obstacle> m_Obstacles = new List<Obstacle>();
        [HideInInspector] public List<Obstacle> Obstacles{ get { return m_Obstacles;}}

        private List<Box> m_Boxs = new List<Box>();
        [HideInInspector] public List<Box> Boxs { get {return m_Boxs;}}

        private List<Box> m_Elements = new List<Box>();
        [HideInInspector] public List<Box> Elements { get {return m_Elements;}}

        [HideInInspector] public AttributeValue SeatCount = new AttributeValue(3);     //可携带的弹珠数量上限
        [HideInInspector] public AttributeValue GlassRate = new AttributeValue(15);     //合成列表出现碎片的几率

        private int m_SeatAddition = 0;  //附加的弹珠槽数量
        public int SeatAddition{ get { return m_SeatAddition;}}
        [HideInInspector] public int AdditionPrice      //随着附加数量的增加而递增
        {
            get { 
                int cost = 5 * (int)Math.Pow((m_SeatAddition + 1), 2);
                return cost; 
            }
        }

        //拷贝一份遗物基础数据
        public List<RelicsData> m_RelicsDatas = new List<RelicsData>();
        internal Dictionary<int, RelicsData> m_RelicsDataDic = new Dictionary<int, RelicsData>();


        public bool SwitchBanFlag = false;  //禁止切换弹珠



        #endregion ============== ======= ======= ======= 


        void Awake()
        {
            m_Simulator     = transform.GetComponent<Simulator>();
            m_Difficult     = transform.GetComponent<Difficult>();
            m_Environment   = transform.GetComponent<EnvironmentController>();
            

            m_Aim_Opos  = c_takeAim.transform.localPosition;

            EventManager.AddHandler(EVENT.ONHITOBSTACLE,        OnReponseObstacleHit);
        }

        void OnDestroy()
        {
            EventManager.DelHandler(EVENT.ONHITOBSTACLE,        OnReponseObstacleHit);
        }

        // Start is called before the first frame update
        void Start()
        {
            State<GameController>[] array = {
                new State_START<GameController>(_C.FSMSTATE.GAME_START), 
                new State_RECORD<GameController>(_C.FSMSTATE.GAME_RECORD),
                new State_IDLE<GameController>(_C.FSMSTATE.GAME_IDLE),
                new State_PLAY<GameController>(_C.FSMSTATE.GAME_PLAY),
                new State_COMPLEXT<GameController>(_C.FSMSTATE.GAME_COMPLEX),
                new State_SHOP<GameController>(_C.FSMSTATE.GAME_SHOP),
                new State_END<GameController>(_C.FSMSTATE.GAME_END)
            };

            m_FSM = new FSM<GameController>(this,  array);
        }

        public void Export(bool is_valid = true)
        {
            ArchiveRecord record = new ArchiveRecord();
            record.Valid    = is_valid;
            record.Order    = this.Stage;
            record.Coin     = this.Coin;
            record.Glass    = this.Glass;

            record.RebornTimes  = this.RebornTimes;
            record.SeatAddition = this.m_SeatAddition;
            //


            //存储弹珠数据
            record.BallRecords =  new List<string>();
            this.Balls.ForEach(b => {
                record.BallRecords.Add(b.Export());
            });

            //存储道具数据
            record.RelicsRecords = new List<string>();
            this.Army.GetRelicses().ForEach(relics => {
                record.RelicsRecords.Add(relics.ID.ToString());
            });

            string json = JsonMapper.ToJson(record);
            Debug.Log("存储关卡存档：" + json);
            PlayerPrefs.SetString(SystemManager.KEY_ARCHIVE, json);
        }

        public void Enter(ArchiveRecord record = null)
        {
            if (record == null) {
                DOTransist(_C.FSMSTATE.GAME_START);   
            } else {
                DOTransist(_C.FSMSTATE.GAME_START, record);   
            }
        }

        public void DOTransist(_C.FSMSTATE state, params object[] values)
        {
            m_FSM.Transist(state, values);   
        }

        public bool IsPlaying()
        {
            return m_FSM.CurrentState.ID == _C.FSMSTATE.GAME_PLAY;
        }

        public void UpdateCoin(int value, bool is_reward = true)
        {
            m_Coin = Crypt.EN(Crypt.DE(m_Coin) + value);

            EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHCOIN, Crypt.DE(m_Coin), true));
            EventManager.SendEvent(new GameEvent(EVENT.ONCOINUPDATE, Crypt.DE(m_Coin), value, is_reward));
        }

        public void UpdateScore(int value)
        {
            m_Score = Crypt.EN(Crypt.DE(m_Score) + value);
            
            EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHSCORE, Crypt.DE(m_Score), GetTargetScore(), false));
        }

        public void PushSeatAddition(int value)
        {
            m_SeatAddition += value;
            SeatCount.PutADD(this, m_SeatAddition);

            EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHBALLS));
        }

        public List<Ball> ActingBalls()
        {
            List<Ball> balls = new List<Ball>();

            m_Balls.ForEach(ball => {
                if (ball.IsActing) {
                    balls.Add(ball);
                }
            });

            m_BallSmalls.ForEach(ball => {
                balls.Add(ball);
            });


            return balls;
        }

        public Ball GetBall(_C.BALLTYPE type)
        {
            for (int i = 0; i < m_Balls.Count; i++) {
                var ball = m_Balls[i];
                if (ball.Type == type) {
                    return ball;
                }
            }

            return null;
        }

        public RelicsData GetRelicsData(int id)
        {
            RelicsData data;
            if (m_RelicsDataDic.TryGetValue(id, out data)) {
                return data;
            }

            return data;
        }

        //场上没有发射中的弹珠，也没有正在运作的Element
        public bool IsSceneIdle()
        {
            foreach (var ball in m_Balls) {
                if (ball.IsActing == true) {
                    return false;
                }

                if (ball.IsRecycle == true && ball.IsShow() == true) {
                    return false;
                }
            }

            if (m_Elements.Count > 0) {
                return false;
            }

            if (m_BallSmalls.Count > 0) {
                return false;
            }

            return true;
        }

        public int FibonacciRecursive(int n)
        {
            if (n <= 1)
            {
                return n;
            }
            else
            {
                return FibonacciRecursive(n - 1) + FibonacciRecursive(n - 2);
            }
        }
        

        public int GetTargetScore(int stage = -1)
        {
            if (stage == -1) stage = this.Stage;

            return m_Difficult.GetDifficultyAtStage(stage);


            // int count = 0;
            // for (int i = 1; i <= stage; i++) {
            //     count += i;
            // }

            //每#关，分数要求提升一个台阶
            // int step = (int)(stage / (2 * _C.STAGESTEP + 1));

            // return count * 2 * (step + 1);

            // int step = (int)(stage / (_C.STAGESTEP + 1));
            // int step = (int)Mathf.Ceil(stage / (_C.STAGESTEP + 1) / 2) + 1;

            // return count * FibonacciRecursive(step + 1);
        }

        public bool IsScoreReach()
        {
            return Score >= GetTargetScore();
        }
    
        void LateUpdate()
        {
            m_FSM.Update();

            //清理小球
            List<Ball> _RemoveBalls = new List<Ball>();
            foreach (var ball in m_Balls) {
                if (ball.IsDead() == true) {
                    _RemoveBalls.Add(ball);
                } 
            }

            //清理衍生球
            foreach (var ball in m_BallSmalls) {
                if (ball.IsDead() == true) {
                    _RemoveBalls.Add(ball);
                } 
            }

            if (_RemoveBalls.Count > 0) {
                foreach (var ball in _RemoveBalls) {
                    if (m_Balls.Contains(ball) == true) {
                        m_Balls.Remove(ball);
                    }

                    if (m_BallSmalls.Contains(ball) == true) {
                        m_BallSmalls.Remove(ball);
                    }

                    ball.Dispose();
                }

                EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHBALLS));
            }
            
            
            //清理障碍物
            List<Obstacle> _RemoveObstacles = new List<Obstacle>();
            foreach (var obt in m_Obstacles) {
                if (obt.IsDead() == true) {
                    _RemoveObstacles.Add(obt);
                } 
            }

            foreach (var item in _RemoveObstacles) {
                m_Obstacles.Remove(item);
                item.Dispose();
            }


            //清理Ghost
            List<Box> _RemoveBoxs = new List<Box>();
            foreach (var g in m_Boxs) {
                if (g.IsDead() == true) {
                    _RemoveBoxs.Add(g);
                } 
            }

            //死亡时可能有特殊效果，所以走DoDead逻辑
            foreach (var item in _RemoveBoxs) {
                m_Boxs.Remove(item);
                item.DoDead();
            }


            //清理Elements
            List<Box> _RemoveElements = new List<Box>();
            foreach (var e in m_Elements) {
                if (e.IsDead() == true) {
                    _RemoveElements.Add(e);
                } 
            }

            //死亡时可能有特殊效果，所以走DoDead逻辑
            foreach (var e in _RemoveElements) {
                m_Elements.Remove(e);
                e.DoDead();
            }
        }
        
        public void FocusAim(Vector3 world_pos)
        {
            world_pos.z     = 0;
            Vector3 dir     = world_pos - c_takeAim.transform.localPosition;
            float angle     = Vector3.SignedAngle(Vector3.down, dir, Vector3.forward);
            c_takeAim.transform.localEulerAngles =  new Vector3(0, 0, angle);
        }

        public void AimStart()
        {
            if (t_Aim_Tweener != null) {
                return;
            }

            c_takeAim.transform.localPosition = m_Aim_Opos;
            t_Aim_Tweener = c_takeAim.transform.DOShakePosition(0.05f, 0.07f, 10, 30).SetLoops(-1);
        }

        public void AimStop()
        {
            if (t_Aim_Tweener != null) {
                c_takeAim.transform.localPosition = m_Aim_Opos;

                t_Aim_Tweener.Kill();
                t_Aim_Tweener = null;
            }
        }


        //添加球
        public Ball PushBall(Vector3 pos, _C.BALLTYPE type)
        {
            var config  = CONFIG.GetBallData(type);
            var obj     = Instantiate(Resources.Load<GameObject>(config.Ball), pos, Quaternion.identity, c_ballPivot);
            var ball    = obj.GetComponent<Ball>();
            ball.Init(type);

            if (type == _C.BALLTYPE.SMALL) {
                m_BallSmalls.Add(ball);
            }  else {
                m_Balls.Add(ball);
                
                EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHBALLS));
            }

            EventManager.SendEvent(new GameEvent(EVENT.ONBALLPUSH, ball));

            return ball;
        }

        //上膛
        public Ball BreechBall(Ball ball)
        {
            GameFacade.Instance.SoundManager.Load(SOUND.BREECH);

            ball.Breech();

            return ball;
        }

        public bool BuyGlass(ComplextEvent evt)
        {
            //判断是否足够
            int price = (int)evt.Cost.ToNumber();
            if (Crypt.DE(m_Coin) < price) {
                GameFacade.Instance.FlyTip("<sprite=1> 金币不足");
                return false;
            }

            this.UpdateCoin(-price, false);

            GameFacade.Instance.Game.PushGlass(1);

            return true;
        }

        //合成球
        public bool ComplextBall(ComplextEvent evt)
        {
            if (m_Balls.Count >= this.SeatCount.ToNumber()) {
                GameFacade.Instance.FlyTip("弹珠已满");
                return false;
            }

            int cost_need = (int)evt.Cost.ToNumber();

            //判断是否足够
            if (Glass < cost_need) {
                GameFacade.Instance.FlyTip("<sprite=0> 碎片不足");
                return false;
            }

            GameFacade.Instance.Game.PushGlass(-cost_need, false);

            Ball ball = GameFacade.Instance.Game.PushBall(_C.BALL_ORIGIN_POS, evt.Type);
            GameFacade.Instance.Game.BreechBall(ball);

            m_Balls.Remove(ball);
            m_Balls.Insert(0, ball);

            //上报事件:合成弹珠
            Platform.Instance.REPORTEVENT(CustomEvent.ComplexBall, new Event_ComplexBall((int)ball.Type));



            EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHBALLS, ball));

            EventManager.SendEvent(new GameEvent(EVENT.ONCOMPLEXBALL, ball));

            return true;
        }

        //发射
        public void ShootBall(Ball ball, Vector3 pos)
        {
            GameFacade.Instance.SoundManager.Load(SOUND.SHOOT);
            Platform.Instance.VIBRATE(_C.VIBRATELEVEL.HEAVY);

            c_takeAim.transform.DOShakeScale(0.2f, new Vector3(0, 0.2f, 0), 8, 30).OnComplete(()=>{
                c_takeAim.transform.localScale = Vector3.one;
            });

            ball.Shoot(pos);
            EventManager.SendEvent(new GameEvent(EVENT.ONMANUALSHOOT, ball));
        }



        //生成Obstacle
        public Obstacle PushObstacle(Vector3 pos, int hp, int order = -1)
        {
            int rand    = order;
            if (rand == -1) {
                rand    = RandomUtility.Random(0, _C.ObstaclePrefabs.Length);
            }
            
            var obj     = GameFacade.Instance.PoolManager.AllocateObstacle(rand);
            obj.transform.SetParent(c_obtPivot.transform);
            obj.transform.localPosition     = pos;
            obj.transform.localEulerAngles  = new Vector3(0, 0, RandomUtility.Random(0, 180));

            var script  = obj.GetComponent<Obstacle>();
            script.Init(rand, hp);
            m_Obstacles.Add(script);

            return script;
        }

        public void RecycleObstacle(Obstacle obt)
        {
            GameFacade.Instance.PoolManager.RecycleObstacle(obt);
        }

        //生成Ghost
        public Ghost PushGhost(Vector3 pos)
        {
            var item    = Instantiate(Resources.Load<GameObject>("Prefab/Box/Ghost"), pos, Quaternion.identity, c_obtPivot);
            var script  = item.GetComponent<Ghost>();
            m_Boxs.Add(script);

            return script;
        }

        //生成炸弹
        public Bomb PushBomb(Vector3 pos)
        {
            var item    = Instantiate(Resources.Load<GameObject>("Prefab/Box/Bomb"), pos, Quaternion.identity, c_obtPivot);
            var script  = item.GetComponent<Bomb>();
            m_Boxs.Add(script);

            return script;
        }

        //生成复制
        public Copy PushCopy(Vector3 pos)
        {
            var item    = Instantiate(Resources.Load<GameObject>("Prefab/Box/Copy"), pos, Quaternion.identity, c_obtPivot);
            var script  = item.GetComponent<Copy>();
            m_Boxs.Add(script);

            return script;
        }


        //获得碎片
        public void PushGlass(int value, bool is_reward = true)
        {
            m_Glass = Crypt.EN(Crypt.DE(m_Glass) + value);

            EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHCOUNT, true));
            EventManager.SendEvent(new GameEvent(EVENT.ONGLASSUPDATE, this.Glass, value, is_reward));

        }

        public Box PushElement(string path, Vector3 pos)
        {
            var item    = Instantiate(Resources.Load<GameObject>(path), pos, Quaternion.identity, c_obtPivot);
            var script  = item.GetComponent<Box>();
            m_Elements.Add(script);

            return script;
        }

        public void ClearElements()
        {
            foreach (var obt in m_FSM.Owner.Obstacles) {
                obt.Dispose();
            }
            m_FSM.Owner.Obstacles.Clear();

            //清理box
            foreach (var ghost in m_FSM.Owner.Boxs) {
                ghost.Dispose();
            }
            m_FSM.Owner.Boxs.Clear();
        }

        //重启战场
        public void Restart()
        {
            m_StartFlag = false;

            m_Hit   = 0;
            m_Stage = Crypt.EN(0);
            m_Score = Crypt.EN(0);
            m_Coin  = Crypt.EN(0);
            m_Glass = Crypt.EN(0);

            m_RelicsDatas.Clear();
            m_RelicsDataDic.Clear();

            m_RebornTimes   = 0;
            m_SeatAddition  = 0;
            SeatCount.Clear();
            GlassRate.Clear();

            //清理小球
            foreach (var ball in m_Balls) {
                ball.Dispose();
            }
            m_Balls.Clear();


            //清理小球
            foreach (var ball in m_BallSmalls) {
                ball.Dispose();
            }
            m_BallSmalls.Clear();

            //清理障碍物
            foreach (var obt in m_Obstacles) {
                obt.Dispose();
            }
            m_Obstacles.Clear();

            //清理Ghost
            foreach (var ghost in m_Boxs) {
                ghost.Dispose();
            }
            m_Boxs.Clear();

            //清理Elements
            foreach (var e in m_Elements) {
                e.Dispose();
            }
            m_Elements.Clear();


            m_Army.Dispose();
            m_Environment.OnEnd();

            if (_GameUI != null)
            {
                Destroy(_GameUI.gameObject);
            }
            _GameUI = null;
        }

        public void Pause()
        {
            PauseFlag   = true;

            Physics2D.simulationMode = SimulationMode2D.Script;
            // Time.timeScale  = 0;

            foreach (var ball in m_Balls) {
                ball.Simulate(false);
            }

            foreach (var ball in m_BallSmalls) {
                ball.Simulate(false);
            }
        }

        public void Resume()
        {
            PauseFlag = false;


            Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
            // Time.timeScale = 1;

            foreach (var ball in m_Balls) {
                ball.Simulate(true);
            }

            foreach (var ball in m_BallSmalls) {
                ball.Simulate(true);
            }
        }

        public void ClearHit()
        {
            m_Hit = 0;
        }

        //生成3个选项
        public List<ComplextEvent> GenerateEvents()
        {
            List<ComplextEvent> events = new List<ComplextEvent>();
            int count = 3;

            Dictionary<object, int> keyValuePairs = new Dictionary<object, int>();

            //允许重复
            foreach (BallData c in CONFIG.GetBallDatas())  {
                if (c.Weight > 0) {
                    keyValuePairs.Add(c, c.Weight);
                }
            }


            bool is_glass_sell = false;
            int glass_rate = (int)m_FSM.Owner.GlassRate.ToNumber();
            for (int i = 0; i < count; i++)
            {
                ComplextEvent et    = new ComplextEvent(); 

                if (RandomUtility.IsHit(glass_rate) == true && is_glass_sell == false)    //10%的概率卖碎片
                {
                    is_glass_sell   = true;
                    et.EventType    = _C.COMPLEXTEVEMT.GLASS;
                    et.Cost         = new AttributeValue(_C.GLASSPRICE);
                }
                else
                {
                    BallData config = (BallData)RandomUtility.PickByWeight(keyValuePairs);
                    et.Type         = config.Type;
                    et.EventType    = _C.COMPLEXTEVEMT.NEW;
                    et.Cost         = new AttributeValue(config.Cost);

                    keyValuePairs.Remove(config);
                }

                EventManager.SendEvent(new GameEvent(EVENT.ONCOMPLEXINIT, et));

                events.Add(et);
            }

            return events;
        }

        public List<ComplextEvent> RefreshEvents(bool is_video_play = false)
        {
            if (is_video_play == false)
            {
                int cost = (int)RefreshCoin.ToNumber();
                if (Crypt.DE(m_Coin) < cost) {
                    GameFacade.Instance.FlyTip("<sprite=1> 金币不足");
                    return null;
                }

                this.UpdateCoin(-cost, false);
                m_RefreshTimes += 1;
                RefreshCoin.PutADD(GameFacade.Instance.Game, m_RefreshTimes);
            }

            List<ComplextEvent> events = GameFacade.Instance.Game.GenerateEvents();

            EventManager.SendEvent(new GameEvent(EVENT.ONREFRESHEVENTS, events, is_video_play));

            return events;
        }

        public List<Relics> GenerateRelicses()
        {
            List<Relics> list = new List<Relics>();

            int count = 3;

            Dictionary<object, int> keyValuePairs = new Dictionary<object, int>();
            foreach (RelicsData r in m_RelicsDatas)  {
                if (r.Weight > 0 && r.Unlock == true && m_FSM.Owner.Army.GetRelics(r.ID) == null) {
                    keyValuePairs.Add(r, r.Weight);
                }
            }

            
            for (int i = 0; i < count; i++)
            {
                if (keyValuePairs.Count == 0) break;

                RelicsData config   = (RelicsData)RandomUtility.PickByWeight(keyValuePairs);
                Relics relics   = new Relics(config);

                keyValuePairs.Remove(config);
                list.Add(relics);
            }

            return list;
        }

        public void ShowBallBubble(Ball ball)
        {
            int demage  = (int)ball.Demage.ToADDNumber();
            if (ball.Demage.ToNumber() == 0) demage = 0;

            var des     = string.Format("<#3297FF>{0}<#FF6631>({1})</color>：</color>{2}", ball.Name, demage, ball.GetDescription());
            EventManager.SendEvent(new GameEvent(EVENT.UI_SHOWBUBBLE, true, des));
        }

        public Relics BuyRelics(Relics relics)
        {
            if (Crypt.DE(m_Coin) < relics.Price) {
                GameFacade.Instance.FlyTip("<sprite=1> 金币不足");
                return null;
            }

            this.UpdateCoin(-relics.Price, false);

            Relics new_relics = m_Army.PushRelics(relics.ID);

            //上报事件:购买道具
            Platform.Instance.REPORTEVENT(CustomEvent.BuyRelics, new Event_BuyRelics(relics.ID));

            

            EventManager.SendEvent(new GameEvent(EVENT.ONBUYRELICS, relics));
            EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHSHOP, relics));

            return new_relics;
        }

        public bool BuyBallSeat()
        {
            int cost = this.AdditionPrice;
            if (this.Coin < cost) {
                GameFacade.Instance.FlyTip("<sprite=1> 金币不足");
                return false;
            }

            this.UpdateCoin(-cost, false);

            GameFacade.Instance.Game.PushSeatAddition(1);
            return true;
        }

        //根据当前记录可以得到的金币
        public int GetScoreCoin()
        {
            return Mathf.Min(GameFacade.Instance.User.Score, 100);  //最多给100
        }

        //根据当前记录可以得到的碎片
        public int GetScoreGlass()
        {
            return _C.DEFAULT_GLASS + Mathf.Min((int)(GameFacade.Instance.User.Score / 5.0), 20);
        }

        //范围伤害
        public void Boom(Vector3 center_pos, float radius, int demage)
        {
            GameFacade.Instance.Game.Obstacles.ForEach(obt => {
                if (Vector3.Distance(obt.transform.localPosition, center_pos) <= radius) {
                    obt.OnHit(null, demage);
                }
            });

            //对box同样造成伤害
            GameFacade.Instance.Game.Boxs.ForEach(b => {
                if (Vector3.Distance(b.transform.localPosition, center_pos) <= radius) {
                    b.OnHit(null, demage);
                    b.OnShake();
                }
            });
        }

        public void Dispose()
        {
            m_Army.Dispose();


            if (_GameUI != null)
            {
                Destroy(_GameUI.gameObject);
            }
            _GameUI = null;
        }

        #region 监听事件
        //障碍物扣血
        public void OnReponseObstacleHit(GameEvent gameEvent)
        {
            int value = (int)gameEvent.GetParam(1);
            this.UpdateScore(value);
            
            m_Hit  += 1;
            EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHHIT, m_Hit));
        }

        #endregion
    }































    #region 状态机

    //游戏开始 
    internal class State_START<T> : State<GameController>
    {
        public State_START(_C.FSMSTATE id) : base(id)
        {
        }

        public override void Enter(params object[] values)
        {   
            GameFacade.Instance.Game.Resume();

            m_FSM.Owner.m_StartFlag = true;
            m_FSM.Owner.Balls.Clear();

            m_FSM.Owner.Army.Awake();

            //拷贝遗物数据暂存至战场数据中
            CONFIG.GetRelicsDatas().ForEach(data => {
                var relics_data = data;

                m_FSM.Owner.m_RelicsDatas.Add(relics_data);
                m_FSM.Owner.m_RelicsDataDic[data.ID] = relics_data;
            });

            m_FSM.Owner.GameUI = GameFacade.Instance.UIManager.LoadWindow("Prefab/UI/GameWindow", GameFacade.Instance.UIManager.BOTTOM).GetComponent<GameWindow>();

            // for (int i = 0; i < 50; i++)
            // {
            //     Debug.Log("关卡 ：  " + (i + 1) + ", 难度 ："+ m_FSM.Owner.GetTargetScore(i+1));
            // }

            ArchiveRecord archiveRecord = null;
            if (values.Length > 0) {
                archiveRecord       = (ArchiveRecord)values[0];
                m_FSM.Owner.Coin    = archiveRecord.Coin;
                m_FSM.Owner.Glass   = archiveRecord.Glass;
                m_FSM.Owner.Stage   = archiveRecord.Order;
                m_FSM.Owner.RebornTimes = archiveRecord.RebornTimes;

                m_FSM.Owner.PushSeatAddition(archiveRecord.SeatAddition);

                //读取弹珠数据
                if (archiveRecord.BallRecords != null) {
                    archiveRecord.BallRecords.ForEach(str => {
                        string[] strings = str.Split(',');
                        var ball = m_FSM.Owner.BreechBall(m_FSM.Owner.PushBall(_C.BALL_ORIGIN_POS, (_C.BALLTYPE)Convert.ToInt32(strings[0])));
                        ball.Sync(str);
                    });
                }

                //读取道具数据
                if (archiveRecord.RelicsRecords != null) {
                    archiveRecord.RelicsRecords.ForEach(str => {
                        string[] strings = str.Split(',');
                        m_FSM.Owner.Army.PushRelics(Convert.ToInt32(strings[0]));
                    });
                }
            } else {
                m_FSM.Owner.Coin    = 0;
                m_FSM.Owner.Glass   = 0;
                m_FSM.Owner.Stage   = 0;

                m_FSM.Owner.BreechBall(m_FSM.Owner.PushBall(_C.BALL_ORIGIN_POS, _C.BALLTYPE.NORMAL));
            }

            m_FSM.Owner.Score   = 0;
            m_FSM.Owner.m_ORank = Rank.Instance.GetMyRankOrder();



            EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHCOUNT, false));
            EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHCOIN,  m_FSM.Owner.Coin, false));
            EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHRELICS));
            EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHSCORE, m_FSM.Owner.Score, m_FSM.Owner.GetTargetScore(1), true));
        

            if (archiveRecord != null) {
                m_FSM.Transist(_C.FSMSTATE.GAME_IDLE, true);
            } else {
                // EventManager.SendEvent(new GameEvent(EVENT.ONGAMESTART));

                m_FSM.Transist(_C.FSMSTATE.GAME_RECORD);
            }
        }
    }


    //根据记录获得奖励
    internal class State_RECORD<T> : State<GameController>
    {
        private RecordWindow m_RecordUI;


        public State_RECORD(_C.FSMSTATE id) : base(id)
        {
        }

        public override void Enter(params object[] values)
        {   
            //显示成就奖励
            int m_ACH_Coin = 0;
            int m_ACH_Glass= 0;

            GameFacade.Instance.DataCenter.Achievements.ForEach(achievement => {
                if (achievement.IsFinished == true) {
                    m_ACH_Coin  += achievement.GetCoin();
                    m_ACH_Glass += achievement.GetGlass();
                }
            });

            if (GameFacade.Instance.User.Score > 0 || m_ACH_Coin > 0 || m_ACH_Glass > 0)
            {
                m_RecordUI = GameFacade.Instance.UIManager.LoadWindow("Prefab/UI/RecordWindow", GameFacade.Instance.UIManager.BOTTOM).GetComponent<RecordWindow>();
                m_RecordUI.Init(m_ACH_Coin, m_ACH_Glass);
            }

        }

        public override void Update()
        {
            if (m_RecordUI == null)
            {
                GameFacade.Instance.Game.DOTransist(_C.FSMSTATE.GAME_IDLE, false);
            }
        }

        public override void Exit()
        {
            m_FSM.Owner.Coin  = GameFacade.Instance.Game.GetScoreCoin();
            m_FSM.Owner.Glass = GameFacade.Instance.Game.GetScoreGlass();

            //成就奖励
            GameFacade.Instance.DataCenter.Achievements.ForEach(achievement => {
                if (achievement.IsFinished == true) {
                    achievement.DoReward();
                }
            });


            // m_FSM.Owner.Army.PushRelics(102);
            // CONFIG.GetRelicsDatas().ForEach(x => {
            //     if (x.Weight > 0) m_FSM.Owner.Army.PushRelics(x.ID);
            // });

            EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHCOUNT, true));
            EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHCOIN, m_FSM.Owner.Coin, true));

            m_RecordUI = null;
        }
    }

    //回合开始
    internal class State_IDLE<T> : State<GameController>
    {
        private CDTimer m_DelayTimer = new CDTimer(1.2f);
        private GuideWindow _GuideUI;

        void DrawObstables()
        {
            Vector2 topLeft     = new Vector2(_C.BOARD_LEFT, _C.BOARD_TOP);
            Vector2 bottomRight = new Vector2(_C.BOARD_RIGHT, _C.BOARD_BOTTOM);


            //生成的宝石血量总和 至少要大于目标分数的1.3倍
            int hp_need = m_FSM.Owner.GetTargetScore();      //目标分数

            List<int> temp_list = new List<int>();

            AttributeValue random_count = new AttributeValue(RandomUtility.Random(22, 28));
            int hp_now  = 0;
            int hp_avg  = (int)Mathf.Ceil(hp_need / random_count.ToNumber());  //平均一颗宝石的血量


            //至少有3个碎片
            temp_list.Add((int)_C.BOXTYPE.GHOST);
            temp_list.Add((int)_C.BOXTYPE.GHOST);
            temp_list.Add((int)_C.BOXTYPE.GHOST);

            for (int i = 0; i < 3; i++) {
                if (RandomUtility.IsHit(70))  {
                    temp_list.Add((int)_C.BOXTYPE.GHOST);
                }
            }
            
            //概率有炸弹
            if (RandomUtility.IsHit(45))
            {
                temp_list.Add((int)_C.BOXTYPE.BOMB);
            }

            if (RandomUtility.IsHit(20))
            {
                temp_list.Add((int)_C.BOXTYPE.COPY);
            }

            EventManager.SendEvent(new GameEvent(EVENT.ONDRAWINGOBSTACLE, temp_list, random_count));

            //生成宝石
            for (int i = 0; i < random_count.ToNumber(); i++)
            {
                int hp = (int)Mathf.Ceil(RandomUtility.Random(hp_avg * 250, hp_avg * 450) / 100.0f);
                temp_list.Add(hp);
                hp_now += hp;
            }

            EventManager.SendEvent(new GameEvent(EVENT.ONAFTERDRAWOBT, temp_list));


            int numberOfPoints  = temp_list.Count; //25; 
            
            Vector2[] randomPoints = ToolUtility.GenerateRandomPoints(topLeft, bottomRight, numberOfPoints, _C.OBSTACLE_OFFSET);


            for (int i = 0; i < randomPoints.Length; i++)
            {
                Vector2 point = randomPoints[i];

                int hp = temp_list[i];
                if (hp == (int)_C.BOXTYPE.GHOST) {
                    m_FSM.Owner.PushGhost(point);
                } else if (hp == (int)_C.BOXTYPE.BOMB) {
                    m_FSM.Owner.PushBomb(point);
                } else if (hp == (int)_C.BOXTYPE.COPY) {
                    m_FSM.Owner.PushCopy(point);
                } else {
                    var obt = m_FSM.Owner.PushObstacle(point, hp);
                    obt.DoScale();
                }
            }
        }
    
        public State_IDLE(_C.FSMSTATE id) : base(id)
        {
        }

        public override void Enter(params object[] values)
        {
            m_DelayTimer.Reset();
            
            //存储关卡存档
            m_FSM.Owner.Export(m_FSM.Owner.Stage > 0);


            m_FSM.Owner.Score = 0;
            m_FSM.Owner.Stage = m_FSM.Owner.Stage + 1;


            //
            m_FSM.Owner.Environment.OnInit(m_FSM.Owner.Stage);

            EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHSCORE, m_FSM.Owner.Score, m_FSM.Owner.GetTargetScore(), true));


            //重置弹珠
            //只显示下一颗待发射的弹珠
            for (int i = 0; i < m_FSM.Owner.Balls.Count; i++)
            {
                var ball = m_FSM.Owner.Balls[i];
                m_FSM.Owner.BreechBall(ball);
                ball.Show(i == 0);
            }


            //回合开始动画
            var obj = GameFacade.Instance.EffectManager.Load(EFFECT.ROUND, new Vector3(0, 2, 0));
            obj.GetComponent<RoundText>().Init(m_FSM.Owner.Stage);

            if (GameFacade.Instance.User.IsNewScore(m_FSM.Owner.Stage) == true)
            {
                GameFacade.Instance.EffectManager.Load(EFFECT.SCORETEXT, new Vector3(0, 5.5f, 0));
            }
        }
        
        public override void Update()
        {   
            if (_GuideUI != null) return;

            m_DelayTimer.Update(Time.deltaTime);
            if (m_DelayTimer.IsFinished() == true) {
                //生成宝石
                DrawObstables();

                if (m_FSM.Owner.Stage == 1 && GameFacade.Instance.User.Score == 0 && GameFacade.Instance.SystemManager.GetIntByKey(SystemManager.KEY_GUIDE) == 0) {
                    _GuideUI = GameFacade.Instance.UIManager.LoadWindow("Prefab/UI/GuideWindow", GameFacade.Instance.UIManager.MAJOR).GetComponent<GuideWindow>();
                } else {
                    m_FSM.Transist(_C.FSMSTATE.GAME_PLAY);
                }
            }
        }
    }
















    internal class State_PLAY<T> : State<GameController>
    {

        private bool m_IsPressDown = false;
        private bool m_IsFinished = false;
        private CDTimer m_DelayTimer = new CDTimer(2.0f);

        //点击到了取消区域
        bool IsTouchCancel()
        {
            var world_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return world_pos.y >= _C.CANCELHEIGHT;
        }

        void OnMouseDown()
        {
            //点击在UI上了
            if (ToolUtility.IsPointerOverGameObject(Input.mousePosition)) {
                return;
            }

            
            if (m_FSM.Owner.PauseFlag == true) {
                return;
            }

            //弹珠都打完了
            if (m_FSM.Owner.ShootQueue.Count == 0) {
                return;
            }

            //点击到了取消区域
            if (IsTouchCancel() == true) {
                return;
            }

            //分数达标了
            if (GameFacade.Instance.Game.IsScoreReach()) {
                return;
            }


            m_IsPressDown = true;

            m_FSM.Owner.AimStart();
        }

        void OnMouseUp()
        {
            if (m_IsPressDown != true) return;

            m_IsPressDown = false;
            m_FSM.Owner.Simulator.SimulateEnd();
            EventManager.SendEvent(new GameEvent(EVENT.UI_SHOWBUBBLE, false));

            m_FSM.Owner.AimStop();

            //取消区域
            if (IsTouchCancel() == true) {
                return;
            }


            if (m_FSM.Owner.CurrentBall != null) {
                var ball = m_FSM.Owner.CurrentBall;
                m_FSM.Owner.ShootQueue.Remove(ball);
                m_FSM.Owner.ShootCache.Add(ball);
                GameFacade.Instance.Game.ShootBall(ball, m_FSM.Owner.FingerPos);
            }

            //显示下一颗待发射的弹珠
            if (m_FSM.Owner.ShootQueue.Count > 0) {
                m_FSM.Owner.CurrentBall = m_FSM.Owner.ShootQueue[0];
                m_FSM.Owner.CurrentBall.Show(true);
            }

            EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHBALLS));
        }

        void ReceiveReward()
        {
            //金币跟积分挂钩，最少也给1块钱
            int min = RandomUtility.Random(1, 4);
            int max = RandomUtility.Random(17, 24);
            int coin = Mathf.Clamp((int)Mathf.Floor(m_FSM.Owner.Score / 20.0f), min, max);

            AttributeValue coin_number = new AttributeValue(coin);

            EventManager.SendEvent(new GameEvent(EVENT.ONWILLRECEIVECOIN, coin_number));

            var real_number = (int)coin_number.ToNumber();
            GameFacade.Instance.Game.UpdateCoin(real_number);
  

            EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHSCORE, 0, m_FSM.Owner.GetTargetScore(m_FSM.Owner.Stage + 1),false));
            EventManager.SendEvent(new GameEvent(EVENT.UI_FLYCOIN, real_number));
        }

        public State_PLAY(_C.FSMSTATE id) : base(id)
        {
        }

        public override void Enter(params object[] values)
        {
            m_IsFinished = false;
            m_DelayTimer.Reset();

            m_FSM.Owner.ShootCache.Clear(); 
            m_FSM.Owner.ShootQueue.Clear();
            foreach (var ball in m_FSM.Owner.Balls) {
                m_FSM.Owner.ShootQueue.Add(ball);
            }

            m_FSM.Owner.CurrentBall = m_FSM.Owner.ShootQueue[0];

            m_FSM.Owner.Environment.OnBegin();

            EventManager.SendEvent(new GameEvent(EVENT.ONPLAYSTART));

            EventManager.AddHandler(EVENT.UI_SHOWBALLLIST,  OnReponseBallList);
            
            EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHBALLS));
        }

        public override void Update()
        {
            //监听弹珠发射
            if (Input.GetMouseButtonDown(0)) // 如果按下了左键（鼠标点击）
            {
                OnMouseDown();
            }

            if (m_IsPressDown == true)
            {
                var world_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                m_FSM.Owner.FingerPos   = new Vector3(world_pos.x, Mathf.Min(world_pos.y, _C.CANCELHEIGHT), 0);

                if (this.IsTouchCancel() == true) {
                    m_FSM.Owner.AimStop();
                    m_FSM.Owner.Simulator.SimulateEnd();

                    
                    EventManager.SendEvent(new GameEvent(EVENT.UI_SHOWBUBBLE, true, "     <#0CA90D>取消发射</color>"));

                } else {
                    m_FSM.Owner.FocusAim(m_FSM.Owner.FingerPos);

                    m_FSM.Owner.Simulator.SimulateShoot(m_FSM.Owner.CurrentBall, m_FSM.Owner.FingerPos);

                    GameFacade.Instance.Game.ShowBallBubble(m_FSM.Owner.CurrentBall);
                }

                
            }
            
            if (Input.GetMouseButtonUp(0)) // 如果松开了左键（鼠标释放）
            {
                OnMouseUp();
            }

            if (m_FSM.Owner.PauseFlag == true) return;

            if (m_IsFinished == true) {
                m_DelayTimer.Update(Time.deltaTime);
                if (m_DelayTimer.IsFinished() == true) {
                    if (m_FSM.Owner.Stage % _C.STAGESTEP == 0) { //每3关
                        m_FSM.Transist(_C.FSMSTATE.GAME_SHOP);
                    }
                    else {
                        m_FSM.Transist(_C.FSMSTATE.GAME_COMPLEX);
                    }
                    
                }
                return;
            }

            //场上清空后，不管还有没有剩余弹珠和积分够不够，直接胜利
            // if (m_FSM.Owner.Boxs.Count == 0 && m_FSM.Owner.Obstacles.Count == 0)
            // {
            //     m_IsFinished = true;
            //     ReceiveReward();

            //     m_FSM.Owner.Environment.OnEnd();
            //     EventManager.SendEvent(new GameEvent(EVENT.ONPLAYEND));
            // }


            if (m_FSM.Owner.IsSceneIdle() == true)
            {
                //分数达成 并且场上没有正在运动的弹珠了，则进入下一个阶段
                if (GameFacade.Instance.Game.IsScoreReach() == true) {
                    //判断胜利失败
                    //积分结算
                    m_IsFinished = true;
                    ReceiveReward();

                    m_FSM.Owner.Environment.OnEnd();
                    EventManager.SendEvent(new GameEvent(EVENT.ONPLAYEND));

                    //存储记录
                    if (GameFacade.Instance.User.IsNewScore(m_FSM.Owner.Stage)) {
                        m_FSM.Owner.m_IsNewScore = true;
                    }
                    GameFacade.Instance.User.SetScore(m_FSM.Owner.Stage);
                    GameFacade.Instance.User.Save();
                    

                    Camera.main.GetComponent<CameraUtility>().DoShake();

                    //清理障碍物
                    GameFacade.Instance.Game.ClearElements();

                    
                } else  {
                    if (m_FSM.Owner.ShootQueue.Count == 0) {
                        m_FSM.Owner.Environment.OnEnd();


                        m_FSM.Transist(_C.FSMSTATE.GAME_END, m_FSM.Owner.Stage - 1);
                    }
                }
            }
        }

        public override void Exit()
        {
            EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHBALLS));

            EventManager.DelHandler(EVENT.UI_SHOWBALLLIST,  OnReponseBallList);
        }


        //负责展示球列表
        void OnReponseBallList(GameEvent gameEvent)
        {
            if (m_FSM.Owner.ShootQueue.Count == 0) return;

            if (m_FSM.Owner.SwitchBanFlag == true) 
            {
                GameFacade.Instance.FlyTip("禁止使用");
                return;
            }


            bool flag = (bool)gameEvent.GetParam(0);


            m_FSM.Owner.GameUI.ShowBallList(flag, m_FSM.Owner.ShootQueue, (int order)=> {
                if (order >= m_FSM.Owner.ShootQueue.Count) return;

                var ball = m_FSM.Owner.ShootQueue[order];

                if (m_FSM.Owner.CurrentBall != null) {
                    m_FSM.Owner.CurrentBall.Show(false);
                }
                m_FSM.Owner.CurrentBall = ball;
                m_FSM.Owner.CurrentBall.Show(true);

                GameFacade.Instance.Game.ShowBallBubble(m_FSM.Owner.CurrentBall);
            });

            if (flag == true) {
                if (m_FSM.Owner.CurrentBall != null) {
                    m_FSM.Owner.CurrentBall.Show(false);
                }
                m_FSM.Owner.CurrentBall = m_FSM.Owner.ShootQueue[0];
                m_FSM.Owner.CurrentBall.Show(true);

                GameFacade.Instance.Game.ShowBallBubble(m_FSM.Owner.CurrentBall);

                GameFacade.Instance.Game.Pause();
            } else {              
                var insert_index = 0;
                for (int i = 0; i < m_FSM.Owner.Balls.Count; i++) {
                    var b = m_FSM.Owner.Balls[i];
                    if (b.IsActing != true) {
                        insert_index = i;
                        break;
                    }
                }
                m_FSM.Owner.Balls.Remove(m_FSM.Owner.CurrentBall);
                m_FSM.Owner.Balls.Insert(insert_index, m_FSM.Owner.CurrentBall);

                m_FSM.Owner.ShootQueue.Clear();
                foreach (var ball in m_FSM.Owner.Balls) {
                    if (ball.IsIdle) {
                        m_FSM.Owner.ShootQueue.Add(ball);
                    }
                }

                GameFacade.Instance.Game.Resume();

                EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHBALLS));
            }
        }

    }

















    internal class State_COMPLEXT<T> : State<GameController>
    {
        private GhostWindow m_GhostUI;

        public State_COMPLEXT(_C.FSMSTATE id) : base(id)
        {
        }

        public override void Enter(params object[] values)
        {
            m_FSM.Owner.m_RefreshTimes = 0;
            m_FSM.Owner.RefreshCoin.PutADD(GameFacade.Instance.Game, m_FSM.Owner.m_RefreshTimes);

            EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHBALLS));

            List<ComplextEvent> events = GameFacade.Instance.Game.GenerateEvents();

            m_GhostUI  = GameFacade.Instance.UIManager.LoadWindow("Prefab/UI/GhostWindow", GameFacade.Instance.UIManager.BOARD).GetComponent<GhostWindow>();
            m_GhostUI.Init(events);
        }

        public override void Exit()
        {
            if (m_GhostUI != null) {
                GameObject.Destroy(m_GhostUI.gameObject);
            }
        }
    }









    //商店阶段
    internal class State_SHOP<T> : State<GameController>
    {
        private ShopWindow m_ShopUI;


        public State_SHOP(_C.FSMSTATE id) : base(id)
        {
        }

        public override void Enter(params object[] values)
        {
            EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHBALLS));

            List<Relics> datas = m_FSM.Owner.GenerateRelicses();

            if (datas.Count == 0) {
                m_FSM.Transist(_C.FSMSTATE.GAME_COMPLEX);
                return;
            }

            m_ShopUI  = GameFacade.Instance.UIManager.LoadWindow("Prefab/UI/ShopWindow", GameFacade.Instance.UIManager.BOARD).GetComponent<ShopWindow>();
            m_ShopUI.Init(datas);
        }

        public override void Exit()
        {
            if (m_ShopUI != null) {
                GameObject.Destroy(m_ShopUI.gameObject);
            }
            m_ShopUI = null;
        }
    }










    internal class State_END<T> : State<GameController>
    {
        public State_END(_C.FSMSTATE id) : base(id)
        {
        }

        public override void Enter(params object[] values)
        {
            GameFacade.Instance.Game.Pause();

            m_FSM.Owner.Export(false);      //清空存档

            int real_score      = (int)values[0];

            //记录分数
            GameFacade.Instance.User.SetScore(real_score);
            GameFacade.Instance.User.Save();

            GameFacade.Instance.SoundManager.StopBGM();

            var obj = GameFacade.Instance.UIManager.LoadWindow("Prefab/UI/ResultWindow", GameFacade.Instance.UIManager.BOARD);
            var ui  = obj.GetComponent<ResultWindow>();
            ui.Init(real_score, m_FSM.Owner.m_IsNewScore);
        }
    }


    #endregion

}