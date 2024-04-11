using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public float m_Time = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_Time -= Time.deltaTime;

        if (m_Time <= 0)
        {
            Destroy(gameObject);
        }
    }
}
