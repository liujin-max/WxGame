using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;



public class GridEditor : MonoBehaviour
{
    public int Order;
    public int X;
    public int Y;
    private Vector2 Position;

    public bool IsValid = true;
    public int JellyID;


    public void Init(int order, int x, int y, Vector2 position)
    {
        Order   = order;
        X       = x;
        Y       = y;

        Position = position;

        StartListeningToUpdates();
    }

    public void Dispose()
    {
        EditorApplication.update -= OnEditorUpdate;
    }

    // 注册Editor更新事件
    [InitializeOnLoadMethod]
    private void StartListeningToUpdates()
    {
        EditorApplication.update += OnEditorUpdate;
    }
 
    // 编辑器更新时调用
    private  void OnEditorUpdate()
    {
        gameObject.SetActive(IsValid);

        if (JellyID > 0)
            transform.Find("Text").GetComponent<TextMeshPro>().text = JellyID.ToString();
        else
            transform.Find("Text").GetComponent<TextMeshPro>().text = "";
    }
}
