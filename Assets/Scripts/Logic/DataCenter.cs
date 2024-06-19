using System;
using System.Collections.Generic;
using UnityEngine;




//数据体
public class CardData
{
    public int ID;
    public string Name;
    public _C.CARD_TYPE Type;
    public bool Breakable;
}



//从本地Json文件读取
[System.Serializable]
public class StageJSON
{
    public int ID;
    public int Weight;
    public int Height;
    public int Step;
    public int Time;
    public int Coin;
    public int Food;
    public List<string> Conditions;
    public List<int> CardPool;
    public List<GridJSON> Grids;
}

//从本地Json文件读取
[System.Serializable]
public class GridJSON
{
    public int Order;
    public int X;
    public int Y;

    public bool IsValid = true;
    public int JellyID;
    public bool InfectionFlag = false;
    public Vector2 Portal;

    public bool IsBan;
    public _C.DIRECTION AutoDirection = _C.DIRECTION.NONE;
}



//全局数据类
public class DataCenter
{
    //方块信息
    private Dictionary<int, CardData> m_CardDic = new Dictionary<int, CardData>();
    private List<CardData> m_Cards = new List<CardData>();



    //账号信息
    public User User;
    //章节信息
    public Levels Level;
    //任务信息
    public Daily Daily;

    public void Init()
    {
        //账号数据
        User = new User();

        //章节数据
        Level = new Levels();

        //任务数据
        Daily = new Daily();
        Daily.Init();



        //方块数据
        {
            List<string[]> list = GameFacade.Instance.CsvManager.GetStringArrays(CsvManager.TableKey_Card);
            foreach (string[] data in list) {
                CardData config = new CardData();
                config.ID       = Convert.ToInt32(data[0]);
                config.Name     = data[1];
                config.Type     = (_C.CARD_TYPE)Convert.ToInt32(data[2]);
                config.Breakable= Convert.ToInt32(data[3]) == 1;

                m_Cards.Add(config);
                m_CardDic[config.ID]  = config;
            }
        }
    }

    public List<CardData> GetCards()
    {
        return m_Cards;
    }

    public CardData GetCardData(int id)
    {
        CardData data;
        if (m_CardDic.TryGetValue(id, out data)) {
            return data;
        }

        return data;
    }


    public void Update(float dt)
    {
        if (User != null)
        {
            User.Update(dt);
        }
    }
}

