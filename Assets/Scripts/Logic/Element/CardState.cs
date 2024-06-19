using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//负责存储方块的各种状态
public class CardState
{
    
    public _C.DEAD_TYPE DeadType = _C.DEAD_TYPE.NORMAL;

    //准备分解
    public bool IsReady2Eliminate = false;
    //死亡分解中
    public bool IsEliminating = false;
    //衍生物ID
    public int DerivedID = -1;
    //消除所处的方块link
    public Card Link;
    //当前消除属于第几次Combo
    public int Combo;

    //传染标记
    //传染物爆炸时会把相邻的方块变成传染物的颜色
    public bool InfectionFlag = false;


    public CardState(Card card)
    {
        
    }

    public void Copy(CardState card_state)
    {
        DerivedID       = card_state.DerivedID;
        InfectionFlag   = card_state.InfectionFlag;
        
    }
}
