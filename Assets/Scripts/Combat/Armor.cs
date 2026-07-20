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

        [SerializeField] private int _defence = 0;
        [SerializeField] private Flags _tags;

        public int Defence => _defence;
        public Flags Tags => _tags;

        public DamageData ReduceDamage(DamageData damage)
        {
            return damage;
        }
    }

}