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

        void Awake()
        {
            GameFacade.Instance.Game.Pause();

            c_Mask.onClick.AddListener(()=>{
                GameFacade.Instance.UIManager.UnloadWindow(gameObject);
            });
        }

        void OnDestroy()
        {
            GameFacade.Instance.Game.Resume();
        }

        public void Init(Relics relics)
        {
            c_Icon.texture   = Resources.Load<Texture>("UI/Relics/" + relics.ID);
            c_Icon.SetNativeSize();

            this.ShowDescription(relics);
        }

        void ShowDescription(Relics relics)
        {
            c_DescriptionPivot.SetActive(true);

            c_DescriptionPivot.transform.Find("Name").GetComponent<ShakeText>().SetText(relics.Name);
            c_DescriptionPivot.transform.Find("Description").GetComponent<ShakeText>().SetText(relics.GetDescription());
        }
    }
}

