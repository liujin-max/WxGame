using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


//管理所有的关卡
public class Levels
{
    public void Init()
    {

    }

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
}
