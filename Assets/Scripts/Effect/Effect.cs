using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public float m_Time = 0;
    public bool m_IsLoop = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsLoop) return ;
        
        m_Time -= Time.deltaTime;

        if (m_Time <= 0)
        {
            Destroy(gameObject);
        }
    }
}
