using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoreGameWindow : MonoBehaviour
{
    void Awake()
    {

    }

    void OnDestroy()
    {

    }

    public void OnClose()
    {
        GameFacade.Instance.UIManager.UnloadWindow(gameObject);
    }

    //观看广告
    public void LookVideo()
    {
        Platform.Instance.REWARD_VIDEOAD("adunit-f8dadc10744cf983");
    }

    //前往【指尖方块】
    public void GotoFingerBlock()
    {
        Platform.Instance.OPENMINIGAME("wx4084a464d1d00b1d");
    }
}
