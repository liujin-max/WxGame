using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//一步走完后的处理
public class State_Check<T> : State<Field>
{
    public State_Check(_C.FSMSTATE id) : base(id){}

    public override void Enter(params object[] values)
    {
        Field.Instance.IsMoved = false;

        Field.Instance.Stage.AddCards(); 

        //传送带逻辑
        foreach (var card in Field.Instance.Cards)
        {
            var grid = card.Grid;
            if (grid.AutoDirection == _C.DIRECTION.NONE) continue;

            var target_grid = Field.Instance.GetGridByDirection(grid, grid.AutoDirection);
            if (target_grid == null) continue;

            card.Grid   = target_grid;
            target_grid.Card = card;

            if (grid.Card ==  card) grid.Card = null;

            GameFacade.Instance.DisplayEngine.Put(DisplayEngine.Track.Common, new DisplayEvent_BeltCard(card));
        }
    }

    public override void Update()
    {
        if (!GameFacade.Instance.DisplayEngine.IsIdle()) return;

        //生成后先检测一遍
        Field.Instance.Transist(_C.FSMSTATE.ELIMINATE);
    }
}

