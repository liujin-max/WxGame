using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    private AudioSource m_AudioSource;
    public bool AutoDestroy = true;


    void Awake()
    {
        m_AudioSource = transform.GetComponent<AudioSource>();

        m_AudioSource.Play();
        m_AudioSource.volume = m_AudioSource.volume * GameFacade.Instance.DataManager.SoundVolume;
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
