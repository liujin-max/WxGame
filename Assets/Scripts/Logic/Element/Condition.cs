using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//关卡胜利条件
public class Condition
{
    public int ID;
    public Pair Count;


    //目前 Condition的ID和Card的ID一一对应
    public Condition(int id, int count)
    {
        ID      = id;
        Count   = new Pair(count, count);
    }

    //判断是否符合
    public bool IsConformTo(int id)
    {
        if (ID == 10000) return true;

        return ID == id;
    }

    //收集
    public void Collect(int count)
    {
        Count.UpdateCurrent(-count);
    }

    public void SetCount(int count)
    {
        Count.SetCurrent(count);
    }

    public int GetScore()
    {
        return Count.Total - Count.Current;
    }

    public bool IsFinished()
    {
        return Count.IsClear();
    }
}
