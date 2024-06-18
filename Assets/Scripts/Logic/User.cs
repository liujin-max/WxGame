using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;




//基础数据
[System.Serializable]
public class BaseData
{
    public string openId;
}

//账号数据
[System.Serializable]
public class GameUserData
{
    public string Name = "未知";
    public string HeadUrl;
    public int Level;       //通关记录
    public int Coin;        //金币
    public int Food = _C.DEFAULT_FOOD;  //体力
    public long RecoveryTimestamp;    //上次体力的恢复时间
    public int Score;       //无尽分数
    public List<int> Tasks = new List<int>();
}




//负责管理账号的各种数据
public class User
{
    //用户数据
    [SerializeField] private BaseData m_Base = new BaseData();
    private GameUserData m_Data = new GameUserData();
    public GameUserData Data {
        set { m_Data = value; }
    }

    public string OpenID { get{ return m_Base.openId;}}
    public string Name { get{ return m_Data.Name;}}
    public string HeadURL { get{ return m_Data.HeadUrl;}}
    public int Level { get{ return m_Data.Level;}}
    public int Coin { get{ return m_Data.Coin;}}
    public int Food { get{ return m_Data.Food;}}
    public int Score { get{ return m_Data.Score;}}
    public List<int> Tasks { get{ return m_Data.Tasks;}}

    private bool m_userUpdate = false;  //账号数据变动标记
    public bool IsDirty {get { return m_userUpdate;}}

    
    //体力恢复计时器
    private CDTimer m_FoodTimer = new CDTimer(_C.FOOD_RECOVERYTIME);
    private float m_FoodTimeStep= 0;
    
    //从本地存档里同步数据
    public void Sync()
    {
        //同步账号基础数据(头像、名字)
        Platform.Instance.LOGIN(()=>{
            //同步账号游玩数据(记录、成就等)
            Platform.Instance.SYNC();

        
        });
    }

    public void SyncRecords()
    {
        //同步任务信息
        Tasks.ForEach(id => {
            var t = GameFacade.Instance.DataCenter.Daily.GetTask(id);
            if (t != null) {
                t.Receive();
            }
        });


        //任务：每日登录
        GameFacade.Instance.DataCenter.Daily.FinishTask((int)_C.TASK.LOGIN);

        //每次启动后，根据当前时间和上一次记录的恢复时间，计算恢复了多少体力，做体力恢复处理，并记录恢复时间
        long last_stamp     = m_Data.RecoveryTimestamp;
        long current_stamp  = ToolUtility.GetUnixTimestamp();
        if (last_stamp > 0)
        {
            int offset = Convert.ToInt32(current_stamp - last_stamp);

            int food = offset / _C.FOOD_RECOVERYTIME;

            int time = offset % _C.FOOD_RECOVERYTIME;
            m_FoodTimer.SetCurrent(time);

            this.UpdateFood(food);
            this.SetRecoveryTimestamp(current_stamp);
            Debug.Log("恢复体力：" + food + ", 剩余时间：" + time);
        }
        else this.SetRecoveryTimestamp(current_stamp);
    }


    public void Upload()
    {
        Platform.Instance.UPLOAD(m_Data);

        //重置标记
        m_userUpdate    = false;
    }

    public void SetLevel(int value)
    {
        if (value <= m_Data.Level ) return;

        m_Data.Level = value;

        m_userUpdate = true;
    }

    public void SetCoin(int value)
    {
        m_Data.Coin = value;

        m_userUpdate = true;
    }

    public void UpdateCoin(int value)
    {
        m_Data.Coin  = Mathf.Max(0, m_Data.Coin + value);

        m_userUpdate = true;
    }

    public void SetFood(int value)
    {
        m_Data.Food = value;

        m_userUpdate = true;
    }

    public void UpdateFood(int value)
    {
        m_Data.Food  = Mathf.Clamp(m_Data.Food + value, 0, _C.DEFAULT_FOOD);
        
        m_userUpdate = true;
    }

    public void SetRecoveryTimestamp(long value)
    {
        m_Data.RecoveryTimestamp = value;

        m_userUpdate = true;
    }

    public void SetScore(int value)
    {
        if (value <= m_Data.Score ) return;

        m_Data.Score = value;

        m_userUpdate = true;
    }

    public int GetFoodTimer()
    {
        return Mathf.CeilToInt(m_FoodTimer.Duration - m_FoodTimer.Current);
    }


    public void Update(float dt)
    {   
        float realtime  = Time.realtimeSinceStartup;
        float offset    = realtime - m_FoodTimeStep;
        m_FoodTimeStep  = realtime;

        //体力恢复线程
        m_FoodTimer.Update(offset);
        if (m_FoodTimer.IsFinished()) {
            m_FoodTimer.Reset();

            this.UpdateFood(1);
            this.SetRecoveryTimestamp(ToolUtility.GetUnixTimestamp());
            Debug.Log("恢复体力：" + ToolUtility.GetUnixTimestamp());

            EventManager.SendEvent(new GameEvent(EVENT.UI_UPDATEFOOD));
        }

        if (IsDirty) {
            Upload();
        }
    }
}


