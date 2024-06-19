using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//一步走完后的处理
public class State_Check<T> : State<Field>
{
    public State_Check(_C.FSMSTATE id) : base(id){}

    public override void Enter(params object[] values)
    {
        Field.Instance.IsMoved  = false;

        Field.Instance.Stage.AddCards(); 

        DoBeltLogic();
    }

    public override void Update()
    {
        if (!GameFacade.Instance.DisplayEngine.IsIdle()) return;

        //生成后先检测一遍
        Field.Instance.Transist(_C.FSMSTATE.ELIMINATE);
    }

    //传送带逻辑
    void DoBeltLogic()
    {
        foreach (var card in Field.Instance.Cards)
        {
            var grid = card.Grid;
            if (grid.BeltDirection == _C.DIRECTION.NONE) continue;
            if (grid.Card != null && grid.Card.IsInValidByBelt == true) continue;   //无法位移传送门

            List<Grid> grid_path = grid.GetBelt2Grid();
            if (grid_path.Count == 0) continue;

            if (grid.Card == card) grid.Card = null;

            var to_grid = grid_path.Last();
            card.Grid   = to_grid;
            to_grid.Card= card;

            GameFacade.Instance.DisplayEngine.Put(DisplayEngine.Track.Common, new DisplayEvent_BeltCard(card, grid_path));
        }
    }
}

