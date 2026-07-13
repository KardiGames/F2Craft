using UnityEngine;
using Battle;
using System;

public class BattleUnit : Unit
{
    [SerializeField] private Weapon _weapon;
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

        if (SqrDistanceTo(_target.transform) > _weapon.Range*_weapon.Range)
            Move();
        else
            Attack();
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



    private void SwitchTarget ()
    {
        throw new NotImplementedException();
    }
}
