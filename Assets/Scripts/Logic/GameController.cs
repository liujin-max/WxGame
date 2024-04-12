using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;




namespace CB
{
    public struct ComplextEvent
    {
        public _C.COMPLEXTEVEMT EventType;
        public _C.BALLTYPE Type;

        public BallData BallConfig;

    }

    public class GameController: MonoBehaviour
    {


        public Transform c_ballPivot;
        public Transform c_obtPivot;
        public GameObject c_takeAim;
        public Collider2D c_borad;
        internal GameWindow _GameUI;

        internal Simulator m_Simulator;
        internal EnvironmentController m_Environment;

        //伤害统计工具 
        public Dictionary<_C.BALLTYPE, int> DemageRecords = new Dictionary<_C.BALLTYPE, int>();

        private Tweener t_Aim_Tweener;
        private Vector3 m_Aim_Opos;


        #region ======= ======= ======= ======= 
        private FSM<GameController> m_FSM;
        [SerializeField] private int FSM_State;


        internal bool m_StartFlag = false;
        internal int m_Score = 0;
        internal int m_Stage = 0;
        public int Stage {get {return m_Stage;}}

        internal int m_Hit = 0;     //每轮的击打次数
        internal int m_Coin = 0;    //本局获得的金币
        internal int m_Glass = 0;   //获得的玻璃碎片
        public int Glass
        {
            get { return m_Glass; }
            set { m_Glass = value;}
        }

        internal int m_RefreshTimes;

        public int RefreshCoin { get {return 3 + m_RefreshTimes;} }

        private Vector3 m_FingerPos;
        public Vector3 FingerPos 
        { 
            get { return m_FingerPos;}
            set { m_FingerPos = value;}
        }


        public int TargetScore 
        {
            //当前阶段的目标分数 
            get {
                int count = 0;
                for (int i = 1; i <= m_Stage; i++) {
                    count += i;
                }

                int rate = 0;
                
                if (m_Stage <= 10) {
                    rate = (int)Mathf.Ceil(m_Stage / 7.0f) * 5;
                } 
                else {
                    rate = (int)Mathf.Ceil(m_Stage / 5.0f) * 4;
                }
                

                return rate * count;
            }
        }


        
        internal List<Ball> m_Balls = new List<Ball>();   //球
        internal List<Ball> m_BallSmalls = new List<Ball>();
        internal List<Obstacle> m_Obstacles = new List<Obstacle>();
        internal List<Ghost> m_Ghosts = new List<Ghost>();

        public List<Obstacle> Obstacles { get {return m_Obstacles;}}
        public List<Ghost> Ghosts { get {return m_Ghosts;}}
        public AttributeValue SeatCount = new AttributeValue(5);     //可携带的高阶球数量上限
        public Army m_Army = new Army();



        #endregion ============== ======= ======= ======= 


        void Awake()
        {
            m_Simulator = transform.GetComponent<Simulator>();
            m_Environment = transform.GetComponent<EnvironmentController>();

            m_Aim_Opos  = c_takeAim.transform.localPosition;

            GameFacade.Instance.EventManager.AddHandler(EVENT.ONOBSTACLEHIT,        OnReponseObstacleHit);
        }

        void OnDestroy()
        {
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONOBSTACLEHIT,        OnReponseObstacleHit);
        }


        // Start is called before the first frame update
        void Start()
        {
            State<GameController>[] array = {
                new State_START<GameController>(_C.FSMSTATE.GAME_START), 
                new State_IDLE<GameController>(_C.FSMSTATE.GAME_IDLE),
                new State_PLAY<GameController>(_C.FSMSTATE.GAME_PLAY),
                new State_COMPLEXT<GameController>(_C.FSMSTATE.GAME_COMPLEX),
                new State_SHOP<GameController>(_C.FSMSTATE.GAME_SHOP),
                new State_END<GameController>(_C.FSMSTATE.GAME_END)
            };

            m_FSM = new FSM<GameController>(this,  array);

            m_Army.Awake();


            //测试代码
            // for (int i = 1; i < 35; i++)
            // {
            //     m_Stage = i;
            //     Debug.Log("Stage：" + i + " : " + TargetScore);
            // }
        }

        public void Enter()
        {
            DOTransist(_C.FSMSTATE.GAME_START);   
        }

        public void DOTransist(_C.FSMSTATE state)
        {
            m_FSM.Transist(state);   
        }

        public void UpdateCoin(int value)
        {
            m_Coin += value;

            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHCOIN, m_Coin));
            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.ONCOINUPDATE, m_Coin));
        }

        public List<Ball> GetBalls()
        {
            return m_Balls;
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

        public bool IsBallsIdle()
        {
            foreach (var ball in m_FSM.Owner.m_Balls) {
                if (ball.IsIdle == false) {
                    return false;
                }
            }
            return true;
        }

    
        void LateUpdate()
        {
            FSM_State = (int)m_FSM.CurrentState.ID;
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

                GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHBALLS));
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
            List<Ghost> _RemoveGhosts = new List<Ghost>();
            foreach (var ghost in m_Ghosts) {
                if (ghost.IsDead() == true) {
                    _RemoveGhosts.Add(ghost);
                } 
            }

            foreach (var item in _RemoveGhosts)
            {
                m_Ghosts.Remove(item);
                item.Dispose();
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
            GameObject prefab = Resources.Load<GameObject>(config.Ball);
            var obj     = Instantiate(prefab, pos, Quaternion.identity, c_ballPivot);
            var ball    = obj.GetComponent<Ball>();
            ball.Init(type);

            if (type == _C.BALLTYPE.SMALL) {
                m_BallSmalls.Add(ball);
            }  else {
                m_Balls.Add(ball);

                GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHBALLS));
            }

            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.ONBALLPUSH, ball));

            return ball;
        }

        //上膛
        public void BreechBall(Ball ball)
        {
            GameFacade.Instance.SoundManager.Load(SOUND.BREECH);

            ball.Velocity = Vector2.zero;
            ball.transform.localPosition = _C.BALL_ORIGIN_POS;
            ball.SetState((int)_C.LAYER.BALLIDLE);
        }

        //合成球
        public bool ComplextBall(ComplextEvent evt)
        {
            Ball ball = null;

            if(evt.EventType == _C.COMPLEXTEVEMT.GLASS)
            {
                //判断是否足够
                int price = _C.GLASSPRICE;
                if (m_Coin < price) {
                    GameFacade.Instance.FlyTip("<sprite=1> 金币不足");
                    return false;
                }

                this.UpdateCoin(-price);

                GameFacade.Instance.Game.PushGlass(1);
            }
            else
            {
                // BallData config = CONFIG.GetBallData(evt.Type);
                var config  = evt.BallConfig;


                int cost_need = (int)config.Cost.ToNumber();

                //判断是否足够
                if (Glass < cost_need) {
                    GameFacade.Instance.FlyTip("<sprite=0> 碎片不足");
                    return false;
                }

                GameFacade.Instance.Game.PushGlass(-cost_need);

                if (evt.EventType == _C.COMPLEXTEVEMT.UPGRADE) {
                    ball = this.GetBall(evt.Type);
                    ball.UpgradeTo(ball.Level + 1);
                } else {
                    ball = GameFacade.Instance.Game.PushBall(_C.BALL_ORIGIN_POS, evt.Type);
                    GameFacade.Instance.Game.BreechBall(ball);
                }

                GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHBALLS));

                GameFacade.Instance.EffectManager.Load(EFFECT.COMPLEX, _C.BALL_ORIGIN_POS);
            }

            



    
            

            return true;
        }

        //发射
        public void ShootBall(Ball ball, Vector3 pos)
        {
            GameFacade.Instance.SoundManager.Load(SOUND.SHOOT);

            c_takeAim.transform.DOShakeScale(0.2f, new Vector3(0, 0.2f, 0), 8, 30).OnComplete(()=>{
                c_takeAim.transform.localScale = Vector3.one;
            });

            Debug.Log("ShootBall : " + ball.Type);
            ball.Shoot(pos);
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
        public void PushGhost(Vector3 pos)
        {
            var item    = Instantiate(Resources.Load<GameObject>("Prefab/Ghost/Ghost"), pos, Quaternion.identity, c_obtPivot);
            var script  = item.GetComponent<Ghost>();
            m_Ghosts.Add(script);
        }

        //获得碎片
        public void PushGlass(int value)
        {
            this.m_Glass += value;

            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHCOUNT));
            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.ONGLASSUPDATE, m_Glass));

        }


        //重启战场
        public void Restart()
        {
            m_StartFlag = false;

            m_Hit   = 0;
            m_Score = 0;
            m_Coin  = 0;
            m_Stage = 0;
            m_Glass = 0;


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
            foreach (var ghost in m_Ghosts) {
                ghost.Dispose();
            }
            m_Ghosts.Clear();

            m_Army.Dispose();

            if (_GameUI != null)
            {
                Destroy(_GameUI.gameObject);
            }
            _GameUI = null;
        }

        public void Pause()
        {
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

        public void InsertDamageRecord(_C.BALLTYPE type , int demage)
        {
            
            int value;
            if (DemageRecords.TryGetValue(type, out value) == true)
            {
                DemageRecords[type] = value + demage;
            }
            else
            {
                DemageRecords.Add(type, demage);
            }
        }

        //生成3个升级选项，不能重复
        public List<ComplextEvent> GenerateEvents()
        {
            List<ComplextEvent> events = new List<ComplextEvent>();
            int count = 3;

            Dictionary<object, int> keyValuePairs = new Dictionary<object, int>();

            List<Ball> balls = this.GetBalls();
            //如果槽位满了，则只从当前已有的弹珠中随机
            if (balls.Count >= SeatCount.ToNumber())
            {
                foreach (var ball in balls) {
                    var c = CONFIG.GetBallData(ball.Type);
                    keyValuePairs.Add(c, c.Weight);
                }
            }
            else 
            {
                foreach (BallData c in CONFIG.GetBallDatas())  {
                    if (c.Weight > 0) {
                        keyValuePairs.Add(c, c.Weight);
                    }
                }
            }

            bool is_glass_sell = false;
            for (int i = 0; i < count; i++)
            {
                BallData config     = (BallData)RandomUtility.PickByWeight(keyValuePairs);
                ComplextEvent et    = new ComplextEvent(); 

                if (RandomUtility.IsHit(10) == true && is_glass_sell == false)    //10%的概率卖碎片
                {
                    is_glass_sell   = true;

                    et.EventType = _C.COMPLEXTEVEMT.GLASS;
                }
                else
                {
                    et.Type = config.Type;

                    var ball = m_FSM.Owner.GetBall(config.Type);
                    if (ball != null ) {
                        et.EventType = _C.COMPLEXTEVEMT.UPGRADE;
                    } else {
                        et.EventType = _C.COMPLEXTEVEMT.NEW;
                    }

                    keyValuePairs.Remove(config);
                }

                events.Add(et);
            }

            return events;
        }

        public List<ComplextEvent> RefreshEvents()
        {
            int cost = RefreshCoin;
            if (m_Coin < cost) {
                GameFacade.Instance.FlyTip("<sprite=0> 金币不足");
                return null;
            }

            this.UpdateCoin(-cost);
            m_RefreshTimes += 1;

            List<ComplextEvent> events = GameFacade.Instance.Game.GenerateEvents();

            return events;
        }

        public void ShowBallBubble(Ball ball)
        {
            var des     = string.Format("<#3297FF>{0}<#FF6631>({1})</color>：</color>{2}", ball.Name, ball.m_Demage.ToNumber(), ball.GetDescription());
            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_SHOWBUBBLE, true, des));
        }

        public Relics BuyRelics(Relics relics)
        {
            if (this.m_Army.IsFull() == true)
            {
                GameFacade.Instance.FlyTip("道具已满");
                return null;
            }

            if (m_Coin < relics.Price) {
                GameFacade.Instance.FlyTip("<sprite=1> 金币不足");
                return null;
            }

            this.UpdateCoin(-relics.Price);

            Relics new_relics = m_Army.PushRelics(relics.ID);

            return new_relics;
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


        #region ==== 监听事件 ====
        //障碍物扣血
        public void OnReponseObstacleHit(GameEvent gameEvent)
        {
            int value = (int)gameEvent.GetParam(0);
            m_Score+= value;
            m_Hit  += 1;

            //伤害记录
            Ball ball = (Ball)gameEvent.GetParam(1);
            this.InsertDamageRecord(ball.Type == _C.BALLTYPE.SMALL ? _C.BALLTYPE.SPLIT : ball.Type, value);


            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHSCORE, m_Score, false));
            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHHIT, m_Hit));
        }

        #endregion
    }































    #region ==========  状态机  ==========

    //游戏开始 
    internal class State_START<T> : State<GameController>
    {
        public State_START(_C.FSMSTATE id) : base(id)
        {
        }

        public override void Enter()
        {   
            m_FSM.Owner.m_StartFlag = true;
            m_FSM.Owner.m_Balls.Clear();

            m_FSM.Owner._GameUI = GameFacade.Instance.UIManager.LoadWindow("Prefab/UI/GameWindow", GameFacade.Instance.UIManager.BOTTOM).GetComponent<GameWindow>();

            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHCOUNT));
            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHCOIN, m_FSM.Owner.m_Coin));
            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHRELICS));
        
            GameFacade.Instance.Game.Resume();

            GameFacade.Instance.Game.DemageRecords.Clear();

            m_FSM.Owner.BreechBall(m_FSM.Owner.PushBall(_C.BALL_ORIGIN_POS, _C.BALLTYPE.NORMAL));

            // m_FSM.Owner.m_Army.PushRelics(109);


            m_FSM.Transist(_C.FSMSTATE.GAME_IDLE);
        }
    }




    //回合开始
    internal class State_IDLE<T> : State<GameController>
    {
        private CDTimer m_DelayTimer = new CDTimer(1.2f);
        private GuideWindow _GuideUI;

        public Vector2[] GenerateRandomPoints(Vector2 topLeft, Vector2 bottomRight, int N, float minDistance)
        {
            List<Vector2> points = new List<Vector2>();
            HashSet<Vector2> usedPoints = new HashSet<Vector2>();

            int count = 0;  //防卡死机制
            while (points.Count < N)
            {
                Vector2 randomPoint = new Vector2(RandomUtility.Random((int)(topLeft.x * 100), (int)(bottomRight.x * 100)) / 100.0f, RandomUtility.Random((int)(bottomRight.y * 100), (int)(topLeft.y * 100)) / 100.0f);

                bool isValid = true;
                foreach (Vector2 existingPoint in usedPoints)
                {
                    if (Vector2.Distance(randomPoint, existingPoint) < minDistance)
                    {
                        isValid = false;
                        break;
                    }
                }

                if (isValid)
                {
                    count = 0;
                    points.Add(randomPoint);
                    usedPoints.Add(randomPoint);
                }
                else
                {
                    count++;
                }

                //单次随机次数超出1000次，直接跳出，免得死循环了
                if (count >= 1000) {
                    Debug.LogError("测试输出 order ： " + points.Count);
                    break;
                }
            }

            return points.ToArray();
        }

        void DrawObstables()
        {
            Vector2 topLeft     = new Vector2(_C.BOARD_LEFT, _C.BOARD_TOP);
            Vector2 bottomRight = new Vector2(_C.BOARD_RIGHT, _C.BOARD_BOTTOM);


            //生成的宝石血量总和 至少要大于目标分数的1.3倍
            int hp_need = (int)Math.Floor(m_FSM.Owner.TargetScore * 1.0f);      //目标分数

            List<int> temp_list = new List<int>();


            int random_count = RandomUtility.Random(24, 28);
            int hp_now  = 0;
            int hp_avg  = (int)Mathf.Ceil(hp_need / (random_count * 1.0f));  //平均一颗宝石的血量
            int count   = 0;

            for (int i = 0; i < random_count; i++)
            {
                if (RandomUtility.IsHit(10) && count < 5) { //7% 最多3个
                    count++;
                    temp_list.Add(-1);
                }

                int hp = (int)Mathf.Ceil(RandomUtility.Random(hp_avg * 250, hp_avg * 450) / 100.0f);
                temp_list.Add(hp);
                hp_now += hp;
            }

            //至少有一个碎片
            if (count == 0) {
                temp_list.Add(-1);
            }

            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.ONDRAWINGOBSTACLE, temp_list));


            int numberOfPoints  = temp_list.Count; //25; 
            
            Vector2[] randomPoints = GenerateRandomPoints(topLeft, bottomRight, numberOfPoints, _C.OBSTACLE_OFFSET);


            for (int i = 0; i < randomPoints.Length; i++)
            {
                Vector2 point = randomPoints[i];

                int hp = temp_list[i];
                if (hp == -1) {
                    m_FSM.Owner.PushGhost(point);
                } else {
                    var obt = m_FSM.Owner.PushObstacle(point, hp);
                    obt.DoScale();
                }
            }
        }
    
        public State_IDLE(_C.FSMSTATE id) : base(id)
        {
        }

        public override void Enter()
        {
            m_DelayTimer.Reset();

            m_FSM.Owner.m_Score = 0;
            m_FSM.Owner.m_Stage += 1;

            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHSCORE, m_FSM.Owner.m_Score, true));


            //重置弹珠
            //只显示下一颗待发射的弹珠
            for (int i = 0; i < m_FSM.Owner.m_Balls.Count; i++)
            {
                var ball = m_FSM.Owner.m_Balls[i];
                ball.Show(i == 0);
                m_FSM.Owner.BreechBall(ball);
            }


            //回合开始动画
            var obj = GameFacade.Instance.EffectManager.Load(EFFECT.ROUND, new Vector3(0, 2, 0));
            obj.GetComponent<RoundText>().Init(m_FSM.Owner.m_Stage);
        }
        
        public override void Update()
        {   
            if (_GuideUI != null) return;

            m_DelayTimer.Update(Time.deltaTime);
            if (m_DelayTimer.IsFinished() == true) {
                //生成宝石
                DrawObstables();

                if (m_FSM.Owner.Stage == 1 && GameFacade.Instance.DataManager.GetIntByKey(DataManager.KEY_GUIDE) == 0) {
                    _GuideUI = GameFacade.Instance.UIManager.LoadWindow("Prefab/UI/GuideWindow", GameFacade.Instance.UIManager.MAJOR).GetComponent<GuideWindow>();
                } else {
                    m_FSM.Transist(_C.FSMSTATE.GAME_PLAY);
                }
                
            }
        }
    }
















    internal class State_PLAY<T> : State<GameController>
    {
        private List<Ball> m_Queue = new List<Ball>();
        private Ball m_ShootBall = null;
        private List<Ball> m_Orders = new List<Ball>(); //记录弹珠的发射顺序

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
            if (EventSystem.current.IsPointerOverGameObject()) {
                return;
            }

            //弹珠都打完了
            if (m_Queue.Count == 0) {
                return;
            }

            //点击到了取消区域
            if (IsTouchCancel() == true) {
                return;
            }


            m_IsPressDown = true;

            m_FSM.Owner.AimStart();
        }

        void OnMouseUp()
        {
            if (m_IsPressDown != true) return;

            m_IsPressDown = false;
            m_FSM.Owner.m_Simulator.SimulateEnd();
            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_SHOWBUBBLE, false));

            m_FSM.Owner.AimStop();

            //取消区域
            if (IsTouchCancel() == true) {
                return;
            }


            if (m_ShootBall != null) {
                var ball = m_ShootBall;
                m_Queue.Remove(ball);
                GameFacade.Instance.Game.ShootBall(ball, m_FSM.Owner.FingerPos);
                m_Orders.Add(ball);
            }

            //显示下一颗待发射的弹珠
            if (m_Queue.Count > 0) {
                m_ShootBall = m_Queue[0];
                m_ShootBall.Show(true);
            }
        }

        void ReceiveReward()
        {
            //金币跟积分挂钩，最少也给1块钱
            int min = RandomUtility.Random(1, 4);
            int max = RandomUtility.Random(20, 26);
            int coin = Mathf.Clamp((int)Mathf.Floor(m_FSM.Owner.m_Score / 20.0f), min, max);
            GameFacade.Instance.Game.UpdateCoin(coin);

            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHSCORE, 0, false));
            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_FLYCOIN, coin));
        }

        public State_PLAY(_C.FSMSTATE id) : base(id)
        {
        }

        public override void Enter()
        {
            m_IsFinished = false;
            m_DelayTimer.Reset();

            m_Orders.Clear();
            m_Queue.Clear();
            foreach (var ball in m_FSM.Owner.m_Balls) {
                ball.SetState((int)_C.LAYER.BALLREADY);
                m_Queue.Add(ball);
            }

            m_ShootBall = m_Queue[0];

                        //
            m_FSM.Owner.m_Environment.OnBegin(m_FSM.Owner.m_Stage);

            GameFacade.Instance.EventManager.AddHandler(EVENT.UI_SHOWBALLLIST,  OnReponseBallList);
        }

        public override void Update()
        {
            if (m_IsFinished == true) {
                m_DelayTimer.Update(Time.deltaTime);
                if (m_DelayTimer.IsFinished() == true) {
                    if (m_FSM.Owner.m_Stage % 3 == 0) { //每3关
                        m_FSM.Transist(_C.FSMSTATE.GAME_SHOP);
                    }
                    else {
                        m_FSM.Transist(_C.FSMSTATE.GAME_COMPLEX);
                    }
                    
                }
                return;
            }

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
                    m_FSM.Owner.m_Simulator.SimulateEnd();

                    
                    GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_SHOWBUBBLE, true, "\n     <#0CA90D>取消发射</color>"));

                } else {
                    m_FSM.Owner.AimStart();
                    m_FSM.Owner.FocusAim(m_FSM.Owner.FingerPos);

                    m_FSM.Owner.m_Simulator.SimulateShoot(m_FSM.Owner.FingerPos);

                    GameFacade.Instance.Game.ShowBallBubble(m_ShootBall);
                }

                
            }
            
            if (Input.GetMouseButtonUp(0)) // 如果松开了左键（鼠标释放）
            {
                OnMouseUp();
            }

            //场上清空后，不管还有没有剩余弹珠和积分够不够，直接胜利
            if (m_FSM.Owner.m_Ghosts.Count == 0 && m_FSM.Owner.m_Obstacles.Count == 0)
            {
                m_IsFinished = true;
                ReceiveReward();

                GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.ONPLAYEND));
            }

            //全部发射完了，并且场上没有正在运动的弹珠了，则进入下一个阶段
            if (m_Queue.Count == 0 && m_FSM.Owner.IsBallsIdle() == true)
            {
                //判断胜利失败
                //积分结算
                if (m_FSM.Owner.m_Score >= m_FSM.Owner.TargetScore)
                {
                    m_IsFinished = true;
                    ReceiveReward();

                    GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.ONPLAYEND));

                    Camera.main.GetComponent<CameraUtility>().DoShake();

                    //清理障碍物
                    foreach (var obt in m_FSM.Owner.m_Obstacles) {
                        obt.Dead();
                    }

                    //清理Ghost
                    foreach (var ghost in m_FSM.Owner.m_Ghosts) {
                        ghost.Dead();
                    }

                } else {
                    m_FSM.Transist(_C.FSMSTATE.GAME_END);
                }

                
                return;
            }
        }

        public override void Exit()
        {
            m_FSM.Owner.m_Environment.OnEnd(m_FSM.Owner.m_Stage);

            GameFacade.Instance.EventManager.DelHandler(EVENT.UI_SHOWBALLLIST,  OnReponseBallList);


            List<Ball> others = new List<Ball>();
            foreach (var ball in m_FSM.Owner.m_Balls)
            {
                if (m_Orders.Contains(ball) != true)
                {
                    others.Add(ball);
                }
            }

            m_FSM.Owner.m_Balls.Clear();
            m_FSM.Owner.m_Balls.AddRange(m_Orders);
            m_FSM.Owner.m_Balls.AddRange(others);

            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.UI_FLUSHBALLS));
        }


        //负责展示球列表
        void OnReponseBallList(GameEvent gameEvent)
        {
            if (m_Queue.Count == 0) return;


            bool flag = (bool)gameEvent.GetParam(0);


            m_FSM.Owner._GameUI.ShowBallList(flag, m_Queue, (int order)=> {
                if (order >= m_Queue.Count) return;

                var ball = m_Queue[order];

                if (m_ShootBall != null) {
                    m_ShootBall.Show(false);
                }
                m_ShootBall = ball;
                m_ShootBall.Show(true);

                GameFacade.Instance.Game.ShowBallBubble(m_ShootBall);
            });

            if (flag == true) {
                if (m_ShootBall != null) {
                    m_ShootBall.Show(false);
                }
                m_ShootBall = m_Queue[0];
                m_ShootBall.Show(true);

                GameFacade.Instance.Game.ShowBallBubble(m_ShootBall);

                GameFacade.Instance.Game.Pause();
            } else {
                GameFacade.Instance.Game.Resume();
            }
        }
    }

















    internal class State_COMPLEXT<T> : State<GameController>
    {
        private GhostWindow m_GhostUI;

        public State_COMPLEXT(_C.FSMSTATE id) : base(id)
        {
        }

        public override void Enter()
        {
            m_FSM.Owner.m_RefreshTimes = 0;

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

        List<Relics> GenerateRelicses()
        {
            List<Relics> list = new List<Relics>();

            int count = 3;

            Dictionary<object, int> keyValuePairs = new Dictionary<object, int>();
            foreach (RelicsData r in CONFIG.GetRelicsDatas())  {
                if (r.Weight > 0 && m_FSM.Owner.m_Army.GetRelics(r.ID) == null) {
                    keyValuePairs.Add(r, r.Weight);
                }
            }


            for (int i = 0; i < count; i++)
            {
                RelicsData config   = (RelicsData)RandomUtility.PickByWeight(keyValuePairs);
                Relics relics   = new Relics(config);

                keyValuePairs.Remove(config);
                list.Add(relics);
            }

            return list;
        }

        public State_SHOP(_C.FSMSTATE id) : base(id)
        {
        }

        public override void Enter()
        {
            List<Relics> datas = this.GenerateRelicses();

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

        public override void Enter()
        {
            GameFacade.Instance.Game.Pause();

            //记录分数
            GameFacade.Instance.DataManager.SetScore(m_FSM.Owner.m_Stage);

            GameFacade.Instance.SoundManager.StopBGM();

            var obj = GameFacade.Instance.UIManager.LoadWindow("Prefab/UI/ResultWindow", GameFacade.Instance.UIManager.BOARD);
            var ui  = obj.GetComponent<ResultWindow>();
            ui.Init(m_FSM.Owner.m_Stage, m_FSM.Owner.m_Coin);
        }
    }


    #endregion

}