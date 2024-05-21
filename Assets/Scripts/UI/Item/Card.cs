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


        private Animal m_Animal;

        void Start()
        {
            // m_Back.SetActive(true);
            // m_Front.SetActive(false);
        }
        

        public void Init(Animal animal)
        {
            m_Animal = animal;

            if (animal.Type == _C.ANIMAL.CAT)
            {
                if (animal.Belong == 100)
                {
                    m_Front.GetComponent<Image>().color = Color.red;
                }
                else
                {
                    m_Front.GetComponent<Image>().color = Color.green;
                }
            } 
            else if (animal.Type == _C.ANIMAL.DOG)
            {
                m_Front.GetComponent<Image>().color = Color.blue;
            }
            

            FlushUI();
        }

        void FlushUI()
        {
            m_Text.text = m_Animal.Name;

            if (m_Animal.Value > 0) {
                m_Value.text    = m_Animal.Value.ToString();
            }
            else m_Value.text    = "";
            
        }
    } 
}

