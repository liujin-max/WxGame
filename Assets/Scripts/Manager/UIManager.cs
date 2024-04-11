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

    void Awake()
    {
        BOTTOM  = GameObject.Find("Canvas/BOTTOM").transform;
        MAJOR   = GameObject.Find("Canvas/MAJOR").transform;
        BOARD   = GameObject.Find("Canvas/BOARD").transform;
        EFFECT  = GameObject.Find("Canvas/EFFECT").transform;
        TIP     = GameObject.Find("Canvas/TIP").transform;
    }

    public GameObject LoadWindow(string path, Transform parent)
    {
        return Instantiate<GameObject>(Resources.Load<GameObject>(path), parent);
    }

    public void UnloadWindow(GameObject window)
    {
        Destroy(window);
    }

    public GameObject LoadItem(string path, Transform parent)
    {
        return Instantiate<GameObject>(Resources.Load<GameObject>(path), parent);
    }
}
