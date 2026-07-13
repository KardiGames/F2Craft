using UnityEngine;
using Battle;
using System;

public class BattleUnit : Unit
{
    [SerializeField] private Weapon _weapon;
    [SerializeField] private ActiveEntity _target;
    [SerializeField] private int _damage;
    [SerializeField] float _attackDistance;
    [SerializeField] private float _attackCooldown;
    private float _cooldown=0f;

    public new void Init (int playerNumber) =>
        base.Init(playerNumber);

    protected override void Start()
    {
        base.Start();
        if (_weapon == null)
        {
            Debug.LogWarning("LINK to weapon was lost!");
            _weapon = GetComponent<Weapon>();
        }
    }
    private void Update()
    {
        if (_target == null)
            _target = BestTarget();
        if (_target == null)
            return;

        if (SqrDistanceTo(_target) > _attackDistance * _attackDistance)
            Move();
        else
            Attack();
    }

    private float SqrDistanceTo (ActiveEntity target) 
    {
        if ( target == null )
        {
            Debug.LogError("No target for DistanceTo");
            return float.MaxValue;
        }
        float sqrDistance = (transform.position - target.transform.position).sqrMagnitude; //TODO m.b. change to 2D distance &&|| cash transform
        sqrDistance -= transform.localScale.x/2;
        sqrDistance -= target.transform.localScale.x/2;
        return sqrDistance;
    }

    private void Move ()
    {
        if (_target == null) 
            return;
        Vector3 targetPoint = _target.transform.position;
        targetPoint.y = transform.position.y;
        transform.LookAt(targetPoint);
        Vector3 moveVector = ((targetPoint - transform.position).normalized)*_moveSpeed*Time.deltaTime;
        
        transform.position = transform.position+ moveVector;
    }

    private void Attack()
    {
        if (_cooldown <= 0f)
        {
            _target.GetDamage(_damage);
            _cooldown += _attackCooldown;
        }
        else
            _cooldown-= Time.deltaTime;
    }

    private ActiveEntity BestTarget(float attackRange = 0)
    {
        float minSqrDistance = float.MaxValue;
        float sqrRange = attackRange*attackRange;
        ActiveEntity target = null;
        AttackPriority currentPriority = 0;
        foreach (ActiveEntity entity in ActiveEntity.GetEnemiesList(_playerNumber))
        {
            AttackPriority priority = _weapon.PriorityToAttack(entity);
            if (priority == AttackPriority.ImpossibleToAttack)
                continue;
            float sqrDistance = SqrDistanceTo(entity);
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

    private void SwitchTarget ()
    {
        throw new NotImplementedException();
    }
}
