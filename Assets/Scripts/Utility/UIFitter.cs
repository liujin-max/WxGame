using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class UIFitter : MonoBehaviour
{
    void Start()
    {
        Platform.Instance.ADAPTATION(GetComponent<RectTransform>());
    }
}
