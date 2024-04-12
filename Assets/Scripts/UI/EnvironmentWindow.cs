using System.Collections;
using System.Collections.Generic;
using CB;
using TMPro;
using UnityEngine;

public class EnvironmentWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_Description;


    public void Init(Environment environment)
    {
        m_Title.text = environment.Name;
        m_Description.text = environment.GetDescription();

        StartCoroutine("AutoDestroy");
    }

    IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(3);

        GameFacade.Instance.UIManager.UnloadWindow(gameObject);

        yield return null; 
    }
}
