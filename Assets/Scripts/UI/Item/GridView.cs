using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridView : MonoBehaviour
{
    private Grid m_Grid;
    public Grid Grid {get { return m_Grid; }}
    public void Init(Grid grid)
    {
        m_Grid = grid;
    }

    public void Show(bool flag)
    {
        gameObject.SetActive(flag);
    }

}
