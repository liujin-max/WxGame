using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadItem : MonoBehaviour
{
    [SerializeField] private UrlImageUtility m_Head;

    public void Init(string url)
    {
        if (string.IsNullOrEmpty(url) == true) {
            m_Head.SetLocalImage("UI/Set/Set_default_head");
            return;
        }
        m_Head.SetImage(url);
    }
}
