#if UNITY_EDITOR
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

    [Header("对应传送门")]
    public Vector2 Portal;

    public bool IsBan = false;

    [Header("自动移动方向")]
    public _C.DIRECTION AutoDirection;


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

        //
        transform.Find("Ban").gameObject.SetActive(IsBan);

        //
        var arrow = transform.Find("Arrow").gameObject;
        arrow.SetActive(this.AutoDirection != _C.DIRECTION.NONE);
        if (this.AutoDirection == _C.DIRECTION.LEFT)    arrow.transform.localEulerAngles = Vector3.zero;
        else if (AutoDirection == _C.DIRECTION.RIGHT)   arrow.transform.localEulerAngles = new Vector3(0, 0, 180);
        else if (AutoDirection == _C.DIRECTION.UP)      arrow.transform.localEulerAngles = new Vector3(0, 0, -90);
        else if (AutoDirection == _C.DIRECTION.DOWN)    arrow.transform.localEulerAngles = new Vector3(0, 0, 90);
    }
}
#endif