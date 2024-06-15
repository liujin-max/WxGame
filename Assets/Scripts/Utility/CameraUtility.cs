using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CameraUtility : MonoBehaviour
{
    private Camera m_Camera;
    // Start is called before the first frame update
    void Awake()
    {
        m_Camera = this.GetComponent<Camera>();

        FilterOrthographic(m_Camera);
    }

    public void Reset()
    {
        Debug.Log("Reset");
        FilterOrthographic(m_Camera);
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
                else
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

    public void AddOSize(float value)
    {
        transform.GetComponent<Camera>().orthographicSize += value;
    }

    public void DoShake()
    {
        transform.DOShakePosition(0.5f, 0.35f, 12, 60);
        Platform.Instance.VIBRATE(_C.VIBRATELEVEL.HEAVY);
    }
}
