using UnityEngine;
namespace Combat
{

    public struct DamageData
    {
        public int Amount;
        public Weapon.Flags WeaponTags;

        public DamageData(int amount, Weapon.Flags weaponTags)
        {
            Amount = amount;
            WeaponTags = weaponTags;
        }

    }

}