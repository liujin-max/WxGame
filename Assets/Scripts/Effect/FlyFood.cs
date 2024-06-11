using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlyFood : MonoBehaviour
{
    [SerializeField] private TextMeshPro m_Text;


    public void SetValue(int value)
    {
        m_Text.text = value.ToString();
    }
}
