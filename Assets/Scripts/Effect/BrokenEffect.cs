using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_ParticleSystem;

    public void Init(int jelly_id)
    {
        m_ParticleSystem.textureSheetAnimation.SetSprite(0, Resources.Load<Sprite>("UI/Element/jelly_" + jelly_id));
    }
}
