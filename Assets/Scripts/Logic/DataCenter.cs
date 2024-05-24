using System;
using System.Collections.Generic;



//数据体
public class GoodsData
{
    public int ID;
    public string Name;
    public int[] Prices;
    public int[] Weights;

    
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
            config.Increase     = data[4];
            config.Reduction    = data[5];

            string[] price_info = data[2].Split('|');
            config.Prices       = new int[price_info.Length];
            for (int i = 0; i < price_info.Length; i++) {
                config.Prices[i] = Convert.ToInt32(price_info[i]);
            }


            string[] weight_info = data[3].Split('|');
            config.Weights      = new int[weight_info.Length];
            for (int i = 0; i < weight_info.Length; i++) {
                config.Weights[i] = Convert.ToInt32(weight_info[i]);
            }



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

