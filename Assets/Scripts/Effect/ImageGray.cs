using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageGray : MonoBehaviour
{


    public void TurnSpriteGray(bool flag)
    {
        SpriteRenderer m_spr = transform.GetComponent<SpriteRenderer>();

        if (m_spr == null) return;

        if (flag == true)
        {
            m_spr.material = new Material(Shader.Find("Sprite/SpriteGray"))
            {
                name = "gray"
            };
            m_spr.material.SetFloat("_Switch", 1);
        }
        else
        {
            if (m_spr.material.name == "gray (Instance)")
            {
                m_spr.material.SetFloat("_Switch", 0);
            }
        }
    }
}
