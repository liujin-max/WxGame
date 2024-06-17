using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Field : MonoBehaviour
{
    private static Field m_Instance;
    public static Field Instance{get{return m_Instance;}}

    private FSM<Field> m_FSM;

    public GameWindow GameWindow;


    private Stage m_Stage;
    public Stage Stage {get{return m_Stage;}}

    private Land m_Land;
    public Land Land {get{return m_Land;}}

    private int m_Weight = 5;
    private int m_Height = 6;

    public _C.GAME_STATE STATE = _C.GAME_STATE.NONE;
    
    public bool IsMoved = false;

    private Grid[,] m_Grids;
    public Grid[,] Grids {get{ return m_Grids;}}

    private List<Card> m_Cards = new List<Card>();
    public List<Card> Cards { get { return m_Cards;}}

    private List<Card> m_GhostCards = new List<Card>();
    public List<Card> GhostCards { get { return m_GhostCards;}}

    public _C.DIRECTION[] Directions = {_C.DIRECTION.UP, _C.DIRECTION.DOWN, _C.DIRECTION.LEFT, _C.DIRECTION.RIGHT};


    //记录每回合的状态
    private int m_Turn = 0;
    public int Turn {
        get {return m_Turn;}
        set {m_Turn = value;}
    }

    private Dictionary<int, History> m_Historys = new Dictionary<int, History>();

    private CDTimer m_SecondTimer = new CDTimer(1);



    void Awake()
    {
        m_Instance = this;
    }

    void OnDestroy()
    {
        
    }

    void Start()
    {
        m_FSM = new FSM<Field>(this,  new State<Field>[] {
            new State_Idle<Field>(_C.FSMSTATE.IDLE),
            new State_Eliminate<Field>(_C.FSMSTATE.ELIMINATE),
            new State_Chain<Field>(_C.FSMSTATE.CHAIN),
            new State_Check<Field>(_C.FSMSTATE.CHECK),
            new State_Result<Field>(_C.FSMSTATE.RESULT)
        });

        GameWindow = GameFacade.Instance.UIManager.LoadWindow("GameWindow", UIManager.BOTTOM).GetComponent<GameWindow>();

        GameFacade.Instance.UIManager.LoadWindow("GuideWindow", UIManager.GUIDE);
    }

    public void Enter(int stage)
    {
        STATE       = _C.GAME_STATE.PLAY;

        m_Land      = new Land();
        m_Stage     = new Stage(GameFacade.Instance.DataCenter.Level.GetStageJSON(stage));

        m_Weight    = m_Stage.Weight;
        m_Height    = m_Stage.Height;

        EventManager.SendEvent(new GameEvent(EVENT.ONENTERSTAGE, m_Stage));

        InitGrids();

        m_Land.FilterScene();
        m_Stage.FilterGrids();
        m_Stage.InitCards();


        m_FSM.Transist(_C.FSMSTATE.IDLE);
    }

    public void Pause()
    {
        STATE   = _C.GAME_STATE.PAUSE;
    }

    public void Resume()
    {
        STATE   = _C.GAME_STATE.PLAY;
    }

    public void Leave()
    {
        Dispose();
        m_FSM.Dispose();
        m_Instance = null;

        GameFacade.Instance.UIManager.UnloadWindow(GameWindow.gameObject);
    }

    public void Dispose()
    {
        STATE   = _C.GAME_STATE.NONE;
        IsMoved = false;
        m_Turn  = 0;

        m_Stage.Dispose();
        

        m_Historys.Clear();
        m_SecondTimer.Reset();

        for (int i = 0; i < m_Weight; i++) {
            for (int j = 0; j < m_Height; j++) {
                var grid = m_Grids[i, j];
                grid.Dispose();
            }
        }

        m_Cards.ForEach(c => {
            c.Dispose();
        });
        m_Cards.Clear();

        m_GhostCards.ForEach(c => {
            c.Dispose();
        });
        m_GhostCards.Clear();

        m_Land.Dispose();
    }

    public void Transist(_C.FSMSTATE state, params object[] values)
    {
        m_FSM.Transist(state, values);
    }

    public _C.FSMSTATE GetCurrentFSMState()
    {
        return m_FSM.CurrentState.ID;
    }


    void InitGrids()
    {
        m_Grids = new Grid[m_Weight, m_Height];

        int count = 1;
        for (int i = 0; i < m_Weight; i++) {
            for (int j = 0; j < m_Height; j++) {
                var json = m_Stage.GetGridJSON(i, j);
                var grid = new Grid(json, new Vector2((i - ((m_Weight - 1) / 2.0f)) * _C.DEFAULT_GRID_WEIGHT, (j - ((m_Height - 1) / 2.0f)) * _C.DEFAULT_GRID_HEIGHT));
                m_Grids[i, j] = grid;

                count++;
            }
        }
    }

    //放置方块
    public Card PutCard(_C.CARD_STATE state, CardData cardData, Grid grid, bool is_jump = false)
    {
        Card card   = new Card(cardData);

        card.STATE  = state;
        if (state == _C.CARD_STATE.GHOST) {
            card.Grid   = grid;

            m_GhostCards.Add(card);

            GameFacade.Instance.DisplayEngine.Put(DisplayEngine.Track.Common, new DisplayEvent_GhostCard(card));

        } else {
            card.Grid   = grid;
            grid.Card   = card;

            m_Cards.Add(card);

            GameFacade.Instance.DisplayEngine.Put(DisplayEngine.Track.Common, new DisplayEvent_NormalCard(card, is_jump));
        }

        return card;
    }

    public void RemoveCard(Card card)
    {
        // c.Grid = null;       //不置空，否则会影响连锁反应的判断

        //炸弹类方块不占用目标格子，所以不需要处理
        if (!card.IsBomb()) {
            card.Grid.Card = null;
        }
        
        m_Cards.Remove(card);
    }

    //添加虚化方块
    public List<Card> InitGhostCards(int count)
    {
        List<Card> add_cards = new List<Card>();

        //获取空着的Grid
        List<object> grid_datas = RandomUtility.Pick(count, Field.Instance.GetEmptyGrids());

        for (int i = 0; i < grid_datas.Count; i++)
        {
            Grid grid   = grid_datas[i] as Grid;

            int rand    = RandomUtility.Random(0, Field.Instance.Stage.Cards.Count);
            Card card   = Field.Instance.PutCard(_C.CARD_STATE.GHOST, Field.Instance.Stage.Cards[rand], grid);

            add_cards.Add(card);
        }

        return add_cards;
    }

    //将虚化方块实体化
    public void CorporealCards()
    {
        //将虚化方块实体化
        m_GhostCards.ForEach(card => {
            if (card.Grid.IsEmpty == true) {
                card.STATE      = _C.CARD_STATE.NORMAL;
                card.Grid.Card  = card;

                m_Cards.Add(card);

                GameFacade.Instance.DisplayEngine.Put(DisplayEngine.Track.Common, new DisplayEvent_NormalCard(card, true));

            } else {
                Debug.LogError("实体化时，当前坐标已经有方块了：" + card.Grid.X + ", " + card.Grid.Y + " => " + card.Grid.Card.ID);
            }
        });

        m_GhostCards.Clear();
    }

    //场上正常的方块
    public List<Card> GetDragableJellys()
    {
        List<Card> cards = new List<Card>();
        m_Cards.ForEach(c => {
            if (c.Dragable && c.TYPE == _C.CARD_TYPE.JELLY) {
                cards.Add(c);
            }
        });

        return cards;
    }


    //获取空位格子
    public List<object> GetEmptyGrids()
    {
        //获取空着的Grid
        List<object> grid_list = new List<object>(); 
        for (int i = 0; i < Field.Instance.Grids.GetLength(0); i++) {
            for (int j = 0; j < Field.Instance.Grids.GetLength(1); j++) {
                var g = Field.Instance.Grids[i, j];
                if (g.IsEmpty == true && g.IsValid == true) {
                    grid_list.Add(g);
                }  
            }
        }

        return grid_list;
    }

    //获取格子
    public Grid GetGrid(int x, int y)
    {
        if (x >= m_Grids.GetLength(0) || x < 0) return null;
        if (y >= m_Grids.GetLength(1) || y < 0) return null;

        return m_Grids[x, y];
    }

    //获取有效格子
    public Grid GetValidGrid(int x, int y)
    {
        var grid = this.GetGrid(x, y);
        if (grid != null && grid.IsValid == true) {
            return grid;
        }
        return null;
    }

    //获取所有同色方块
    public List<Card> GetCards(int card_id, _C.CARD_STATE state)
    {   
        List<Card> cards= new List<Card>();

        m_Cards.ForEach(card => {
            if (card.ID == card_id && card.STATE == state) {
                cards.Add(card);
            }
        });

        return cards;
    }

    //清理残影
    //如果当前位置有残影，则清空残影
    public void ClearGhost(Card card)
    {
        for (int i = 0; i < m_GhostCards.Count; i++)
        {
            var c = m_GhostCards[i];
            if (c.Grid == card.Grid) {
                m_GhostCards.Remove(c);
                c.Dispose();
                break;
            }
        }
    }

    bool IsGridSame(Grid g1, Grid g2)
    {
        if (g1 == null || g2 == null) return false;

        if (g1.Card != null && g2.Card != null && g1.Card.ID == g2.Card.ID) {
            return true;
        }
        return false;
    }

    bool IsGridHasSameCard(Grid grid, int card_id)
    {
        if (grid == null) return false;

        if (grid.Card != null && grid.Card.ID == card_id) {
            return true;
        }
        return false;
    }

    //返回的是相邻同色方块的数组
    public List<Card> GetSameCardNear(Grid grid, int card_id)
    {
        List<Card> _cards = new List<Card>();

        foreach (_C.DIRECTION dir in Directions)
        {
            var g = this.GetGridByDirection(grid, dir);
            if (IsGridHasSameCard(g, card_id)) 
            {
                _cards.Add(g.Card);
            }
        }

        return _cards;
    }

    //
    public Grid GetGridByDirection(Grid g, _C.DIRECTION direction)
    {
        switch (direction)
        {
            case _C.DIRECTION.UP:
            {
                if (g.Y == m_Height - 1)
                    return null;

                return Field.Instance.Grids[g.X, g.Y + 1];
            }
            

            case _C.DIRECTION.DOWN:
            {
                if (g.Y == 0)
                    return null;

                return Field.Instance.Grids[g.X, g.Y - 1];
            }

            
            case _C.DIRECTION.LEFT:
            {
                if (g.X == 0)
                    return null;

                return Field.Instance.Grids[g.X - 1, g.Y];
            }


            case _C.DIRECTION.RIGHT:
            {
                if (g.X == m_Weight - 1)
                    return null;

                return Field.Instance.Grids[g.X + 1, g.Y];
            }
        }


        return null;
    }
    
    //获取四个方向相邻的方块
    public Card GetCardByDirection(Grid g, _C.DIRECTION direction)
    {
        var grid = this.GetGridByDirection(g, direction);
        if (grid != null) {
            return grid.Card;
        }

        return null;
    }

    //获取两个格子间的距离
    public float GetDistanceByGrids(Grid g1, Grid g2)
    {
        return Vector2.Distance(new Vector2(g1.X, g1.Y), new Vector2(g2.X, g2.Y));
    }

    public bool IsGridNearSide(Grid g)
    {
        if (g.X <= 0 || g.X >= m_Weight - 1) return true;
        if (g.Y <= 0 || g.Y >= m_Height - 1) return true;

        return false;
    }

    #region 移动
    public Grid Move(Card card, _C.DIRECTION direction, bool is_manual = false)
    {
        if (card.IsFixed) return null;

        Grid origin = card.Grid;

        List<Grid> grid_path = new List<Grid>();

        switch (direction) 
        {
            case _C.DIRECTION.LEFT:     //向左
            {
                if (origin.X == 0) return null;

                int pos_x   = origin.X - 1;
                int pos_y   = origin.Y;

                while (pos_x >= 0 && pos_x < m_Weight && pos_y >= 0 && pos_y < m_Height)
                {
                    Grid grid = this.GetGrid(pos_x, pos_y);;

                    if (!grid.IsValid) break;
                    if (grid_path.Contains(grid)) break;

                    if (grid.IsEmpty)
                    {
                        grid_path.Add(grid);
                        pos_x--;

                        if (grid.IsBan) {
                            break;
                        }
                    }
                    else 
                    {
                        if (grid.IsPortalCanCross(card, direction) == true)    //传送门
                        {
                            grid_path.Add(grid);
                            grid_path.Add(grid.Portal);

                            pos_x = grid.Portal.X - 1;
                            pos_y = grid.Portal.Y;
                        }
                        else if (card.IsBomb() == true)
                        {
                            if (grid.Portal != null) break;

                            grid_path.Add(grid);
                            pos_x--;
                            if (card.ID == (int)_C.CARD.BOMB) {
                                break;
                            }
                        }

                        else
                        {
                            break;
                        }
                    }
                }

                if (grid_path.Count == 0)  return null;
                card.CrossGrids = grid_path;

                Grid target = grid_path.Last();
                card.Grid   = target;
                origin.Card = null;
                if (!card.IsBomb()) {    //炸弹类方块不占用格子
                    target.Card = card;
                } 

                // ClearGhost(card);
                if (is_manual == true) m_Stage.UpdateMoveStep(-1);

                GameFacade.Instance.DisplayEngine.Put(DisplayEngine.Track.Common, new DisplayEvent_MoveCard(card, _C.DIRECTION.LEFT, grid_path, is_manual));

                return target;
            }

            case _C.DIRECTION.RIGHT:    //向右
            {
                if (grid_path.Count > 20) break;

                if (origin.X == m_Weight - 1) return null;

                int pos_x   = origin.X + 1;
                int pos_y   = origin.Y;

                while (pos_x >= 0 && pos_x < m_Weight && pos_y >= 0 && pos_y < m_Height)
                {
                    Grid grid = this.GetGrid(pos_x, pos_y);;

                    if (!grid.IsValid) break;
                    if (grid_path.Contains(grid)) break;

                    if (grid.IsEmpty)
                    {
                        grid_path.Add(grid);
                        pos_x++;

                        if (grid.IsBan) {
                            break;
                        }
                    }
                    else 
                    {
                        if (grid.IsPortalCanCross(card, direction) == true)    //传送门
                        {
                            grid_path.Add(grid);
                            grid_path.Add(grid.Portal);

                            pos_x = grid.Portal.X + 1;
                            pos_y = grid.Portal.Y;
                        }
                        else if (card.IsBomb() == true)
                        {
                            if (grid.Portal != null) break;

                            grid_path.Add(grid);
                            pos_x++;
                            if (card.ID == (int)_C.CARD.BOMB) {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (grid_path.Count == 0)  return null;
                card.CrossGrids = grid_path;

                Grid target = grid_path.Last();
                card.Grid   = target;
                origin.Card = null;
                if (!card.IsBomb()) {    //炸弹类方块不占用格子
                    target.Card = card;
                } 

                // ClearGhost(card);
                if (is_manual == true) m_Stage.UpdateMoveStep(-1);

                GameFacade.Instance.DisplayEngine.Put(DisplayEngine.Track.Common, new DisplayEvent_MoveCard(card, _C.DIRECTION.RIGHT, grid_path, is_manual));

                return target;
            }

            case _C.DIRECTION.UP:       //向上
            {
                if (origin.Y == m_Height - 1) return null;

                int pos_x   = origin.X;
                int pos_y   = origin.Y + 1;

                while (pos_x >= 0 && pos_x < m_Weight && pos_y >= 0 && pos_y < m_Height)
                {
                    Grid grid = this.GetGrid(pos_x, pos_y);

                    if (!grid.IsValid) break;
                    if (grid_path.Contains(grid)) break;

                    if (grid.IsEmpty)
                    {
                        grid_path.Add(grid);
                        pos_y++;

                        if (grid.IsBan) {
                            break;
                        }
                    }
                    else 
                    {
                        if (grid.IsPortalCanCross(card, direction) == true)    //传送门
                        {
                            grid_path.Add(grid);
                            grid_path.Add(grid.Portal);

                            pos_x = grid.Portal.X;
                            pos_y = grid.Portal.Y + 1;
                        }
                        else if (card.IsBomb() == true)
                        {
                            if (grid.Portal != null) break;

                            grid_path.Add(grid);
                            pos_y++;
                            if (card.ID == (int)_C.CARD.BOMB) {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (grid_path.Count == 0)  return null;
                card.CrossGrids = grid_path;

                Grid target = grid_path.Last();
                card.Grid   = target;
                origin.Card = null;
                if (!card.IsBomb()) {    //炸弹类方块不占用格子
                    target.Card = card;
                } 
                
                

                // ClearGhost(card);
                if (is_manual == true) m_Stage.UpdateMoveStep(-1);

                GameFacade.Instance.DisplayEngine.Put(DisplayEngine.Track.Common, new DisplayEvent_MoveCard(card, _C.DIRECTION.UP, grid_path, is_manual));

                return target;
            }
            
            case _C.DIRECTION.DOWN:     //向下
            {
                if (origin.Y == 0) return null;


                int pos_x   = origin.X;
                int pos_y   = origin.Y - 1;

                while (pos_x >= 0 && pos_x < m_Weight && pos_y >= 0 && pos_y < m_Height)
                {
                    Grid grid = this.GetGrid(pos_x, pos_y);;

                    if (!grid.IsValid) break;
                    if (grid_path.Contains(grid)) break;

                    if (grid.IsEmpty)
                    {
                        grid_path.Add(grid);
                        pos_y--;

                        if (grid.IsBan) {
                            break;
                        }
                    }
                    else 
                    {
                        if (grid.IsPortalCanCross(card, direction) == true)    //传送门
                        {
                            grid_path.Add(grid);
                            grid_path.Add(grid.Portal);

                            pos_x = grid.Portal.X;
                            pos_y = grid.Portal.Y - 1;
                        }
                        else if (card.IsBomb() == true)
                        {
                            if (grid.Portal != null) break;

                            grid_path.Add(grid);
                            pos_y--;
                            if (card.ID == (int)_C.CARD.BOMB) {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (grid_path.Count == 0)  return null;
                card.CrossGrids = grid_path;

                Grid target = grid_path.Last();
                card.Grid   = target;
                origin.Card = null;
                if (!card.IsBomb()) {    //炸弹类方块不占用格子
                    target.Card = card;
                } 

                // ClearGhost(card);
                if (is_manual == true) m_Stage.UpdateMoveStep(-1);

                GameFacade.Instance.DisplayEngine.Put(DisplayEngine.Track.Common, new DisplayEvent_MoveCard(card, _C.DIRECTION.DOWN, grid_path, is_manual));

                return target;
            }
        }

        return null;
    }
    #endregion

    //计算消除
    //相邻的
    public List<Card> CheckEliminate()
    {
        List<Card> _Removes = new List<Card>();

        m_Cards.ForEach(card => {
            if (card.TYPE == _C.CARD_TYPE.JELLY) 
            {   
                List<Card> nears = this.GetSameCardNear(card.Grid, card.ID);
                if (nears.Count > 0) {
                    card.IsReady2Eliminate = true;  
                }
            }

            if (card.IsReady2Eliminate)
            {
                _Removes.Add(card); 
            }

        });

        CheckLink(_Removes);

        return _Removes;
    }

    //某关开放
    //计算消除是否生成飞弹、炸弹
    void CheckLink(List<Card> remove_cards)
    {
        if (m_Stage.ID < _C.BOMB_UNLOCK_LEVEL) return;


        for (int i = remove_cards.Count - 1; i >= 0; i--)
        {
            Card card   = remove_cards[i];

            if (card.IsBomb() || card.TYPE == _C.CARD_TYPE.FRAME) continue;

            List<Card> links = this.GenerateLinkCards(card);
            if (links.Count > 1)
            {
                int near_count  = 1;
                Card cs_card    = null;
                for (int j = 0; j < links.Count; j++)
                {
                    var c = links[j];
                    int count = this.GetSameCardNear(c.Grid, c.ID).Count;
                    if (count > near_count)
                    {
                        near_count = count;
                        cs_card = c;
                    }
                }

                if (cs_card != null)
                {
                    //消除3个同色方块，产生一枚飞弹
                    //消除4个同色方块，产生一枚炸弹
                    if (links.Count == 3) {
                        cs_card.DerivedID = (int)_C.CARD.MISSILE;
                    }

                    if (links.Count > 3) {
                        cs_card.DerivedID = (int)_C.CARD.BOMB;
                    }
                }
            }
        }
    }

    //获取当前方块所处的Link
    List<Card> GenerateLinkCards(Card card)
    {
        List<Card> link_cards = new List<Card>();

        if (card.Link != null) {
            return link_cards;
        }

        card.Link = card;
        link_cards.Add(card);

        var near_cards = this.GetSameCardNear(card.Grid, card.ID);

        while (near_cards.Count > 0) {
            var c = near_cards[0];
            //未访问过 且 能被消除的(并不一定相邻就一定会消除，石块木块这类特殊方块)
            if (c.Link == null && c.IsReady2Eliminate == true) {
                c.Link = card;
                link_cards.Add(c);
                near_cards.AddRange(this.GetSameCardNear(c.Grid, c.ID));
            }
            near_cards.Remove(c);
        }


        return link_cards;
    }

    //死局
    //无路可走
    bool IsImpasse()
    {
        var grids = this.GetEmptyGrids();

        if (grids.Count == 0) return true;

        //无路可走了
        if (grids.Count == 1 && m_GhostCards.Count > 0) {
            Grid g = grids[0] as Grid;

            HashSet<int> _records = new HashSet<int>();

            for (int i = 0; i < Directions.Length; i++)
            {
                _C.DIRECTION dir = Directions[i];
                var grid = this.GetGridByDirection(g, dir);
                if (grid != null) {
                    if (_records.Contains(grid.Card.ID)) {
                        return false;
                    }
                    _records.Add(grid.Card.ID);
                }
            }

            return true;
        }

        return false;
    }

    public _C.RESULT CheckResult()
    {
        if (m_Stage.IsFinished() == true) return _C.RESULT.VICTORY;

        //检查步数
        if (m_Stage.NeedCheckStep()) {
            if (m_Stage.IsStepClear() == true) {
                return _C.RESULT.LOSE;
            }
        }

        //检查时间
        if (m_Stage.NeedCheckTimer()) {
            if (m_Stage.IsTimerClear() == true) return _C.RESULT.LOSE;
        }

        //无路可走了
        if (this.IsImpasse() == true) {
            return _C.RESULT.LOSE;
        }


        return _C.RESULT.NONE;
    }

    public void RecordHistory()
    {
        History history     = new History(m_Turn);
        m_Historys[m_Turn]  = history;

        history.Record();
    }

    void Update()
    {
        if (this.STATE != _C.GAME_STATE.PLAY) return;

        if (m_FSM != null) m_FSM.Update();

        //倒计时
        if (m_Stage != null && m_Stage.NeedCheckTimer()) {
            m_SecondTimer.Update(Time.deltaTime);
            if (m_SecondTimer.IsFinished() == true) {
                m_SecondTimer.Reset();

                m_Stage.UpdateCountDown(-1);

                EventManager.SendEvent(new GameEvent(EVENT.UI_UPDATETIME, false));
            }
        }
    }




    #region 广告逻辑
    //添加时间
    public void ad_add_time(int second)
    {
        if (!m_Stage.NeedCheckTimer()) return;

        m_Stage.UpdateCountDown(second);

        EventManager.SendEvent(new GameEvent(EVENT.UI_UPDATETIME, true));
    }

    //添加步数
    public void ad_add_step(int value)
    {
        if (!m_Stage.NeedCheckStep()) return;

        m_Stage.UpdateMoveStep(value);

        EventManager.SendEvent(new GameEvent(EVENT.UI_UPDATESTEP, true));
    }

    //打乱方块
    //为场上的方块重新指定位置
    public void ad_shuffle()
    {
        int ghost_count = m_GhostCards.Count;

        m_GhostCards.ForEach(c => {
            c.Dispose();
        });
        m_GhostCards.Clear();

        List<Card> shuffle_cards = new List<Card>();

        m_Cards.ForEach(card => {
            if (card.TYPE == _C.CARD_TYPE.JELLY) {
                card.Grid.Card = null;
                shuffle_cards.Add(card);
            }
        });

        List<object> grid_datas = RandomUtility.Pick(m_Cards.Count, Field.Instance.GetEmptyGrids());

        shuffle_cards.ForEach(card => {
            Grid grid = grid_datas[RandomUtility.Random(0, grid_datas.Count)] as Grid;
            card.Grid = grid;
            grid.Card = card;

            grid_datas.Remove(grid);
        });

        //添加虚化方块
        Field.Instance.InitGhostCards(ghost_count);


        GameFacade.Instance.DisplayEngine.Put(DisplayEngine.Track.Common, new DisplayEvent_ShuffleCard(m_Cards));
    }

    //撤销
    //撤销上一步操作
    public bool can_revoke()
    {
        return m_Historys.ContainsKey(m_Turn - 1);
    }

    public void ad_revoke()
    {
        History history;
        if (m_Historys.TryGetValue(m_Turn - 1, out history)) {
            history.Revoke();
        }
    }


    #endregion
}
