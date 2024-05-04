using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadItem : MonoBehaviour
{
    [SerializeField] private UrlImageUtility m_Head;

    public void Init(string url)
    {
        m_Head.SetImage(url);
    }
}
