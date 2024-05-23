using System;
using System.Collections.Generic;



//数据体
public class GoodsData
{
    public int ID;
    public string Name;
    public int Price_Min;
    public int Price_Normal;
    public int Price_Max;
    
    //描述
    public string Increase;
    public string Reduction;
}

//全局数据类
public class DataCenter
{
    private static List<GoodsData> GoodsDatas = new List<GoodsData>();
    private static Dictionary<int, GoodsData> GoodsDic = new Dictionary<int, GoodsData>();

    public void Init()
    {
        //弹珠数据
        List<string[]> list = GameFacade.Instance.CsvManager.GetStringArrays(CsvManager.TableKey_Goods);
        foreach (string[] data in list)
        {
            GoodsData config    = new GoodsData();
            config.ID           = Convert.ToInt32(data[0]);
            config.Name         = data[1];
            config.Increase     = data[3];
            config.Reduction    = data[4];

            string[] price_info = data[2].Split('|');
            config.Price_Min    = Convert.ToInt32(price_info[0]);
            config.Price_Normal = Convert.ToInt32(price_info[1]);
            config.Price_Max    = Convert.ToInt32(price_info[2]);



            if (!GoodsDic.ContainsKey(config.ID)) {
                GoodsDic.Add(config.ID, config);
            }
            GoodsDatas.Add(config);
        }
    }

    public List<GoodsData> GetGoodsDatas()
    {
        return GoodsDatas;
    }

    public GoodsData GetGoods(int id)
    {
        GoodsData data;
        if (GoodsDic.TryGetValue(id, out data)) {
            return data;
        }

        return data;
    }


}

