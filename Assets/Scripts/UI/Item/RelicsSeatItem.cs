using System.Collections;
using System.Collections.Generic;
using CB;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelicsSeatItem : MonoBehaviour
{
    public Relics m_Relics;

    [SerializeField] private Image c_Icon;
    [SerializeField] private TextMeshProUGUI c_Value;

    public void Init(Relics relics = null)
    {
        m_Relics = relics;

        if (relics == null) {
            c_Icon.gameObject.SetActive(false);
            c_Value.text = "";

        } else {
            c_Icon.gameObject.SetActive(true);


            c_Icon.sprite = Resources.Load<Sprite>("UI/Relics/" + relics.ID);
            c_Icon.SetNativeSize();

            c_Value.text = relics.ShowValue();
        }
    }

    void Update()
    {
        if (m_Relics != null) {
            c_Value.text = m_Relics.ShowValue();
        }
    }
}
