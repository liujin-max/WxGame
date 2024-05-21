using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace PC
{
    //动物的基类
    public class Animal
    {
        private AnimalData m_Data;

        public string Name { get {return m_Data.Name;}}
        public int Value { get {return m_Data.Value;}}
        public int Belong { get {return m_Data.Belong;}}
        public _C.ANIMAL Type{ get {return m_Data.Type;}}


        public Animal(AnimalData data)
        {
            m_Data = data;
        }
    }


    public class Dog : Animal
    {
        public Dog(AnimalData data) : base(data)
        {
        }
    }

    public class Cat : Animal
    {
        public Cat(AnimalData data) : base(data)
        {
        }
    }

    public class Mouse : Animal
    {
        public Mouse(AnimalData data) : base(data)
        {
        }
    }
}

