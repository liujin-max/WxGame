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
        GameFacade.Instance.Game.Pause();
        yield return new WaitForSeconds(2);
        GameFacade.Instance.Game.Resume();
        GameFacade.Instance.UIManager.UnloadWindow(gameObject);

        yield return null; 
    }
}
