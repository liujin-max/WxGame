using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//日常任务管理
public class Daily
{
    //任务信息
    private Dictionary<int, Task> m_TaskDic = new Dictionary<int, Task>();
    private List<Task> m_Tasks = new List<Task>();



    public void Init()
    {
        Parse();
    }

    void Parse()
    {
        //任务数据
        List<string[]> list = GameFacade.Instance.CsvManager.GetStringArrays(CsvManager.TableKey_Task);
        foreach (string[] configs in list) {
            Task t = new Task(configs);

            m_Tasks.Add(t);
            m_TaskDic[t.ID]  = t;
        }
    }

    public List<Task> GetTasks()
    {
        return m_Tasks;
    }

    public Task GetTask(int id)
    {
        Task t;
        if (m_TaskDic.TryGetValue(id, out t)) {
            return t;
        }
        return null;
    }

    public bool HasFinishedTask()
    {
        for (int i = 0; i < m_Tasks.Count; i++) {
            var t = m_Tasks[i];
            if (t.STATE == _C.TASK_STATE.FINISH) {
                return true;
            }
        }
        
        return false;
    }

    //完成任务
    public void FinishTask(int task_id)
    {
        //已经完成了
        if (GameFacade.Instance.DataCenter.User.Tasks.Contains(task_id) == true) return;

        var task = this.GetTask(task_id);
        task.Finish();

        EventManager.SendEvent(new GameEvent(EVENT.UI_UPDATETASK, task));
    }

    //领取任务奖励
    public void ReceiveTask(Task task)
    {
        //已经完成了
        if (GameFacade.Instance.DataCenter.User.Tasks.Contains(task.ID) == true) return;

        task.Receive();

        GameFacade.Instance.DataCenter.User.RecordsTask(task);
        GameFacade.Instance.DataCenter.User.UpdateCoin(task.Coin);

        EventManager.SendEvent(new GameEvent(EVENT.UI_UPDATECOIN));
        EventManager.SendEvent(new GameEvent(EVENT.UI_UPDATETASK, task));
    }
}
