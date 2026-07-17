using UnityEngine;
namespace Combat
{

    public readonly struct DamageData
    {
        public int Amount { get; }
        public Weapon.Flags WeaponTags { get; }

        public DamageData (int amount, Weapon.Flags weaponTags)
        {
            Amount = amount;
            WeaponTags = weaponTags;
        }
    }

}