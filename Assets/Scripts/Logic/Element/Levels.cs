using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//管理所有的关卡
public class Levels
{
    //关卡信息
    private Dictionary<int, StageData> m_StageDic = new Dictionary<int, StageData>();
    private List<StageData> m_Stages = new List<StageData>();

    public void Init()
    {
        //关卡数据
        List<string[]> list = GameFacade.Instance.CsvManager.GetStringArrays(CsvManager.TableKey_Stage);
        foreach (string[] data in list) {
            StageData config = new StageData();
            config.ID       = Convert.ToInt32(data[0]);
            config.Name     = data[1];
            config.Weight   = Convert.ToInt32(data[2]);
            config.Height   = Convert.ToInt32(data[3]);
            config.Step     = Convert.ToInt32(data[4]);
            config.Condition= data[5];
            config.Coin     = Convert.ToInt32(data[6]);
            config.Cards    = data[7];

            m_Stages.Add(config);
            m_StageDic[config.ID]  = config;
        }
    }

    public StageData GetStageData(int id)
    {
        StageData data;
        if (m_StageDic.TryGetValue(id, out data)) {
            return data;
        }

        return data;
    }
}
