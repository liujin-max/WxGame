using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace CB
{
    public class BoxShake : MonoBehaviour
    {
        public void MegaShake()
        {
            transform.DOShakePosition(0.3f, 0.1f);
        }
    }
}
