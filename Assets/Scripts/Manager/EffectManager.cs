using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    //加载特效
    public GameObject Load(string path, Vector3 pos, GameObject parent = null)
    {
        var e = Instantiate(Resources.Load<GameObject>(path), pos, Quaternion.identity);

        if (parent != null) {
            e.transform.SetParent(parent.transform);
            e.transform.localPosition = pos;
        }

        return e;
    }


    public void FlyRate(Vector3 pos,  float value)
    {
        var e = GameFacade.Instance.EffectManager.Load(EFFECT.RATENUMBER, pos * 100, GameFacade.Instance.UIManager.EFFECT.gameObject);
        e.transform.localScale = Vector3.one;
        e.GetComponent<RateNumber>().Fly(value);
    }
}
