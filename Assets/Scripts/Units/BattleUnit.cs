using UnityEngine;

public class BattleUnit : Unit
{
    [SerializeField] private ActiveEntity _target;
    [SerializeField] private int _damage;
    [SerializeField] float _attackDistance;
    [SerializeField] private float _attackCooldown;
    private float _timeToAttack=0f;

    private void Start() //CRUTCH
    {
        Init(0, 50, 50, GameObject.Find("SingleScripts").GetComponent<ActiveEntitiesManager>());
    }

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

        Vector3 moveVector = ((_target.transform.position - transform.position).normalized)*_moveSpeed*Time.deltaTime;
        
        transform.position = transform.position+ moveVector;

    }

    private void Attack()
    {
        if (_timeToAttack <= 0f)
        {
            _target.GetDamage(_damage);
            _timeToAttack += _attackCooldown;
        }
        else
            _timeToAttack-= Time.deltaTime;
    }
}
