using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace PC
{
   public class Card : MonoBehaviour
    {
        [SerializeField] private GameObject m_Back;

        [SerializeField] private GameObject m_Front;
        [SerializeField] private TextMeshProUGUI m_Text;
        [SerializeField] private TextMeshProUGUI m_Value;
        [SerializeField] private Transform m_ArrowPivot;


        private Animal m_Animal;

        void Start()
        {
            // m_Back.SetActive(true);
            // m_Front.SetActive(false);

            //2 6 4 4
            //5 3 3 4
        }
        

        public void Init(Animal animal)
        {
            m_Animal = animal;

            if (animal.SIDE == _C.SIDE.OUR)
            {
                m_Front.GetComponent<Image>().color = Color.red;
            } 
            else if (animal.SIDE == _C.SIDE.ENEMY)
            {
                m_Front.GetComponent<Image>().color = Color.green;
            }
            else
            {   
                if (animal.Type == _C.ANIMAL.DOG)
                {
                    m_Front.GetComponent<Image>().color = Color.blue;
                }
            }

            InitArrows(animal);

            FlushUI();
        }

        void FlushUI()
        {
            m_Text.text = m_Animal.Name;

            if (m_Animal.Value > 0)  m_Value.text = m_Animal.Value.ToString();
            else m_Value.text = "";
            
        }

        void InitArrows(Animal animal)
        {
            if (animal.Type != _C.ANIMAL.DOG) return;
            
            Dog dog = animal as Dog;

            if (dog == null) return;

            var directions = dog.GetFocusDirections();


            directions.ForEach(direction => {
                var arrow = GameFacade.Instance.UIManager.LoadItem("CardArrow", m_ArrowPivot);

                int angle_radians = (int)(Mathf.Atan2(direction.x, direction.y)  * (180 / Mathf.PI));
                arrow.transform.localEulerAngles = new Vector3(0, 0, angle_radians);
            });
        }
    } 
}

