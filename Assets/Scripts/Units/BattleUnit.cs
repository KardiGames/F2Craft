using UnityEngine;

public class BattleUnit : Unit
{
    [SerializeField] private ActiveEntity _target;
    [SerializeField] private int _damage;
    [SerializeField] float _attackDistance;
    [SerializeField] private float _attackCooldown;
    private float _cooldown=0f;

    public new void Init (int playerNumber) =>
        base.Init(playerNumber);

    private void Update()
    {
        if (_target == null)
            return;

        if (SqrDistanceTo(_target) > _attackDistance * _attackDistance)
            Move();
        else
            Attack();
    }

    private float SqrDistanceTo (ActiveEntity target) 
    {
        if ( _target == null )
        {
            print("Error! No target for DistanceTo");
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
}
