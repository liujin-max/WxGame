using System.Collections;
using System.Collections.Generic;
using System.Text;
using CB;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingWindow : MonoBehaviour
{
    [SerializeField] Button c_Mask;
    [SerializeField] Slider c_MusicSlider;
    [SerializeField] Slider c_SoundSlider;
    [SerializeField] Toggle c_VibrateToggle;


    void Awake()
    {
        c_Mask.onClick.AddListener(()=>{
            GameFacade.Instance.UIManager.UnloadWindow(gameObject);
        });

        c_MusicSlider.onValueChanged.AddListener((float value)=>{
            GameFacade.Instance.SystemManager.MusicVolume = value;
        });

        c_SoundSlider.onValueChanged.AddListener((float value)=>{
            GameFacade.Instance.SystemManager.SoundVolume = value;
        });

        c_VibrateToggle.onValueChanged.AddListener((flag)=>{
            GameFacade.Instance.SystemManager.VibrateFlag = flag;
        });
    }

    // Start is called before the first frame update
    void Start()
    {
        c_MusicSlider.value     = GameFacade.Instance.SystemManager.MusicVolume;
        c_SoundSlider.value     = GameFacade.Instance.SystemManager.SoundVolume;

        c_VibrateToggle.isOn    = GameFacade.Instance.SystemManager.VibrateFlag;
    }


}
