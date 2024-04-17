using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeChatWASM;

public class UIFitter : MonoBehaviour
{
    void Start()
    {
        FillterUI();
    }
    private void FillterUI()
    {
        #if UNITY_EDITOR

        #else
            // WX.InitSDK((int code) => {
                RectTransform rectTransform = GetComponent<RectTransform>();

                var info = WX.GetSystemInfoSync();
                float py = (float)info.safeArea.top / (float)info.windowHeight;

                // Debug.Log("safeArea : " + info.safeArea.top);
                // Debug.Log("windowHeight : " + info.windowHeight);
                // Rootrect初始时设置其Anchor，使其与父节点一样大，也就是屏幕的大小
                // 调整屏幕移到刘海屏下面, 
                float rate = (float)info.safeArea.top / (float)info.windowHeight;
                rectTransform.anchorMin = new Vector2(0,  rate);

                rectTransform.anchorMax = new Vector2(1, 1 - rate);

                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;

                // 重新计算缩放，让高度占满刘海屏以下的区域
                // cs.referenceResolution = new Vector2(cs.referenceResolution.x, cs.referenceResolution.y * (1.0f+py));
            // });

        #endif
        
    
    }
}
