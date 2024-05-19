using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    private float m_OriginVolume;
    private AudioSource m_AudioSource;
    public bool AutoDestroy = true;


    void Awake()
    {
        m_AudioSource = transform.GetComponent<AudioSource>();

        m_OriginVolume= m_AudioSource.volume;
        // m_AudioSource.Play();
        // m_AudioSource.volume = m_AudioSource.volume * GameFacade.Instance.SystemManager.SoundVolume;
    }

    //因为部分音效绑定在特效上，特效走的是缓存池，所以不能放在Awake里做播放
    void OnEnable()
    {
        m_AudioSource.Play();
        m_AudioSource.volume = m_OriginVolume * GameFacade.Instance.SystemManager.SoundVolume;
    }

    void Update()
    {
        if (AutoDestroy == true) {
            if (m_AudioSource.isPlaying == false) {
                Destroy(gameObject);
            }
        }
    }
}
