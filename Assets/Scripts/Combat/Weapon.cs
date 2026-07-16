using System;
using UnityEngine;
namespace Battle
{
    public class Weapon : MonoBehaviour
    {
        [Flags] public enum Flags
        {
            None = 0,
            Building = 1 << 0,
            Mechanical = 1 << 1,
            Biological = 1 << 2,
            Flying = 1 << 3,
            Hover = 1 << 4,
            Massive = 1 << 5,
            Superheavy = 1 << 6,
            Cloaked = 1 << 7,
            Detector = 1 << 8,
            Regeneration = 1 << 9,
            Carrier = 1 << 10,
            Burrow = 1 << 11
        }

        [SerializeField] private int _playerNumber = -1;
        [SerializeField] private float _range;
        [SerializeField] private int _damage;
        [SerializeField] private float _attackCooldown;
        [SerializeField] private ActiveEntity _target;
        private float _cooldown = 0f;
        private bool _wasTargetAttacked = false;
        //public float Range => _range;
        public int PlayerNumber
        {
            get => _playerNumber; set
            {
                if (_playerNumber < 0 && value >= 0)
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
                TrySetTarget(FindTarget());
            if (_cooldown > 0f)
                _cooldown -= Time.deltaTime;
            else if (TargetInRange)
                TryAttack();
        }

        private bool TryAttack()
        {
            if (_cooldown > 0f || _target == null)
                return false;
            if (TargetInRange == false)
            {
                if (_wasTargetAttacked && TryUpdateTarget())
                    return TryAttack();
                else
                    return false;
            }

            _target.GetDamage(_damage);
            _wasTargetAttacked = true;
            _cooldown += _attackCooldown;
            return true;
        }

        public AttackPriority PriorityToAttack(ActiveEntity target)
        {
            if (target.Tags.HasFlag(ActiveEntity.Flags.Building))
                return AttackPriority.Building;
            if (target.Tags.HasFlag(ActiveEntity.Flags.Flying))
                return AttackPriority.ImpossibleToAttack;

            return AttackPriority.Normal;
        }

        // call when got damage of target out of range
        public bool TryUpdateTarget()
        {

            ActiveEntity alternativeTarget = FindTarget();
            if (alternativeTarget == null)
                return false;
            if (_target == null)
            {
                _target = alternativeTarget;
                return true;
            }

            AttackPriority alternativeTypePriority = PriorityToAttack(alternativeTarget);
            if (alternativeTypePriority <= AttackPriority.ZeroDamage)
                return false;
            AttackPriority typePriority = PriorityToAttack(_target);
            if (typePriority <= AttackPriority.ZeroDamage)
            {
                _target = alternativeTarget;
                return true;
            }

            bool inRange = _target.SqrDistanceTo(transform) < _range * _range;
            bool inAlternativeRange = alternativeTarget.SqrDistanceTo(transform) < _range * _range;
            if (inRange == true && inAlternativeRange == false)
                return false;
            if (inRange == false && inAlternativeRange == true)
            {
                _target = alternativeTarget;
                return true;
            }

            float maxTypePriority = typePriority >= alternativeTypePriority ? (float)typePriority : (float)alternativeTypePriority;
            float priority = (float)typePriority + (1.0f - _target.HP / _target.MaxHp) * maxTypePriority;
            float alternativePriority = (float)alternativeTypePriority + (1.0f - alternativeTarget.HP / alternativeTarget.MaxHp) * maxTypePriority;

            if (priority >= alternativePriority)
                return false;
            _target = alternativeTarget;
            return true;
        }
        private ActiveEntity FindTarget()
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

        private bool TrySetTarget(ActiveEntity target)
        {
            if (target == null)
                return false;

            _target = target;
            _wasTargetAttacked = false;
            return true;
        }
    }
}

