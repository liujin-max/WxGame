using UnityEngine;
using WeChatWASM;

namespace CB
{
    //系统类
    public class SystemManager : MonoBehaviour
    {

        public const string KEY_MUSIC  = "KEY_MUSIC";  //音乐
        public const string KEY_SOUND  = "KEY_SOUND";  //音效


        private float m_MusicVolume;
        public float MusicVolume {
            get { return m_MusicVolume;}
            set {
                m_MusicVolume = value;
                PlayerPrefs.SetFloat(KEY_MUSIC, m_MusicVolume);
            }
        }

        private float m_SoundVolume;
        public float SoundVolume {
            get { return m_SoundVolume;}
            set {
                m_SoundVolume = value;
                PlayerPrefs.SetFloat(KEY_SOUND, m_SoundVolume);
            }
        }

        void Awake()
        {
            m_MusicVolume   = PlayerPrefs.GetFloat(KEY_MUSIC, 1);
            m_SoundVolume   = PlayerPrefs.GetFloat(KEY_SOUND, 1);
        }


        public int GetIntByKey(string key)
        {
            return PlayerPrefs.GetInt(key, 0);
        }

        public void SetIntByKey(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

    }
}