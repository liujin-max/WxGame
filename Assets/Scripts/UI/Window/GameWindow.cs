using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PC
{
    public class GameWindow : MonoBehaviour
    {
        [SerializeField] private Transform m_CardPivot;


        Card[,] m_Cards = new Card[5, 5];



        void Awake()
        {
            InitCards();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void InitCards()
        {
            for (int i = 0; i < 5; i++) {
                for (int j = 0; j < 5; j++) {
                    m_Cards[i, j] = GameFacade.Instance.UIManager.LoadItem("Card", m_CardPivot).GetComponent<Card>();
                }
            }
        }

        public void InitAnimals(Animal[,] animals)
        {
            for (int i = 0; i < 5; i++) {
                for (int j = 0; j < 5; j++) {
                    var animal = animals[i, j];
                    Debug.Log("InitAnimals : " + i + "," + j + ", " + animal.Name);
                    m_Cards[i, j].Init(animal);
                }
            }
        }
    }
}

