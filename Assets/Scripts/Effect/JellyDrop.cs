using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyDrop : MonoBehaviour
{
    public GameObject m_Pregab;

    private CDTimer m_Timer = new CDTimer(0.1f);
    


    void FixedUpdate()
    {
        m_Timer.Update(Time.deltaTime);
        if (m_Timer.IsFinished()) {
            m_Timer.Reset(RandomUtility.Random(300, 600) / 1000.0f);


            Vector3 pos = new Vector3(RandomUtility.Random(-500, 600) / 100.0f, 12, 0);
            var obj = Instantiate(m_Pregab, pos, Quaternion.identity);
            obj.transform.localEulerAngles = new Vector3(0, 0, RandomUtility.Random(0, 180));


            int rand_id     = RandomUtility.Random(10000, 10007);
            obj.GetComponent<SpriteRenderer>().sprite   = Resources.Load<Sprite>("UI/Element/jelly_" + rand_id);

            float pos_x     = RandomUtility.Random(0, 100) / 100.0f;
            float pos_y     = RandomUtility.Random(0, 100) / 100.0f * -1;

            if (pos.x > 0) {
                pos_x *= -1;
            }
            obj.GetComponent<Rigidbody2D>().velocity = new Vector2(pos_x, pos_y) * 5;
        }
    }
}
