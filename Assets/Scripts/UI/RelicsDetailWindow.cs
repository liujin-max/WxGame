using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



namespace CB
{
    public class RelicsDetailWindow : MonoBehaviour
    {
        [SerializeField] Button c_Mask;
        [SerializeField] RawImage c_Icon;
        [SerializeField] GameObject c_DescriptionPivot;


        public void Init(Relics relics, Action callback)
        {
            c_Icon.texture   = Resources.Load<Texture>("UI/Relics/" + relics.ID);
            c_Icon.SetNativeSize();

            this.ShowDescription(relics);

            c_Mask.onClick.AddListener(()=>{
                if (callback != null)
                    callback();
            });
        }

        void ShowDescription(Relics relics)
        {
            c_DescriptionPivot.SetActive(true);

            c_DescriptionPivot.transform.Find("Name").GetComponent<ShakeText>().SetText(relics.Name);
            c_DescriptionPivot.transform.Find("Description").GetComponent<ShakeText>().SetText(relics.GetDescription());
        }
    }
}

