using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

//

//弹珠数据体
public struct BallData
{
    public _C.BALLTYPE Type;
    public int Cost;
    public string Ball;
    public int Weight;

    public string Name;
    public string Icon;
}

//遗物数据体
public struct RelicsData
{
    public int ID;
    public string Name;
    public string Effect;
    public string Icon;
    public int Weight;
    public int Price;
}

//成就数据体
public struct AchievementData
{
    public int ID;
    public string Name;
    public string Description;
    public int Coin;
    public int Glass;
}


//数据中心，读取CSV的数据并保存在此处
public static class CONFIG
{
    //弹珠数据
    private static List<BallData> BallDatas = new List<BallData>();
    private static Dictionary<_C.BALLTYPE, BallData> BallDataDic = new Dictionary<_C.BALLTYPE, BallData>();

    //道具数据
    private static List<RelicsData> RelicsDatas = new List<RelicsData>();
    private static Dictionary<int, RelicsData> RelicsDataDic = new Dictionary<int, RelicsData>();

    //成就数据
    private static List<AchievementData> AchievementDatas = new List<AchievementData>();
    private static Dictionary<int, AchievementData> AchievementDataDic = new Dictionary<int, AchievementData>();

    public static void InitDatas()
    {
        //弹珠数据
        List<string[]> list = GameFacade.Instance.CsvManager.GetStringArrays(CsvManager.TableKey_Ball);
        foreach (string[] data in list)
        {
            BallData config = CreateBallData(data);

            BallDatas.Add(config);
            BallDataDic.Add(config.Type, config);
        }

        //遗物数据
        List<string[]> list_relics = GameFacade.Instance.CsvManager.GetStringArrays(CsvManager.TableKey_Relics);
        foreach (string[] data in list_relics)
        {
            RelicsData config = new RelicsData();
            config.ID       = Convert.ToInt16(data[0]);
            config.Name     = data[1].ToString();
            config.Weight   = Convert.ToInt16(data[2]);
            config.Effect   = data[3].ToString();
            config.Price    = Convert.ToInt16(data[5]);

            // Debug.Log(config.Name + ": " + config.Price);
            
            RelicsDatas.Add(config);
            RelicsDataDic.Add(config.ID, config);
        }

        //成就数据
        List<string[]> list_achievements = GameFacade.Instance.CsvManager.GetStringArrays(CsvManager.TableKey_Achievement);
        foreach (string[] data in list_achievements)
        {
            AchievementData config = new AchievementData();
            config.ID           = Convert.ToInt16(data[0]);
            config.Name         = data[1].ToString();
            config.Coin         = Convert.ToInt16(data[2]);
            config.Glass        = Convert.ToInt16(data[3]);
            config.Description  = data[4].ToString();
            
            AchievementDatas.Add(config);
            AchievementDataDic.Add(config.ID, config);
        }
    }

    public static BallData CreateBallData(string[] data)
    {
        BallData config = new BallData();
        config.Type     = (_C.BALLTYPE)Convert.ToInt16(data[0]);
        config.Name     = data[1].ToString();
        config.Cost     = Convert.ToInt16(data[2]);
        config.Weight   = Convert.ToInt16(data[3]);
        config.Icon     = data[4].ToString();
        config.Ball     = data[5].ToString();

        return config;
    }

    public static List<BallData> GetBallDatas()
    {
        return BallDatas;
    }

    public static BallData GetBallData(_C.BALLTYPE type)
    {
        BallData data;
        if (BallDataDic.TryGetValue(type, out data)) {
            return data;
        }

        return data;
    }

    public static List<RelicsData> GetRelicsDatas()
    {
        return RelicsDatas;
    }

    public static RelicsData GetRelicsData(int id)
    {
        RelicsData data;
        if (RelicsDataDic.TryGetValue(id, out data)) {
            return data;
        }

        return data;
    }

    public static List<AchievementData> GetAchievementDatas()
    {
        return AchievementDatas;
    }

    public static AchievementData GetAchievementData(int id)
    {
        AchievementData data;
        if (AchievementDataDic.TryGetValue(id, out data)) {
            return data;
        }

        return data;
    }
}
