
using System;
using UnityEngine;

public class JoyStickWindow : MonoBehaviour
{
    [SerializeField] GameObject m_JoyPan;
    [SerializeField] GameObject m_JoyPoint;

    float m_Radius = 1.5f;


    

    void Awake()
    {
        HideJoy();
    }


    void OnDestroy()
    {

    }



    public void ShowJoy(Vector3 world_pos)
    {
        m_JoyPan.SetActive(true);
        m_JoyPan.transform.position = new Vector3(world_pos.x, world_pos.y, 0);
        m_JoyPoint.transform.localPosition  = Vector3.zero;

    }

    public Vector3 DragJoy(Vector3 world_pos)
    {
        Vector3 vector3 = new Vector3(world_pos.x, world_pos.y, 0);

        Vector3 offset  = vector3 - m_JoyPan.transform.position;
        // Debug.Log("长度：" + offset.magnitude);

        float length    = Math.Min(offset.magnitude, m_Radius);
        Vector3 pos     = offset.normalized * length;
        m_JoyPoint.transform.localPosition = pos * 100;

        return pos;
    }

    public void HideJoy()
    {
        m_JoyPan.SetActive(false);
    }
}
