using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConditionItem : MonoBehaviour
{
    private Condition m_Condition;

    [SerializeField] private Image m_Icon;
    [SerializeField] private TextMeshProUGUI m_Count;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(Condition condition)
    {
        m_Condition = condition;

        if (condition.ID == 10001) m_Icon.color = Color.red;
        if (condition.ID == 10002) m_Icon.color = Color.yellow;
        if (condition.ID == 10003) m_Icon.color = Color.blue;
        if (condition.ID == 10004) m_Icon.color = Color.green;

        FlushUI();
    }

    void FlushUI()
    {
        m_Count.text = m_Condition.Count.Current.ToString();
    }

    public void Show(bool flag)
    {
        gameObject.SetActive(flag);
    }
}
