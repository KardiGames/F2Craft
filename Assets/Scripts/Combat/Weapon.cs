using UnityEngine;
namespace Battle
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private int _playerNumber = -1;
        [SerializeField] private float _range;
        [SerializeField] private int _damage;
        [SerializeField] private float _attackCooldown;
        [SerializeField] private ActiveEntity _target;
        private float _cooldown = 0f;
        //public float Range => _range;
        public int PlayerNumber
        {
            get => _playerNumber; set
            {
                if (_playerNumber < 0 && value > 0)
                    _playerNumber = value;
                else
                    Debug.LogWarning("Trying initialize player number with " + value, gameObject);
            }
        }
        public ActiveEntity Target => _target;
        public bool TargetInRange
        {
            get
            {
                if (_target == null)
                    return false;
                if (_target.SqrDistanceTo(transform) > _range * _range)
                    return false;
                return true;
            }
        }
        private void Start()
        {
            if (_playerNumber < 0)
            {
                PlayerNumber = GetComponent<ActiveEntity>().PlayerNumber;
            }
        }

        void Update()
        {
            if (_target == null)
                _target = BestTarget();
            if (_cooldown > 0f)
                _cooldown -= Time.deltaTime;
            else if (TargetInRange)
                Attack();
        }

        private void Attack()
        {
            if (_cooldown <= 0f)
            {
                _target.GetDamage(_damage);
                _cooldown += _attackCooldown;
            }
        }

        public AttackPriority PriorityToAttack(ActiveEntity target)
        {
            if (target.Tags.HasFlag(ActiveEntity.Flags.Building))
                return AttackPriority.Building;
            if (target.Tags.HasFlag(ActiveEntity.Flags.Flying))
                return AttackPriority.ImpossibleToAttack;

            return AttackPriority.Normal;
        }

        /* There is no attacker accesseble yet
        public ActiveEntity BetterTarget(ActiveEntity attacker, ActiveEntity target1, ActiveEntity target2, out int priority)
        {
            priority = 0;
            AttackPriority priority2 = PriorityToAttack(target2);
            if (priority2 <= AttackPriority.ZeroDamage)
                return target1;

            AttackPriority priority1 = PriorityToAttack(target1);
            if (priority1 <= AttackPriority.ZeroDamage)
                return target2;

            bool inRange1 = attacker.SqrDistanceTo(target1.transform) < _range * _range;
            bool inRange2 = attacker.SqrDistanceTo(target2.transform) < _range * _range;
            
            if (inRange1 == true && inRange2 == false)
                return target1;
            if ()
            
        }*/
        private ActiveEntity BestTarget()
        {
            Transform weaponTransform = transform;
            float minSqrDistance = float.MaxValue;
            float sqrRange = _range * _range;
            ActiveEntity target = null;
            AttackPriority currentPriority = 0;
            foreach (ActiveEntity entity in ActiveEntity.GetEnemiesList(_playerNumber))
            {
                AttackPriority priority = PriorityToAttack(entity);
                if (priority == AttackPriority.ImpossibleToAttack)
                    continue;
                float sqrDistance = entity.SqrDistanceTo(weaponTransform);
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

