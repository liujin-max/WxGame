using UnityEngine;
using DG.Tweening;
using System;

public class BezierCurveAnimation : MonoBehaviour
{
    private Action m_Callback;

    private Vector2 m_StartPos;
    private Vector2 m_ControlPos;
    private Vector2 m_EndPos;

    private float duration = 0.8f; // 动画持续时间
    private float t = 0f; // 用于插值计算

    public void Fly(Vector2 endPoint, Action callback = null)
    {
        m_Callback  = callback;

        m_StartPos  = transform.position;
        m_EndPos    = endPoint;
        
        float distance  = Vector3.Distance(m_StartPos, endPoint);
        m_ControlPos    = ToolUtility.FindPointOnCircle(m_StartPos, distance * 0.7f, RandomUtility.Random(0, 360));
    }



    void Update()
    {
        if (t < 1f)
        {
            t += Time.deltaTime / duration;

            Vector2 newPos = CalculateBezierPoint(t, m_StartPos, m_ControlPos, m_EndPos);
            transform.position = newPos;
        } else {
            if (m_Callback != null) m_Callback();

            Destroy(gameObject);
        }
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;

        return p;
    }
}
