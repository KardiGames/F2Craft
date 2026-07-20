using UnityEngine;
using Combat;
using System;

public class BattleUnit : Unit
{
    [SerializeField] private Weapon _weapon;
    [SerializeField] private Armor _armor;
    //public Weapon Weapon => _weapon;
    public Armor Armor => _armor;
    public new void Init(int playerNumber)
    {
        base.Init(playerNumber);
        if (_weapon != null )
            _weapon.PlayerNumber = playerNumber;
    }

    public override void TakeDamage(Combat.DamageData damage)
    {
        if (_armor != null )
            damage = _armor.ReduceDamage (damage);
        base.TakeDamage(damage);
        
        if (_hp > 0)
        {
            _weapon.TryUpdateTarget();
        }
    }

    protected override void Start()
    {
        base.Start();
        if (_weapon == null)
        {
            Debug.LogWarning("LINK to weapon was lost!");
            _weapon = GetComponent<Weapon>();
        }
        if (_armor == null)
        {
            Debug.LogWarning("LINK to armor was lost!");
            _armor = GetComponent<Armor>();
        }
    }
    private void Update()
    {
        Move();
    }

    private void Move ()
    {
        if (_weapon.Target == null || _weapon.TargetInRange) 
            return;
        Vector3 targetPoint = _weapon.Target.transform.position;
        targetPoint.y = transform.position.y;
        transform.LookAt(targetPoint);
        Vector3 moveVector = ((targetPoint - transform.position).normalized)*_moveSpeed*Time.deltaTime;
        
        transform.position = transform.position+ moveVector;
    }
}
