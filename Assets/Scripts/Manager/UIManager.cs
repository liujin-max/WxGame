using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public Transform BOTTOM;
    public Transform MAJOR;
    public Transform TIP;
    public Transform BOARD;
    public Transform EFFECT;
    public Transform RANK;


    private Dictionary<string, GameObject> WindowCaches = new Dictionary<string, GameObject>();

    void Awake()
    {
        BOTTOM  = GameObject.Find("Canvas/BOTTOM").transform;
        MAJOR   = GameObject.Find("Canvas/MAJOR").transform;
        BOARD   = GameObject.Find("Canvas/BOARD").transform;
        EFFECT  = GameObject.Find("Canvas/EFFECT").transform;
        TIP     = GameObject.Find("Canvas/TIP").transform;

        RANK    = GameObject.Find("RankCanvas").transform;
    }

    public GameObject LoadWindow(string path, Transform parent)
    {
        return Instantiate<GameObject>(Resources.Load<GameObject>(path), parent);
    }

    public void UnloadWindow(GameObject window)
    {
        Destroy(window);
    }

    //显示界面
    public GameObject ShowWindow(string name)
    {
        GameObject cache;
        if (WindowCaches.TryGetValue(name, out cache)) {
            // cache.SetActive(true);
            cache.transform.localPosition = Vector3.zero;
            WindowCaches.Remove(name);
            return cache;
        }
        return null;
    }

    //隐藏界面(不销毁)
    public void HideWindow(string name, GameObject obj)
    {
        GameObject cache;
        if (WindowCaches.TryGetValue(name, out cache)) {
            cache.SetActive(false);
            return;
        }

        obj.transform.localPosition = new Vector3(9999, 9999, 0);
        WindowCaches.Add(name, obj);
    }

    public GameObject LoadItem(string path, Transform parent)
    {
        return Instantiate<GameObject>(Resources.Load<GameObject>(path), parent);
    }
}
