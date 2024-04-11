using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TipWindow : MonoBehaviour
{
    [SerializeField] GameObject c_TipPivot;

    // Start is called before the first frame update
    void Start()
    {
        c_TipPivot.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FlyTip(string text)
    {
        c_TipPivot.SetActive(true);
        c_TipPivot.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = text;
        c_TipPivot.GetComponent<Animation>().Play("FlyTip");
    }
}
