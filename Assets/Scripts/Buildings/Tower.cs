using UnityEngine;

public class Tower : Building
{
    [SerializeField] private ActiveEntity _target;
    [SerializeField] private int _damage;
    [SerializeField] float _attackDistance;
    [SerializeField] private float _attackCooldown;
    private float _cooldown=0f;


    private void Update()
    {
        if (_target == null)
            return;

            Attack();
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
}
