using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CameraUtility : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        var camera = this.GetComponent<Camera>();

        if (camera.orthographic == true) 
        {
            FilterOrthographic(camera);
        }
        else
        {   
            FilterPerspective(camera);
        }
    }

    void FilterOrthographic(Camera camera)
    {
        ScreenOrientation designAutoRotation = Screen.orientation;

        float aspect = camera.aspect;
        float designOrthographicSize = 9.6f;
        float designHeight = 1920f;
        float designWidth = 1080f;
        float designAspect = designWidth / designHeight;
        float widthOrthographicSize = designOrthographicSize * designAspect;
        switch (designAutoRotation)
        {
            case ScreenOrientation.Portrait:
                if(aspect < designAspect)
                {
                    camera.orthographicSize = widthOrthographicSize / aspect;
                }
                if (aspect > designAspect)
                {
                    camera.orthographicSize = designOrthographicSize;
                    //_camera.orthographicSize = designOrthographicSize * (aspect / designAspect);
                }
                break;
            case ScreenOrientation.AutoRotation:
                break;
            case ScreenOrientation.LandscapeLeft:
                break;
            case ScreenOrientation.LandscapeRight:
                break;
            case ScreenOrientation.PortraitUpsideDown:
                break;
            default:
                break;

        }
    }

    void FilterPerspective(Camera camera)
    {
        //Camera.main.fieldOfView是用来设置摄像机视野的大小

        float targetHight = 1920.0f;
        if (1080 * Screen.height > 1920 * Screen.width)
        {
            targetHight = 1080f * Screen.height / Screen.width;
        }
        camera.fieldOfView = 60 * (targetHight/1920.0f);
    }

    public void DoShake()
    {
        transform.DOShakePosition(0.5f, 0.35f, 12, 60);
        Platform.Instance.VIBRATE(_C.VIBRATELEVEL.HEAVY);
    }

    public void DoSmallShake()
    {
        transform.DOShakePosition(0.3f, 0.2f, 12, 60);
    }
}
