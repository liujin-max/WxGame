using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private GameObject m_BGM = null;

    //加载音效
    public GameObject Load(string path)
    {
        return Instantiate(Resources.Load<GameObject>(path), Vector3.zero, Quaternion.identity);
    }

    //播放音乐
    public void PlayBGM(string path)
    {
        if (m_BGM != null) {
            Destroy(m_BGM.gameObject);
        }

        m_BGM = Instantiate(Resources.Load<GameObject>(path), Vector3.zero, Quaternion.identity);

        DontDestroyOnLoad(m_BGM);
    }

    public void StopBGM()
    {
        if (m_BGM != null) {
            Destroy(m_BGM.gameObject);
        }
        m_BGM = null;
    }
}
