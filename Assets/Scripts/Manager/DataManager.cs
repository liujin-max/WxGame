using UnityEngine;

namespace CB
{
    //全局数据类
    public class DataManager : MonoBehaviour
    {
        public  const string KEY_STAGE = "KEY_STAGE1";


        public const string KEY_GUIDE  = "KEY_GUIDE";
        public const string KEY_MUSIC  = "KEY_MUSIC";  //音乐
        public const string KEY_SOUND  = "KEY_SOUND";  //音效


        //最高分
        private int m_Score;
        public int Score { get {return m_Score;} }


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
            m_Score = PlayerPrefs.GetInt(KEY_STAGE, 0);


            m_MusicVolume   = PlayerPrefs.GetFloat(KEY_MUSIC, 1);
            m_SoundVolume   = PlayerPrefs.GetFloat(KEY_SOUND, 1);
        }

        //层数记录
        public void SetScore(int value)
        {
            if (value <= m_Score ) return;

            m_Score = value;

            PlayerPrefs.SetInt(KEY_STAGE, value);
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
