using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Jelly : MonoBehaviour
{
    private Card m_Card;

    //实体
    private GameObject m_Entity;
    public GameObject Entity {get {return m_Entity;}}


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Jelly(Card card)
    {
        m_Card = card;
    }


    public void Display()
    {
        m_Entity = Instantiate(Resources.Load<GameObject>("Prefab/Element/Jelly"), Vector3.zero, Quaternion.identity, Field.Instance.Land.ENTITY_ROOT);
        m_Entity.transform.localPosition = new Vector3(m_Card.Grid.Position.x, m_Card.Grid.Position.y, -0.5f);
        m_Entity.transform.localEulerAngles = Vector3.zero;
    }

    public void Flush()
    {

    }

    public void Destroy()
    {

    }
}
