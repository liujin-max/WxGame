using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


//管理所有的关卡
public class Levels
{
    public StageJSON GetStageJSON(int id)
    {
        var path = "Json/Stage_" + id;

        TextAsset jsonAsset = Resources.Load<TextAsset>(path);
        if (jsonAsset != null) 
        {
            return JsonUtility.FromJson<StageJSON>(jsonAsset.text);
        }
        
        Debug.LogError("未读取到配置：" + path);
        return null;
    }

    //判断进入关卡的体力是否足够
    public bool IsFoodEnough2Next(StageJSON stage_json)
    {
        // var stage_json  = this.GetStageJSON(level);

        if (stage_json == null) return false;

        if (GameFacade.Instance.DataCenter.User.Food >= stage_json.Food)
        {
            return true;
        }

        return false;
    }
}
