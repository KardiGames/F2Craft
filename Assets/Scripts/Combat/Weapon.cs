using UnityEngine;
namespace Battle
{
    public class Weapon : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public AttackPriority PriorityToAttack(ActiveEntity target)
        {
            if (target.Tags.HasFlag(ActiveEntity.Flags.Building))
                return AttackPriority.Building;
            if (target.Tags.HasFlag(ActiveEntity.Flags.Flying))
                return AttackPriority.ImpossibleToAttack;
            
            return AttackPriority.Normal;
        }

        public ActiveEntity BetterPriority (ActiveEntity target1, ActiveEntity target2, out int priority)
        {
            priority = 0;
            AttackPriority priority2 = PriorityToAttack(target2);
            if (priority2 <= AttackPriority.ZeroDamage)
                return target1;

            AttackPriority priority1 = PriorityToAttack(target1);
            if (priority1 <= AttackPriority.ZeroDamage)
                return target2;

            bool inRange1 = 
        }
    }
}

