using System;
using System.Collections.Generic;



//数据体
public class AnimalData
{
    public int ID;
    public string Name;
    public int Belong;
    public int Value;

    public _C.ANIMAL Type;
}

//全局数据类
public class DataCenter
{
    //动物数据
    //根据Belong分类
    private static Dictionary<int, List<AnimalData>> AnimalBelongDic = new Dictionary<int, List<AnimalData>>();
    //根据Type分类
    private static Dictionary<_C.ANIMAL, List<AnimalData>> AnimalTypeDic = new Dictionary<_C.ANIMAL, List<AnimalData>>();

    public void Init()
    {
        //弹珠数据
        List<string[]> list = GameFacade.Instance.CsvManager.GetStringArrays(CsvManager.TableKey_Animal);
        foreach (string[] data in list)
        {
            AnimalData config = new AnimalData();
            config.ID       = Convert.ToInt32(data[0]);
            config.Name     = data[1];
            config.Value    = Convert.ToInt32(data[2]);
            config.Type     = (_C.ANIMAL)Convert.ToInt32(data[3]);
            config.Belong   = Convert.ToInt32(data[4]);



            if (!AnimalBelongDic.ContainsKey(config.Belong)) {
                AnimalBelongDic.Add(config.Belong, new List<AnimalData>());
            }
            AnimalBelongDic[config.Belong].Add(config);

            if (!AnimalTypeDic.ContainsKey(config.Type)) {
                AnimalTypeDic.Add(config.Type, new List<AnimalData>());
            }
            AnimalTypeDic[config.Type].Add(config);
        }
    }

    public List<AnimalData> GetDatasByType(_C.ANIMAL type)
    {
        List<AnimalData> data;
        if (AnimalTypeDic.TryGetValue(type, out data)) {
            return data;
        }

        return data;
    }

    public List<AnimalData> GetDatasByBelong(int belong)
    {
        List<AnimalData> data;
        if (AnimalBelongDic.TryGetValue(belong, out data)) {
            return data;
        }

        return data;
    }


}

