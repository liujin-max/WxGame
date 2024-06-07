#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;




public class MapEditor : MonoBehaviour
{
     [Header("关卡ID")]
    [SerializeField] private int m_ID;

    [Header("尺寸")]
    [SerializeField] private int m_Weight;
    [SerializeField] private int m_Height;

    [Header("行动步数")]
    [SerializeField] private int m_Step;

    [Header("通关金币")]
    [SerializeField] private int m_Coin;

    [Header("过关条件")]
    [SerializeField] private List<string> m_Conditions;
    [Header("方块池")]
    [SerializeField] private List<int> m_CardPool;



    [Header("控件")]

    [SerializeField] private Transform GridPivot;


    void ClearGrids()
    {
        List<GridEditor> _Removes = new List<GridEditor>();
        for (int i = 0; i < GridPivot.childCount; i++)
        {
            var g = GridPivot.GetChild(i).GetComponent<GridEditor>();
            g.Dispose();
          
            _Removes.Add(g);
        }

        _Removes.ForEach(g => {
            DestroyImmediate(g.gameObject);
        });
    }


    public void Clear()
    {
        m_ID        = 0;
        m_Weight    = 0;
        m_Height    = 0;
        m_Step      = 0;
        m_Coin      = 0;

        m_Conditions.Clear();
        m_CardPool.Clear();


        ClearGrids();
    }



    //生成格子
    public void InitGrids()
    {
        ClearGrids();

        int count = 0;
        for (int i = 0; i < m_Weight; i++) {
            for (int j = 0; j < m_Height; j++) {
                count++;

                var pos     = new Vector2((i - ((m_Weight - 1) / 2.0f)) * _C.DEFAULT_GRID_WEIGHT, (j - ((m_Height - 1) / 2.0f)) * _C.DEFAULT_GRID_HEIGHT);
                var entity  = Instantiate(Resources.Load<GameObject>("Prefab/MapEditor/GridEditor"), pos, Quaternion.identity, GridPivot);
                var grid    = entity.GetComponent<GridEditor>();
                grid.Init(count, i, j, pos);
            }
        }
    }

    //读取数据
    public void Load()
    {
        if (m_ID == 0) {
            EditorUtility.DisplayDialog("提示", "未填写Stage", "确定");
            return;
        }

        string json = ReadJsonFromFile(m_ID);

        StageJSON mapJSON = JsonUtility.FromJson<StageJSON>(json);

        m_ID        = mapJSON.ID;
        m_Weight    = mapJSON.Weight;
        m_Height    = mapJSON.Height;
        m_Step      = mapJSON.Step;
        m_Coin      = mapJSON.Coin;
        m_Conditions= mapJSON.Conditions;
        m_CardPool  = mapJSON.CardPool;

        ClearGrids();

        mapJSON.Grids.ForEach(grid_json => {
            var pos     = new Vector2((grid_json.X - ((m_Weight - 1) / 2.0f)) * _C.DEFAULT_GRID_WEIGHT, (grid_json.Y - ((m_Height - 1) / 2.0f)) * _C.DEFAULT_GRID_HEIGHT);
            var entity  = Instantiate(Resources.Load<GameObject>("Prefab/MapEditor/GridEditor"), pos, Quaternion.identity, GridPivot);
            var grid    = entity.GetComponent<GridEditor>();
            grid.Init(grid_json.Order, grid_json.X, grid_json.Y, pos);
            grid.IsValid = grid_json.IsValid;
            grid.JellyID = grid_json.JellyID;
        });

    }

    //导出数据
    public void Export()
    {
        if (m_ID == 0) {
            EditorUtility.DisplayDialog("提示", "未填写Stage", "确定");
            return;
        }

        StageJSON mapData = new StageJSON();
        mapData.ID      = m_ID;
        mapData.Weight  = m_Weight;
        mapData.Height  = m_Height;
        mapData.Step    = m_Step;
        mapData.Coin    = m_Coin;
        mapData.Conditions = m_Conditions;
        mapData.CardPool = m_CardPool;

        mapData.Grids   = new List<GridJSON>();
        for (int i = 0; i < GridPivot.childCount; i++)
        {
            var g = GridPivot.GetChild(i).GetComponent<GridEditor>();

            GridJSON gj = new GridJSON();
            gj.Order = g.Order;
            gj.X = g.X;
            gj.Y = g.Y;
            gj.IsValid = g.IsValid;
            gj.JellyID = g.JellyID;

            mapData.Grids.Add(gj);
        }

        string json = JsonUtility.ToJson(mapData);
        

        WriteJsonToFile(json);
    }

    void WriteJsonToFile(string json)
    {
        var path = _C.JSON_PATH + "/Stage_" + this.m_ID + ".json";
        Debug.Log("写入路径 " + path + ", Json : " + json);


        if (File.Exists(path))
        {
            bool flag = EditorUtility.DisplayDialog("提示", "是否覆盖：" + path, "确定", "取消");
            if (flag == true)
            {
                File.WriteAllText(path, json);

                EditorUtility.DisplayDialog("提示", "导出成功：" + path, "确定");
            }
        }
        else
        {
            File.WriteAllText(path, json);

            EditorUtility.DisplayDialog("提示", "导出成功：" + path, "确定");
        }
        
    }

    string ReadJsonFromFile(int stage)
    {
        var path = _C.JSON_PATH + "/Stage_" + this.m_ID + ".json";

        string json = File.ReadAllText(path);
        Debug.Log("读取路径 " + path + ", Json : " + json);


        return json;
    }
}
#endif