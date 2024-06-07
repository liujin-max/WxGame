#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;



//指定对那个脚本进行拓展
[CustomEditor(typeof(MapEditor))]
public class InspectorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
 
        //拿到拓展的类 调用内部公开的方法
        MapEditor map = (MapEditor)target;
        if (GUILayout.Button("清空数据"))
        {
            map.Clear();
        }

        if (GUILayout.Button("读取关卡数据"))
        {
            map.Load();
        }

        if (GUILayout.Button("生成Grids"))
        {
            map.InitGrids();
        }


        if (GUILayout.Button("导出Json"))
        {
            map.Export();
        }
    }
}

#endif