using UnityEngine;
namespace Battle
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] float _range;
        [SerializeField] private int _damage;
        [SerializeField] private float _attackCooldown;
        [SerializeField] private ActiveEntity _target;
        private float _cooldown = 0f;
        public float Range => _range;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            if ()
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

        public ActiveEntity BetterTarget (ActiveEntity attacker, ActiveEntity target1, ActiveEntity target2, out int priority)
        {
            priority = 0;
            AttackPriority priority2 = PriorityToAttack(target2);
            if (priority2 <= AttackPriority.ZeroDamage)
                return target1;

            AttackPriority priority1 = PriorityToAttack(target1);
            if (priority1 <= AttackPriority.ZeroDamage)
                return target2;

            bool inRange1 = attacker.SqrDistanceTo(target1.transform) > ;
        }
        private ActiveEntity BestTarget()
        {
            float minSqrDistance = float.MaxValue;
            float sqrRange = _range * _range;
            ActiveEntity target = null;
            AttackPriority currentPriority = 0;
            foreach (ActiveEntity entity in ActiveEntity.GetEnemiesList(_playerNumber))
            {
                AttackPriority priority = _weapon.PriorityToAttack(entity);
                if (priority == AttackPriority.ImpossibleToAttack)
                    continue;
                float sqrDistance = SqrDistanceTo(entity.transform);
                if (sqrDistance < sqrRange && priority > currentPriority)
                {
                    target = entity;
                    currentPriority = priority;
                }
                else if (currentPriority <= AttackPriority.Building && sqrDistance < minSqrDistance)
                {
                    target = entity;
                    minSqrDistance = sqrDistance;
                }
            }
            return target;
        }
    }
}

