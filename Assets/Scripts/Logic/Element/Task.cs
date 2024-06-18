using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//任务
public class Task
{
    public int ID;
    public string Name;
    public string Description;
    public int Coin;


    public _C.TASK_STATE STATE = _C.TASK_STATE.NONE;

    public Task(string[] configs)
    {
        ID      = Convert.ToInt32(configs[0]);
        Name    = configs[1];
        Description = configs[2];
        Coin    = Convert.ToInt32(configs[3]);
    }

    public void Finish()
    {
        STATE = _C.TASK_STATE.FINISH;
    }

    public void Receive()
    {
        STATE = _C.TASK_STATE.RECEIVED;
    }
}
