using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//一步走完后的处理
public class State_Result<T> : State<Field>
{
    public State_Result(_C.FSMSTATE id) : base(id){}

    public override void Enter(params object[] values)
    {
        var result = (_C.RESULT)values[0];

        if (result == _C.RESULT.VICTORY) {  //成功
            GameFacade.Instance.UIManager.LoadWindow("VictoryWindow", UIManager.BOARD).GetComponent<VictoryWindow>();
        } else {    //失败
            Debug.Log("失败");
        }
        
    }
}

