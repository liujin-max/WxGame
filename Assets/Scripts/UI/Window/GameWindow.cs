using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace PC
{
    public class GameWindow : MonoBehaviour
    {
        [SerializeField] private Transform m_CardPivot;
        [SerializeField] private TextMeshProUGUI m_OurScore;
        [SerializeField] private TextMeshProUGUI m_EnemyScore;
        [SerializeField] private Button m_BtnSettle;


        Card[,] m_Cards = new Card[_C.DEFAULT_WEIGHT, _C.DEFAULT_HEIGHT];

        void Awake()
        {
            m_BtnSettle.onClick.AddListener(()=>{
                Field.Instance.Settle();
            });


            EventManager.AddHandler(EVENT.UI_CARDTURNFRONT,     OnReponseCardTurnFront);
            EventManager.AddHandler(EVENT.UI_CARDTURNBACK,      OnReponseCardTurnBack);
        }


        void OnDestroy()
        {
            EventManager.DelHandler(EVENT.UI_CARDTURNFRONT,     OnReponseCardTurnFront);
            EventManager.DelHandler(EVENT.UI_CARDTURNBACK,      OnReponseCardTurnBack);
        }

        // Update is called once per frame
        void Update()
        {
            m_OurScore.text     = Field.Instance.GetScore(_C.SIDE.OUR).ToString();
            m_EnemyScore.text   = Field.Instance.GetScore(_C.SIDE.ENEMY).ToString();
        }


        public void InitCards(Animal[,] animals)
        {
            for (int i = 0; i < _C.DEFAULT_WEIGHT; i++) {
                for (int j = 0; j < _C.DEFAULT_HEIGHT; j++) {
                    var animal  = animals[i, j];
                    var item    = GameFacade.Instance.UIManager.LoadItem("Card", m_CardPivot).GetComponent<Card>();
                    item.Init(animal);
                    m_Cards[i, j] = item;
                }
            }
        }


        #region 监听事件
        private void OnReponseCardTurnBack(GameEvent @event)
        {
            var animal  = (Animal)@event.GetParam(0);
            var card    = m_Cards[animal.X, animal.Y];

            card.TurnBack();
        }

        private void OnReponseCardTurnFront(GameEvent @event)
        {
            var animal  = (Animal)@event.GetParam(0);
            var card    = m_Cards[animal.X, animal.Y];

            card.TurnFront();
        }

        #endregion
    }
}

