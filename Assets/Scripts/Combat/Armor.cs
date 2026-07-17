using System;
using UnityEngine;
namespace Combat
{

    public class Armor : MonoBehaviour
    {
        [Flags]
        public enum Flags
        {
            None = 0,
            EnergyShield = 1 << 0,
            AntiBallistic = 1 << 1,
            ReactiveArmor = 1 << 2
        }

        private int _defence = 0;
        private Flags _tags;

        public int Defence => _defence;
        public Flags Tags => _tags;

        public void ReduceDamage(DamageData damage)
        {
            
        }
    }

}