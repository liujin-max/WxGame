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

    public void DoShake()
    {
        transform.DOShakePosition(0.5f, 0.35f, 12, 60);
    }
}
