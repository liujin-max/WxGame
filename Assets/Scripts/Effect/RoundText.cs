using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundText : MonoBehaviour
{
    [SerializeField] private TextMeshPro _Rounds;


    public void Init(int rounds)
    {
        _Rounds.text = rounds.ToString();
    }
}
