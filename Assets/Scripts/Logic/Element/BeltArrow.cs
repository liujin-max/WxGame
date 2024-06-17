using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BeltArrow : MonoBehaviour
{
    private Grid m_Grid;

    private float m_Percent;
    private Vector2 m_LastPosition;



    public void Init(Grid grid)
    {
        m_Grid = grid;

        m_LastPosition  = grid.Position;
        transform.localEulerAngles = GetAngle(m_Grid.BeltDirection);
    }

    Vector3 GetAngle(_C.DIRECTION dIRECTION)
    {
        Vector3 angle = Vector3.zero;

        switch (dIRECTION)
        {
            case _C.DIRECTION.LEFT:
                return Vector3.zero;

            case _C.DIRECTION.RIGHT:
                return new Vector3(0, 0, 180);

            case _C.DIRECTION.UP:
                return new Vector3(0, 0, -90);

            case _C.DIRECTION.DOWN:
                return new Vector3(0, 0, 90);
        }


        return angle;
    }

    Grid GetBelt2Grid()
    {
        Grid to_grid = null;

        if (m_Grid.IsPortalCanCross(m_Grid.Card, m_Grid.BeltDirection, true) == true)    //传送门
        {
            to_grid = m_Grid.Portal;
        }
        else to_grid = Field.Instance.GetGridByDirection(m_Grid, m_Grid.BeltDirection);

        return to_grid;
    }

    void FixedUpdate()
    {
        var to_grid = this.GetBelt2Grid(); //Field.Instance.GetGridByDirection(m_Grid, m_Grid.BeltDirection);
        if (to_grid == null)  return;


        if (Field.Instance.GetDistanceByGrids(m_Grid, to_grid) > 1) {
            m_Percent = 1;
        }

        m_Percent += Time.fixedDeltaTime / 3;

        transform.localPosition = Vector2.Lerp(m_LastPosition, to_grid.Position, m_Percent);


        if (to_grid.BeltDirection != m_Grid.BeltDirection) {
            if (m_Percent >= 0.9f) {
                m_Percent   = 0;
                m_Grid      = to_grid;

                m_LastPosition  = transform.localPosition;

                transform.DOLocalRotate(GetAngle(to_grid.BeltDirection), 0.3f).SetEase(Ease.Linear);
            }
        } else {
            if (m_Percent >= 1) {
                m_Percent   = 0;
                m_Grid      = to_grid;

                m_LastPosition  = transform.localPosition;
            }
        }
    }

    public void Dispose()
    {
        Destroy(gameObject);
    }
}
