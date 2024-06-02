using System;
using System.Collections.Generic;



//数据体
public class CardData
{
    public int ID;
    public string Name;

}

//全局数据类
public class DataCenter
{
    private static Dictionary<int, CardData> m_CardDic = new Dictionary<int, CardData>();
    private static List<CardData> m_Cards = new List<CardData>();

    public void Init()
    {
        //弹珠数据
        List<string[]> list = GameFacade.Instance.CsvManager.GetStringArrays(CsvManager.TableKey_Card);
        foreach (string[] data in list) {
            CardData config = new CardData();
            config.ID       = Convert.ToInt32(data[0]);
            config.Name     = data[1];

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

