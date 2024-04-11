using UnityEngine;
using DG.Tweening;

public class BezierCurveAnimation : MonoBehaviour
{
    private Vector3 m_StartPos;
    private Vector3 m_ControlPos;
    private Vector3 m_EndPos;

    private float duration = 0.8f; // 动画持续时间
    private float t = 0f; // 用于插值计算

    public void Fly(Vector3 endPoint)
    {
        m_StartPos = transform.localPosition;

        Vector3 direction = Quaternion.Euler(0, 0, RandomUtility.Random(0, 360)) * Vector3.right * Vector3.Distance(m_StartPos, endPoint);

        m_ControlPos = m_StartPos + direction;

        m_EndPos = endPoint;
    }



    void Update()
    {
        if (t < 1f)
        {
            t += Time.deltaTime / duration;

            Vector3 newPos = CalculateBezierPoint(t, m_StartPos, m_ControlPos, m_EndPos);
            transform.position = newPos;
        } else {
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
