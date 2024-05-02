using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeChatWASM;


namespace CB
{
    public class RankWindow : MonoBehaviour
    {
        [SerializeField] private Button Mask;
        [SerializeField] private RawImage RankImg;
        [SerializeField] private GameObject Box;



        public void Hide()
        {
            Mask.gameObject.SetActive(false);
            Box.SetActive(false);

            WX.HideOpenData();
        }


        public void Show()
        {
            Mask.gameObject.SetActive(true);
            Box.SetActive(true);

            // WX.ShowOpenData(RankImg.texture, 200, 480, 680, 860);
            var referenceResolution = new Vector2(1080, 1920);
            var p = RankImg.transform.position;
            // 计算渲染的区域、大小，通知开放数据域初始化渲染
            // 这里的计算效果不一定理想，需根据实际慢慢调试
            // 此处设置的值可能影响排行榜单的滚动、点击等操作
            WX.ShowOpenData(RankImg.texture, (int)p.x, Screen.height - (int)p.y, (int)((Screen.width / referenceResolution.x) * RankImg.rectTransform.rect.width), (int)((Screen.width / referenceResolution.x) * RankImg.rectTransform.rect.height));



            WXUtility.ShowFriendsRank();
        }
    }
}

