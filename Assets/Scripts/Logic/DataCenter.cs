using System;
using System.Collections.Generic;



//数据体
public class CardData
{
    public int ID;
    public string Name;
    public _C.CARD_TYPE Type;
}

//数据体
public class StageData
{
    public int ID;
    public string Name;
    public int Weight;
    public int Height;
    public int Step;
    public string Condition;
    public int Coin;
    public string Cards;
}



//全局数据类
public class DataCenter
{
    //方块信息
    private Dictionary<int, CardData> m_CardDic = new Dictionary<int, CardData>();
    private List<CardData> m_Cards = new List<CardData>();

    //章节信息
    public Levels Level;

    public void Init()
    {
        //章节数据
        Level = new Levels();
        Level.Init();

        //果冻数据
        List<string[]> list = GameFacade.Instance.CsvManager.GetStringArrays(CsvManager.TableKey_Card);
        foreach (string[] data in list) {
            CardData config = new CardData();
            config.ID       = Convert.ToInt32(data[0]);
            config.Name     = data[1];
            config.Type     = (_C.CARD_TYPE)Convert.ToInt32(data[2]);

            m_Cards.Add(config);
            m_CardDic[config.ID]  = config;
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



}

